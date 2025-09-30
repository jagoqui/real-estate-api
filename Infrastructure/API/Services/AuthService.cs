using System;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using BCrypt.Net;
using Google.Apis.Auth;
using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Utils;

namespace RealEstate.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
        }

        public async Task<(string accessToken, string refreshToken, UserDto user)> RegisterAsync(string email, string name, string password)
        {
            ValidateEmail(email);
            ValidatePassword(password);

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("A user already exists with that email.");

            var user = new User
            {
                Email = email,
                Name = name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = UserRole.OWNER,
            };

            await _userRepository.CreateAsync(user);

            var accessToken = _jwtHelper.GenerateAccessToken(user, 1); // 1 hora
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            await _userRepository.SaveRefreshTokenAsync(user.Id!, refreshToken);

            return (accessToken, refreshToken, ToDto(user));
        }

        public async Task<(string accessToken, string refreshToken, UserDto user)> LoginWithEmailAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var accessToken = _jwtHelper.GenerateAccessToken(user, 1); // 1 hora
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            await _userRepository.SaveRefreshTokenAsync(user.Id!, refreshToken);

            return (accessToken, refreshToken, ToDto(user));
        }

        public async Task<AuthResponseDto> LoginWithGoogleCodeAsync(string code)
        {
            var googleClientId = Environment.GetEnvironmentVariable("Authentication__GoogleClientId")
                     ?? throw new InvalidOperationException("GOOGLE_CLIENT_ID is not set.");
            var googleClientSecret = Environment.GetEnvironmentVariable("Authentication__GoogleClientSecret")
                                     ?? throw new InvalidOperationException("GOOGLE_CLIENT_SECRET is not set.");
            var redirectUri = Environment.GetEnvironmentVariable("Authentication__GoogleRedirectUri") ?? "http://localhost";

            using var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", googleClientId },
            { "client_secret", googleClientSecret },
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" },
        }),
            };

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new UnauthorizedAccessException("Failed to exchange Google authorization code.");

            var payload = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
            if (payload == null || string.IsNullOrEmpty(payload.IdToken))
                throw new UnauthorizedAccessException("Invalid Google token response.");

            var validPayload = await GoogleJsonWebSignature.ValidateAsync(payload.IdToken);
            if (validPayload == null)
                throw new UnauthorizedAccessException("Invalid Google ID token.");

            var user = await _userRepository.GetByGoogleIdAsync(validPayload.Subject);

            if (user == null)
            {
                user = new User
                {
                    Email = validPayload.Email,
                    Name = validPayload.Name,
                    GoogleId = validPayload.Subject,
                    PhotoUrl = validPayload.Picture,
                    Role = UserRole.OWNER,
                };

                await _userRepository.CreateAsync(user);
            }
            else
            {
                user.Email = validPayload.Email;
                user.Name = validPayload.Name;
                user.PhotoUrl = validPayload.Picture;

                await _userRepository.UpdateAsync(user);
            }

            var accessToken = _jwtHelper.GenerateAccessToken(user, 1); // 1 hora
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            await _userRepository.SaveRefreshTokenAsync(user.Id!, refreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = ToDto(user),
            };
        }

        // =======================
        // Refresh Token
        // =======================
        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            // Buscar usuario que tenga este refresh token
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            // Generar nuevos tokens
            var newAccessToken = _jwtHelper.GenerateAccessToken(user, 1); // 1 hora
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            // Guardar el nuevo refresh token e invalidar el anterior
            await _userRepository.SaveRefreshTokenAsync(user.Id!, newRefreshToken);

            return (newAccessToken, newRefreshToken);
        }

        // =======================
        // Helpers
        // =======================
        private UserDto ToDto(User user) => new()
        {
            Id = user.Id!,
            Email = user.Email,
            Name = user.Name,
            PhotoUrl = user.PhotoUrl,
            Role = user.Role,
        };

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern))
                throw new ArgumentException("Email format is invalid.");
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, pattern))
                throw new ArgumentException("Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.");
        }
    }
}
