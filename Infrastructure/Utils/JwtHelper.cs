using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Utils
{
    public class JwtHelper
    {
        private readonly string _secret;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtHelper(IHttpContextAccessor httpContextAccessor)
        {
            _secret = Environment.GetEnvironmentVariable("Jwt__Secret")
                      ?? throw new InvalidOperationException("JWT secret missing");
            _httpContextAccessor = httpContextAccessor;
        }

        // =======================
        // Generate Access Token (short-lived)
        // =======================
        public string GenerateAccessToken(User user, int hoursValid = 1)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(hoursValid),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // =======================
        // Generate Refresh Token (random, long-lived)
        // =======================
        public string GenerateRefreshToken(int size = 64)
        {
            var randomBytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        // =======================
        // Validate Access Token & Get ClaimsPrincipal
        // =======================
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // allow expired
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
                if (securityToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        // =======================
        // Extraer token desde Headers
        // =======================
        private string? GetTokenFromHeaders()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // =======================
        // Obtener Claim genérico
        // =======================
        private string? GetClaimFromToken(string claimType)
        {
            var token = GetTokenFromHeaders();
            if (token == null)
                return null;

            var principal = GetPrincipalFromExpiredToken(token);
            return principal?.FindFirst(claimType)?.Value;
        }

        // =======================
        // Obtener UserId
        // =======================
        public string? GetUserIdFromToken()
        {
            return GetClaimFromToken(ClaimTypes.NameIdentifier);
        }

        // =======================
        // Obtener Role
        // =======================
        public string? GetUserRoleFromToken()
        {
            return GetClaimFromToken(ClaimTypes.Role);
        }
    }
}
