using Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace Api.Data.Seeding
{
    public static class UserSeeding
    {
        public static  async Task SeedAsync(DatingContext context , ILoggerFactory loggerFactory)
        {
			try
			{
                if (await context.Users.AnyAsync()) return;
                
                var UsersData = File.ReadAllText("Data/Seeding/UserSeedData.json");
                var options  = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var Users = JsonSerializer.Deserialize<List<AppUser>>(UsersData);
                foreach (var user in Users)
                {
                    using var hmac = new HMACSHA512();
                    user.UserName = user.UserName.ToLower();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("123456"));
                    user.PasswordSalt = hmac.Key;
                    await context.Users.AddAsync(user);
                }
                await context.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {

                var logger = loggerFactory.CreateLogger<DatingContext>();
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
