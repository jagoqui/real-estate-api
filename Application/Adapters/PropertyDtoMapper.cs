namespace RealEstate.Application.Adapters
{
    using RealEstate.Domain.Entities;
    using RealEstate.Domain.Enums;
    using RealEstate.Infrastructure.DTOs;

    public static class PropertyDtoMapper
    {
        public static Property ToProperty(this PropertyRequestDto dto)
        {
            return new Property
            {
                Name = dto.Name,
                Address = dto.Address,
                Price = dto.Price,
                CodeInternal = dto.CodeInternal,
                Year = dto.Year,
                Description = dto.Description,
                Bathrooms = dto.Bathrooms,
                Bedrooms = dto.Bedrooms,
                AreaSqm = dto.AreaSqm,
                HighlightedFeatures = dto.HighlightedFeatures,
                Amenities = dto.Amenities,
                Featured = dto.Featured,
                Images = dto.Images,
                Views360Url = dto.Views360Url,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                Location = dto.Location,
                IdOwner = dto.IdOwner,
                Status = Enum.TryParse<PropertyStatus>(dto.Status, out var status) ? status : PropertyStatus.AVAILABLE,
                Type = Enum.TryParse<PropertyTypes>(dto.Type, out var type) ? type : PropertyTypes.OTHER,
                UpdatedAt = DateTime.UtcNow,
            };
        }
    }
}