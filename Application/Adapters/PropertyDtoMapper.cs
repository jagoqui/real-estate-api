namespace RealEstate.Application.Adapters
{
    using RealEstate.Domain.Entities;
    using RealEstate.Domain.Enums;
    using RealEstate.Infrastructure.DTOs;

    public static class PropertyDtoMapper
    {
        public static Property ToProperty(this PropertyRequestDto dto, List<string> imagesUrl, string? coverImageUrl = null, string? id = null)
        {
            var property = new Property
            {
                Name = dto.Name,
                Address = dto.Address,
                Price = dto.Price,
                CodeInternal = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                Year = dto.Year,
                Description = dto.Description,
                Bathrooms = dto.Bathrooms,
                Bedrooms = dto.Bedrooms,
                AreaSqm = dto.AreaSqm,
                HighlightedFeatures = dto.HighlightedFeatures,
                Amenities = dto.Amenities.Select(a => new Amenity
                {
                    Name = a.Name,
                    Icon = a.Icon,
                }).ToList(),
                Featured = dto.Featured,
                Images = imagesUrl,
                CoverImage = coverImageUrl,
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

            if (!string.IsNullOrEmpty(id))
            {
                property.Id = id;
            }

            return property;
        }

        public static PropertyResponseDto ToPropertyResponseDto(this Property property)
        {
            return new PropertyResponseDto
            {
                Id = property.Id,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                Description = property.Description,
                Bathrooms = property.Bathrooms,
                Bedrooms = property.Bedrooms,
                AreaSqm = property.AreaSqm,
                HighlightedFeatures = property.HighlightedFeatures,
                Amenities = property.Amenities.Select(a => new AmenityDto
                {
                    Name = a.Name,
                    Icon = a.Icon,
                }).ToList(),
                Featured = property.Featured,
                Images = property.Images,
                CoverImage = property.CoverImage,
                Views360Url = property.Views360Url,
                City = property.City,
                State = property.State,
                Country = property.Country,
                Location = property.Location,
                IdOwner = property.IdOwner,
                Status = property.Status.ToString(),
                Type = property.Type.ToString(),
                CreatedAt = property.CreatedAt,
                UpdatedAt = property.UpdatedAt,
            };
        }
    }
}