using System;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using BCrypt.Net;
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

        public async Task<(string accessToken, string refreshToken, UserDto user)> LoginWithGoogleAsync(string email, string googleId, string? name)
        {
            var user = await _userRepository.GetByGoogleIdAsync(googleId);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    GoogleId = googleId,
                    Name = name,
                    Role = UserRole.OWNER,
                };
                await _userRepository.CreateAsync(user);
            }

            var accessToken = _jwtHelper.GenerateAccessToken(user, 1);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            await _userRepository.SaveRefreshTokenAsync(user.Id!, refreshToken);

            return (accessToken, refreshToken, ToDto(user));
        }

        // =======================
        // Refresh Token
        // =======================
        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            // Buscar el usuario que tenga este refresh token válido
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new SecurityException("Invalid or expired refresh token");

            // Generar nuevos tokens
            var newAccessToken = _jwtHelper.GenerateAccessToken(user, 1); // 1 hora
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            // Guardar el nuevo refresh token
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
