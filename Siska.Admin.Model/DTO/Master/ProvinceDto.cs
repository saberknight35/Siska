namespace Siska.Admin.Model.DTO.Master
{
    public class ProvinceDto
    {
        public int Id { get; set; }
        public string ProvinceCode { get; set; } = null!;
        public string ProvinceName { get; set; } = null!;
        
        // Audit fields
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = null!;
        public DateTime ModifiedDate { get; set; }
        
        // For UI hierarchy display (optional)
        public List<RegencyCityDto>? RegenciesCities { get; set; }
    }
}