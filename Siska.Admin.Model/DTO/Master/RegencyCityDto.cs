namespace Siska.Admin.Model.DTO.Master
{
    public class RegencyCityDto
    {
        public int Id { get; set; }
        public string RegencyCode { get; set; } = null!;
        public string RegencyName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int ProvinceId { get; set; }
        public string? ProvinceName { get; set; } // For display purposes
        
        // Audit fields
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = null!;
        public DateTime ModifiedDate { get; set; }

        // For UI hierarchy display (optional)
        public List<DistrictDto>? Districts { get; set; }
    }
}