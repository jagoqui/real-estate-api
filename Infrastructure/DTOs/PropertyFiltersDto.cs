using RealEstate.Domain.Enums;

namespace RealEstate.Infrastructure.DTOs
{
    public class PropertyFiltersDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Location { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MaxBedrooms { get; set; }
        public int? MinBathrooms { get; set; }
        public int? MaxBathrooms { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public PropertyTypes? PropertyType { get; set; }
        public PropertyStatus? PropertyStatus { get; set; }
    }
}