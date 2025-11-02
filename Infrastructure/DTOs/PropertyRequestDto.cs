using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Infrastructure.DTOs
{
    public class PropertyRequestDto : PropertyBase
    {
        public new string IdOwner { get; set; } = null!;
        public new string Status { get; set; } = nameof(PropertyStatus.AVAILABLE);
        public new string Type { get; set; } = nameof(PropertyTypes.OTHER);
    }
}
