using System.ComponentModel.DataAnnotations;
using Siska.Admin.Model.Entities;

namespace Siska.Admin.Model.DTO.System
{
    public record RoleDTO
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<APIEndpoint> APIEndpoint { get; set; }
    }
}
