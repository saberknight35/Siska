using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities.Master
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

        // Foreign key to RegencyCity using Id
        [Required]
        public int RegencyCityId { get; set; }

        // Navigation properties
        [ForeignKey("RegencyCityId")]
        public RegencyCity RegencyCity { get; set; } = null!;
        
        public List<Village> Villages { get; set; } = new List<Village>();
    }
}