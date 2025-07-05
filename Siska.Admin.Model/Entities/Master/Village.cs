using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities.Master
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

        // Foreign key to District using Id
        [Required]
        public int DistrictId { get; set; }

        // Navigation properties
        [ForeignKey("DistrictId")]
        public District District { get; set; } = null!;
    }
}