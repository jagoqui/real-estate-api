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
            var result = await _authService.RegisterAsync(
                request.Email,
                request.Name,
                request.Password);

            return Ok(new
            {
                result.AccessToken,
                result.RefreshToken,
                result.User,
            });
        }

        // =======================
        // Login
        // =======================
        [HttpPost("email-login")]
        public async Task<ActionResult> EmailLogin([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginWithEmailAsync(
                request.Email,
                request.Password);

            return Ok(new
            {
                result.AccessToken,
                result.RefreshToken,
                result.User,
            });
        }

        // =======================
        // Login with Google
        // =======================
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthCodeDto dto)
        {
            if (string.IsNullOrEmpty(dto.Code))
                return BadRequest("Authorization code is required.");

            var result = await _authService.LoginWithGoogleCodeAsync(dto.Code);
            return Ok(result);
        }

        // =======================
        // Refresh Token
        // =======================
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

            return Ok(new
            {
                result.AccessToken,
                result.RefreshToken,
            });
        }
    }
}
