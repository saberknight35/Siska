using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities
{
    [Table("RegenciesCities")]
    public class RegencyCity : Base
    {
        [StringLength(4)]
        [Column(TypeName = "CHAR(4)")]
        public string RegencyCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string RegencyName { get; set; } = null!;

        [Required]
        [StringLength(2)]
        [Column(TypeName = "CHAR(2)")]
        public string Type { get; set; } = null!;  // 'K', 'C', 'CA', 'KA'

        [Required]
        [StringLength(2)]
        [Column(TypeName = "CHAR(2)")]
        public string ProvinceCode { get; set; } = null!;

        // Navigation properties
        [ForeignKey("ProvinceCode")]
        public Province Province { get; set; } = null!;
        
        public List<District> Districts { get; set; } = new List<District>();
    }
}