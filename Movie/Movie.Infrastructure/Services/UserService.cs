using Microsoft.EntityFrameworkCore;
using Movie.Core.Entities;
using Movie.Core.Interfaces;
using Movie.Core.Models;
using Movie.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace Movie.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly MovieDbContext _dbContext;
        private readonly IJwtTokenService _jwtTokenService;

        public UserService(MovieDbContext dbContext, IJwtTokenService jwtTokenService)
        {
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponse?> AuthenticateAsync(string username, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Username == username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash))
                return null;

            var token = _jwtTokenService.GenerateJwtToken(user.Id, user.Username);

            return new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Token = token
            };
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            if (await UsernameExistsAsync(username) || await EmailExistsAsync(email))
                return false;

            var user = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<UserEntity?> GetByIdAsync(string id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private static bool VerifyPasswordHash(string password, string storedHash)
        {
            string computedHash = HashPassword(password);
            return computedHash == storedHash;
        }
    }
}