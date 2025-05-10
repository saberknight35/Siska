using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities
{
    [Table("Roles")]
    public class Roles : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public List<APIEndpoint> APIEndpoint { get; set; }
        public List<Users> Users { get; set; }
    }
}
