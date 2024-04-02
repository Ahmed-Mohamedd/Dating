using Api.Data.Entities;
using Api.DTOs;
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

        public async Task<IEnumerable<MemberDto>> GetMembers()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
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
