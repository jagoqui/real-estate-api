using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs;

namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // =======================
        // Register
        // =======================
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var (accessToken, refreshToken, user) = await _authService.RegisterAsync(
                request.Email,
                request.Name,
                request.Password);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user,
            });
        }

        // =======================
        // Login
        // =======================
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
        {
            var (accessToken, refreshToken, user) = await _authService.LoginWithEmailAsync(
                request.Email,
                request.Password);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user,
            });
        }

        // =======================
        // Login with Google
        // =======================
        [HttpPost("google-login")]
        public async Task<ActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            var (accessToken, refreshToken, user) = await _authService.LoginWithGoogleAsync(
                request.Email,
                request.GoogleId,
                request.Name);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user,
            });
        }

        // =======================
        // Refresh Token
        // =======================
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var (accessToken, refreshToken) = await _authService.RefreshTokenAsync(request.RefreshToken);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }
    }
}
