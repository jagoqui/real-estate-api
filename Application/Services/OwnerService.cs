using RealEstate.Domain.Entities;
using RealEstate.Application.Contracts;
using RealEstate.Application.Exceptions;

namespace RealEstate.Application.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnerService(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository ?? throw new ArgumentNullException(nameof(ownerRepository));
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

        public async Task<Owner> AddOwnerAsync(OwnerWithoutId owner)
        {
            if (owner == null)
                throw new BadRequestException("Owner cannot be null.");

            try
            {
                return await _ownerRepository.AddOwnerAsync(createOwnerWithId(owner));
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("Error adding owner.", ex);
            }
        }

        public async Task<Owner> UpdateOwnerAsync(string id, OwnerWithoutId owner)
        {
            if (owner == null)
                throw new BadRequestException("Owner cannot be null.");

            Owner existingOwner = await EnsureOwnerExistsAsync(id);

            try
            {

                await _ownerRepository.UpdateOwnerAsync(existingOwner.IdOwner, createOwnerWithId(owner, existingOwner.IdOwner));

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
            await EnsureOwnerExistsAsync(id);

            try
            {
                await _ownerRepository.DeleteOwnerAsync(id);
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

        private Owner createOwnerWithId(OwnerWithoutId owner, string? id = null)
        {
            return id != null ? new Owner
            {
                IdOwner = id,
                Name = owner.Name,
                Address = owner.Address,
                Photo = owner.Photo,
                Birthday = owner.Birthday
            } : new Owner
            {
                Name = owner.Name,
                Address = owner.Address,
                Photo = owner.Photo,
                Birthday = owner.Birthday
            };
        }
    }
}
