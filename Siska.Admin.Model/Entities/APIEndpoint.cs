using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities
{
    [Table("APIEndpoint")]
    public class APIEndpoint : Base
    {
        [Required]
        public string ApiPath { get; set; }

        [Required]
        public string ApiMethod { get; set; } = "GET";

        [Required]
        public string ApiDescription { get; set; }

        public List<Roles> Roles { get; set; }
    }
}
