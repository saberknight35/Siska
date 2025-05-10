using System.ComponentModel.DataAnnotations;
using Siska.Admin.Model.Entities;

namespace Siska.Admin.Model.DTO.System
{
    public class APIEndpointDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string ApiPath { get; set; }

        [Required]
        public string ApiMethod { get; set; } = "GET";

        [Required]
        public string ApiDescription { get; set; }

        public List<Roles> Roles { get; set; }
    }
}
