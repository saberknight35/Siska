using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities.Master
{
    [Table("Provinces")]
    public class Province : Base
    {
        [StringLength(2)]
        [Column(TypeName = "CHAR(2)")]
        public string ProvinceCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ProvinceName { get; set; } = null!;

        // Navigation property with List
        public List<RegencyCity> RegenciesCities { get; set; } = new List<RegencyCity>();
    }
}