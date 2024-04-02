using Api.Data;
using Api.Data.Entities;
using Api.DTOs;
using Api.Extensions;
using Api.Interfaces;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository=userRepository;
            _mapper=mapper;
            _photoService=photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetUsers()
        {
            var members = (IReadOnlyList<MemberDto>) await _userRepository.GetMembers();
            return Ok(members);
        }



        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUserByUsername(string username)
        {
            var user = await _userRepository.GetMember(username);
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto dto)
        {
            var user  = await _userRepository.GetUserByName(User.GetUuerName());
            if (user == null) return NotFound();

            _mapper.Map(dto, user);
            if (await _userRepository.Save()) return NoContent();
            return BadRequest("Failed to Update USer");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile File)
        {
            var user = await _userRepository.GetUserByName(User.GetUuerName());
            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(File);
            if(result.Error != null)  return BadRequest(result.Error.Message);

            var photo = new Photo()
            {
                Url = result.Url.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count ==0 )
                photo.IsMain = true;

            user.Photos.Add(photo);
            if (await _userRepository.Save()) return CreatedAtAction(nameof(GetUsers) , new {username = user.UserName} , _mapper.Map<PhotoDto>(photo));

            return BadRequest("ooops!,problem happened while adding a photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByName(User.GetUuerName());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest(" photo is already your main one!");

            var currentMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);
            if (currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
                photo.IsMain = true;
            }
            if(await _userRepository.Save()) return NoContent();

            return BadRequest("problem happenend while setting your main photo");

         
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByName(User.GetUuerName());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault( p=> p.Id == photoId);
            if (photo is null) return NotFound();
            if (photo.IsMain) return BadRequest("u can't delete your main photo");

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result?.Error?.Message);                
            }
            user.Photos.Remove(photo);

            if (await _userRepository.Save()) return Ok();
            return BadRequest("An error occurred while trying your delete your photo");

        }
    }
}
