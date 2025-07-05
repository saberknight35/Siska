namespace Siska.Admin.Model.DTO.Master
{
    public class DistrictDto
    {
        public int Id { get; set; }
        public string DistrictCode { get; set; } = null!;
        public string DistrictName { get; set; } = null!;
        public int RegencyCityId { get; set; }
        public string? RegencyCityName { get; set; } // For display purposes
        
        // Audit fields
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = null!;
        public DateTime ModifiedDate { get; set; }

        // For UI hierarchy display (optional)
        public List<VillageDto>? Villages { get; set; }
    }
}