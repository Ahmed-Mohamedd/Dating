﻿using Api.Data.Entities;
using Api.DTOs;
using Api.Helpers;
using Api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.IO.Pipes;

namespace Api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DatingContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DatingContext context, IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }

        public async Task<MemberDto> GetMember(string username)
        {
            return await  _context.Users.Where(u => u.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembers(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            query = query.Where(u => u.Gender == userParams.Gender);

            var MaxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(- userParams.MinAge));
            var MinDob = DateOnly.FromDateTime(DateTime.Today.AddYears(- userParams.MaxAge -1));

            query = query.Where( u => u.DateOfBirth <= MaxDob && u.DateOfBirth >=MinDob);

            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider), 
                userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<AppUser> GetUserById(int id)
            => await _context.Users.FindAsync(id);

        public async Task<AppUser> GetUserByName(string username)
            => await _context.Users.Include(u=>u.Photos).FirstOrDefaultAsync(u => u.UserName == username);

        public async Task<IEnumerable<AppUser>> GetUsers()
            => await _context.Users.Include(u => u.Photos).ToListAsync();  

        public async Task<bool> Save()
            => await _context.SaveChangesAsync() > 0 ;

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
            //_context.Users.Update(user);
        }
    }
}
