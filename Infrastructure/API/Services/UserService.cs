using RealEstate.Application.Contracts;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.DTOs;
using RealEstate.Infrastructure.Utils;

namespace RealEstate.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly JwtHelper _jwtHelper;
        private readonly IImageUploadService _imageUploadService;

        public UserService(IUserRepository repository, JwtHelper jwtHelper, IImageUploadService imageUploadService)
        {
            _repository = repository;
            _jwtHelper = jwtHelper;
            _imageUploadService = imageUploadService;
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto request)
        {
            var existingUser = await _repository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("A user with this email already exists.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = passwordHash,
                Role = UserRole.OWNER,
            };

            var createdUser = await _repository.CreateAsync(user);

            return new UserDto
            {
                Id = createdUser.Id!,
                Email = createdUser.Email,
                Name = createdUser.Name,
                GoogleId = createdUser.GoogleId,
                Role = createdUser.Role,
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id!,
                Email = u.Email,
                Name = u.Name,
                GoogleId = u.GoogleId,
                Role = u.Role,
            });
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            return new UserDto
            {
                Id = user.Id!,
                Email = user.Email,
                Name = user.Name,
                GoogleId = user.GoogleId,
                Role = user.Role,
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersWithoutOwnersAsync()
        {
            var users = await _repository.GetUserWithoutOwnersAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id!,
                Email = u.Email,
                Name = u.Name,
                GoogleId = u.GoogleId,
                Role = u.Role,
            });
        }

        public async Task<UserDto> UpdateAsync(string id, UserWithFileDto user)
        {
            if (id != user.Id)
            {
                throw new ArgumentException("User ID mismatch.");
            }

            var userId = _jwtHelper.GetUserIdFromToken();
            var userRole = _jwtHelper.GetUserRoleFromToken();

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid or missing JWT.");
            }

            var existingUser = await _repository.GetByIdAsync(id);

            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            var isCurrentUser = existingUser.Id == userId;

            var isAdmin = userRole == UserRole.ADMIN.ToString();

            if (!isCurrentUser && !isAdmin)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this user.");
            }

            if (user.PhotoFile != null)
            {
                var imageUrl = await _imageUploadService.UploadImageAsync(user.PhotoFile, "users");

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    user.PhotoUrl = imageUrl;
                }

                if (!string.IsNullOrEmpty(existingUser.PhotoUrl))
                {
                    await _imageUploadService.DeleteImageAsync(existingUser.PhotoUrl);
                }
            }

            var userUpdateResult = await _repository.UpdateAsync(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                PhotoUrl = user.PhotoUrl,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
            });

            if (userUpdateResult == null)
            {
                await _imageUploadService.DeleteImageAsync(user.PhotoUrl!);

                throw new KeyNotFoundException($"User with ID {user.Id} not found after image upload.");
            }

            return new UserDto
            {
                Id = userUpdateResult.Id!,
                Email = userUpdateResult.Email,
                Name = userUpdateResult.Name,
                GoogleId = userUpdateResult.GoogleId,
                Role = userUpdateResult.Role,
                PhoneNumber = userUpdateResult.PhoneNumber,
                Bio = userUpdateResult.Bio,
                PhotoUrl = userUpdateResult.PhotoUrl,
            };
        }

        public async Task<UserDto> RecoverPasswordAsync(RecoverPasswordRequest request)
        {
            var (userId, email, newPassword) = (request.UserId, request.Email, request.NewPassword);

            ValidatePassword(newPassword);

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            var user = await _repository.RecoverAsync(userId, email, newPasswordHash);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found or email does not match.");
            }

            return new UserDto
            {
                Id = user.Id!,
                Email = user.Email,
                Name = user.Name,
                GoogleId = user.GoogleId,
                Role = user.Role,
            };
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            await _repository.DeleteAsync(userId);
            return true;
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
