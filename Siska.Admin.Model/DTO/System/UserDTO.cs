using System.ComponentModel.DataAnnotations;

namespace Siska.Admin.Model.DTO.System
{
    public record UserDTO
    {
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        public IList<string>? Roles { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public int DataStatus { get; set; }
        public int? UserTelegramId { get; set; }
    }
}
