using Movie.Core.Entities;
using Movie.Core.Models;

namespace Movie.Core.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponse?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password);
        Task<UserEntity?> GetByIdAsync(string id);
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}