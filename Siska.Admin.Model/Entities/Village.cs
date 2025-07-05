using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities
{
    [Table("Villages")]
    public class Village : Base
    {
        [StringLength(10)]
        [Column(TypeName = "CHAR(10)")]
        public string VillageCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string VillageName { get; set; } = null!;

        [Required]
        [StringLength(1)]
        [Column(TypeName = "CHAR(1)")]
        public string Type { get; set; } = null!;  // 'D' = Desa, 'U' = Kelurahan

        [Required]
        [StringLength(6)]
        [Column(TypeName = "CHAR(6)")]
        public string DistrictCode { get; set; } = null!;

        // Navigation properties
        [ForeignKey("DistrictCode")]
        public District District { get; set; } = null!;
    }
}