using Microsoft.AspNetCore.Mvc;
using Movie.Core.Interfaces;
using Movie.Core.Models;

namespace Movie.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userService.AuthenticateAsync(request.Username, request.Password);

            if (response == null)
                return Unauthorized(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userService.UsernameExistsAsync(request.Username))
                return Conflict(new { message = "Username is already taken" });

            if (await _userService.EmailExistsAsync(request.Email))
                return Conflict(new { message = "Email is already registered" });

            var success = await _userService.RegisterAsync(
                request.Username,
                request.Email,
                request.Password);

            if (!success)
                return BadRequest(new { message = "User registration failed" });

            return Ok(new { message = "Registration successful. You can now login." });
        }
    }
}