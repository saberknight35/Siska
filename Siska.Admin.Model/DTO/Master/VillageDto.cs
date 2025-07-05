namespace Siska.Admin.Model.DTO.Master
{
    public class VillageDto
    {
        public int Id { get; set; }
        public string VillageCode { get; set; } = null!;
        public string VillageName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int DistrictId { get; set; }
        public string? DistrictName { get; set; } // For display purposes
        
        // Audit fields
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = null!;
        public DateTime ModifiedDate { get; set; }
    }
}