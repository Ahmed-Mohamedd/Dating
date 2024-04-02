using Api.Data.Entities;
using Api.DTOs;
using Api.Helpers;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetUsers();
        Task<AppUser> GetUserById(int id);
        Task<AppUser> GetUserByName(string username);

        void Update(AppUser user);
        Task<bool> Save();

        Task<PagedList<MemberDto>> GetMembers(UserParams userParams);
        Task<MemberDto> GetMember(string username);

    }
}
