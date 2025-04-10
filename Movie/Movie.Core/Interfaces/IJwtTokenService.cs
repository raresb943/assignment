using System.Security.Claims;

namespace Movie.Core.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(string userId, string username, IEnumerable<Claim>? additionalClaims = null);
    }
}