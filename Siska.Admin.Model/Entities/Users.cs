using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities
{
    [Table("Users")]
    public class Users : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public int DataStatus { get; set; } = 1;

        public int? UserTelegramId { get; set; }

        public List<Roles> Roles { get; set; }

        public Users() { }
    }
}
