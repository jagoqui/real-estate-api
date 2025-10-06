using System.Net.Mime;
using System.Security.Claims;
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
        private readonly IOwnerRepository _ownerRepository;
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageUploadService _imageUploadService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(
            IUserRepository userRepository,
            IOwnerRepository ownerRepository,
            JwtHelper jwtHelper,
            IHttpContextAccessor httpContextAccessor,
            IImageUploadService imageUploadService,
            IHttpClientFactory httpClientFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _ownerRepository = ownerRepository ?? throw new ArgumentNullException(nameof(ownerRepository));
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _imageUploadService = imageUploadService ?? throw new ArgumentNullException(nameof(imageUploadService));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<AuthResponseDto> RegisterAsync(string email, string name, string password)
        {
            ValidateEmail(email);
            ValidatePassword(password);

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("A user already exists with that email.");

            UserRole role = email.Contains("@admin.com") ? UserRole.ADMIN : UserRole.OWNER;

            var user = new User
            {
                Email = email,
                Name = name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
            };

            await _userRepository.CreateAsync(user);

            await SyncOwnerAsync(user);

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

        public async Task<AuthResponseDto> LoginWithEmailAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            await SyncOwnerAsync(user);

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

        public async Task<AuthResponseDto> LoginWithGoogleCodeAsync(string code)
        {
            var googleClientId = Environment.GetEnvironmentVariable("Authentication__GoogleClientId")
                             ?? throw new InvalidOperationException("GOOGLE_CLIENT_ID is not set.");
            var googleClientSecret = Environment.GetEnvironmentVariable("Authentication__GoogleClientSecret")
                                     ?? throw new InvalidOperationException("GOOGLE_CLIENT_SECRET is not set.");
            var httpContext = _httpContextAccessor.HttpContext
                              ?? throw new InvalidOperationException("No HttpContext available");

            using var tokenHttpClient = _httpClientFactory.CreateClient();

            var origin = httpContext.Request.Headers["Origin"].FirstOrDefault();

            if (string.IsNullOrEmpty(origin))
                throw new InvalidOperationException("Origin header is missing.");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", googleClientId },
                    { "client_secret", googleClientSecret },
                    { "redirect_uri", origin },
                    { "grant_type", "authorization_code" },
                }),
            };

            var response = await tokenHttpClient.SendAsync(request);
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
                string? cloudinaryPhotoUrl = null;
                string googlePhotoUrl = validPayload.Picture;

                if (!string.IsNullOrEmpty(googlePhotoUrl))
                {
                    try
                    {
                        using var imageHttpClient = _httpClientFactory.CreateClient();
                        var imageResponse = await imageHttpClient.GetAsync(googlePhotoUrl);

                        if (imageResponse.IsSuccessStatusCode)
                        {
                            var imageStream = await imageResponse.Content.ReadAsStreamAsync();

                            using var memoryStream = new MemoryStream();
                            await imageStream.CopyToAsync(memoryStream);
                            memoryStream.Position = 0;

                            var contentType = imageResponse.Content.Headers.ContentType?.MediaType ?? MediaTypeNames.Image.Jpeg;

                            var fileExtension = contentType.Split('/').Last();
                            var fileName = $"google_{validPayload.Subject}.{fileExtension}";

                            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = contentType,
                            };

                            cloudinaryPhotoUrl = await _imageUploadService.UploadImageAsync(formFile, "user");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Cloudinary upload for Google photo failed: {ex.Message}");
                    }
                }

                user = new User
                {
                    Email = validPayload.Email,
                    Name = validPayload.Name,
                    GoogleId = validPayload.Subject,

                    PhotoUrl = cloudinaryPhotoUrl ?? validPayload.Picture,
                    Role = UserRole.OWNER,
                };

                await _userRepository.CreateAsync(user);
            }

            await SyncOwnerAsync(user);

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

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var newAccessToken = _jwtHelper.GenerateAccessToken(user, 1);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            await _userRepository.SaveRefreshTokenAsync(user.Id!, newRefreshToken);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                User = ToDto(user),
            };
        }

        public async Task LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("No HttpContext available");

            var user = httpContext.User;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("JWT does not contain a valid user identifier.");

            await _userRepository.SaveRefreshTokenAsync(userId, string.Empty);
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

        private async Task SyncOwnerAsync(User user)
        {
            var existingOwner = await _ownerRepository.GetOwnerByUserIdAsync(user.Id!);

            if (existingOwner == null)
            {
                var newOwner = new Owner
                {
                    UserId = user.Id!,
                    Name = user.Name ?? string.Empty,
                    Address = string.Empty,
                    Photo = user.PhotoUrl ?? string.Empty,
                    Birthday = DateTime.MinValue,
                };

                await _ownerRepository.AddOwnerAsync(newOwner);
            }
            else
            {
                existingOwner.Name = user.Name ?? existingOwner.Name;
                existingOwner.Photo = user.PhotoUrl ?? existingOwner.Photo;

                await _ownerRepository.UpdateOwnerAsync(existingOwner);
            }
        }
    }
}
