using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Movie.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movie.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(string userId, string username, IEnumerable<Claim>? additionalClaims = null)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"] ??
                throw new InvalidOperationException("JWT Key not configured"));
            var issuer = _configuration["JwtSettings:Issuer"] ??
                throw new InvalidOperationException("JWT Issuer not configured");
            var audience = _configuration["JwtSettings:Audience"] ??
                throw new InvalidOperationException("JWT Audience not configured");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username)
            };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}