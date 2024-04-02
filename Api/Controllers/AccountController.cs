using Api.Data;
using Api.Data.Entities;
using Api.DTOs;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

namespace Api.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly DatingContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(DatingContext context, ITokenService tokenService, IMapper mapper)
        {
            _context=context;
            _tokenService=tokenService;
            _mapper=mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register([FromBody] RegisterDto dto)
        {
            if (await UserExists(dto.UserName)) return BadRequest("UserName Is Taken");
            var user = _mapper.Map<AppUser>(dto);
            using var hmac = new HMACSHA512();


            user.UserName = dto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.password));
            user.PasswordSalt = hmac.Key;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var returneduser = new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
            return Ok(returneduser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login([FromBody]LoginDto dto)
        {
            var user = await _context.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (user is null) return Unauthorized("User Not Found");

            if (!IsPaswwordValid(user, dto.Password))
                return Unauthorized("password is invalid");

            var returneduser = new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl=user.Photos.SingleOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
            return Ok(returneduser);
        }



        private async Task<bool> UserExists(string username)
            => await  _context.Users.AnyAsync(user => user.UserName == username.ToLower());
    
    
        private bool IsPaswwordValid(AppUser user , string password)
        {
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var ComputedHash =  hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for(int  i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i])
                    return false;
            }
            return true;
        }
    }
}
