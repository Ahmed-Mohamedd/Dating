using Api.Data.Entities;

namespace Api.Services
{
    public interface ITokenService
    {
         Task<string> CreateToken(AppUser user);
    }
}
