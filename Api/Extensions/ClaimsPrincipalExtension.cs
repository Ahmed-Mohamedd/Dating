using System.Security.Claims;

namespace Api.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetUuerName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
