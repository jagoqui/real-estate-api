using RealEstate.Application.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.API.Exceptions;
using RealEstate.Infrastructure.Utils;

namespace RealEstate.Infrastructure.API.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly JwtHelper _jwtHelper;

        public OwnerService(IOwnerRepository ownerRepository, IPropertyRepository propertyRepository, JwtHelper jwtHelper)
        {
            _ownerRepository = ownerRepository ?? throw new ArgumentNullException(nameof(ownerRepository));
            _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
        }

        public async Task<IEnumerable<Owner>> GetAllOwnersAsync()
        {
            try
            {
                return await _ownerRepository.GetOwnersAsync();
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error retrieving owners.", ex);
            }
        }

        public async Task<Owner?> GetOwnerByIdAsync(string id)
        {
            return await EnsureOwnerExistsAsync(id);
        }

        public async Task<Owner?> GetOwnerByUserIdAsync(string userId)
        {
            EnsureUserIsAuthorized(userId);
            return await _ownerRepository.GetOwnerByUserIdAsync(userId)
                   ?? throw new NotFoundException($"No owner found with User ID {userId}.");
        }

        public async Task<Owner> AddOwnerAsync(OwnerWithoutIds owner)
        {
            if (owner == null)
                throw new BadRequestException("Owner cannot be null.");

            var userId = _jwtHelper.GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid or missing JWT.");

            var newOwner = new OwnerWithoutIds
            {
                Name = owner.Name,
                Address = owner.Address,
                Photo = owner.Photo,
                Birthday = owner.Birthday,
            };

            try
            {
                return await _ownerRepository.AddOwnerAsync(CreateOwnerWithId(newOwner, userId));
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error adding owner.", ex);
            }
        }

        public async Task<Owner> UpdateOwnerAsync(string id, OwnerWithoutIds owner)
        {
            if (owner == null)
                throw new BadRequestException("Owner cannot be null.");

            var existingOwner = await EnsureOwnerExistsAsync(id);
            EnsureUserCanModifyOwner(existingOwner);

            try
            {
                await _ownerRepository.UpdateOwnerAsync(
                    existingOwner.IdOwner,
                    CreateOwnerWithId(owner, existingOwner.UserId, existingOwner.IdOwner));

                return await _ownerRepository.GetOwnerByIdAsync(id)
                       ?? throw new InternalServerErrorException("Failed to retrieve the updated owner.");
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error updating owner with ID {id}.", ex);
            }
        }

        public async Task DeleteOwnerAsync(string id)
        {
            var existingOwner = await EnsureOwnerExistsAsync(id);
            EnsureUserCanModifyOwner(existingOwner);

            try
            {
                var properties = await _propertyRepository.GetPropertiesByOwnerIdAsync(id);
                if (properties.Any())
                {
                    throw new BadRequestException($"Owner with ID {id} cannot be deleted because it has {properties.Count()} properties assigned.");
                }

                await _ownerRepository.DeleteOwnerAsync(id);
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException($"Error deleting owner with ID {id}.", ex);
            }
        }

        private async Task<Owner> EnsureOwnerExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequestException("Owner ID cannot be empty.");

            var owner = await _ownerRepository.GetOwnerByIdAsync(id);
            if (owner == null)
                throw new NotFoundException($"No owner found with ID {id}.");

            return owner;
        }

        private void EnsureUserIsAuthorized(string userId)
        {
            var tokenUserId = _jwtHelper.GetUserIdFromToken();
            var role = _jwtHelper.GetUserRoleFromToken();

            if (role != UserRole.ADMIN.ToString() && tokenUserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to access this resource.");
        }

        private void EnsureUserCanModifyOwner(Owner owner)
        {
            var tokenUserId = _jwtHelper.GetUserIdFromToken();
            var role = _jwtHelper.GetUserRoleFromToken();

            if (role != UserRole.ADMIN.ToString() && owner.UserId != tokenUserId)
                throw new UnauthorizedAccessException("You are not authorized to modify this owner.");
        }

        private Owner CreateOwnerWithId(OwnerWithoutIds owner, string userId, string? id = null)
        {
            return id != null
                ? new Owner
                {
                    IdOwner = id,
                    UserId = userId,
                    Name = owner.Name,
                    Address = owner.Address,
                    Photo = owner.Photo,
                    Birthday = owner.Birthday,
                }
                : new Owner
                {
                    UserId = userId,
                    Name = owner.Name,
                    Address = owner.Address,
                    Photo = owner.Photo,
                    Birthday = owner.Birthday,
                };
        }
    }
}
