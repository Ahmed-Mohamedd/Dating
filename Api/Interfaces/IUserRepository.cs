using Api.Data.Entities;
using Api.DTOs;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetUsers();
        Task<AppUser> GetUserById(int id);
        Task<AppUser> GetUserByName(string username);

        void Update(AppUser user);
        Task<bool> Save();

        Task<IEnumerable<MemberDto>> GetMembers();
        Task<MemberDto> GetMember(string username);

    }
}
