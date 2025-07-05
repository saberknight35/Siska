using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities
{
    [Table("Districts")]
    public class District : Base
    {
        [StringLength(6)]
        [Column(TypeName = "CHAR(6)")]
        public string DistrictCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DistrictName { get; set; } = null!;

        [Required]
        [StringLength(4)]
        [Column(TypeName = "CHAR(4)")]
        public string RegencyCode { get; set; } = null!;

        // Navigation properties
        [ForeignKey("RegencyCode")]
        public RegencyCity RegencyCity { get; set; } = null!;
        
        public List<Village> Villages { get; set; } = new List<Village>();
    }
}