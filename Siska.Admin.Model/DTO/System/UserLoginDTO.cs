using System.ComponentModel.DataAnnotations;

namespace Siska.Admin.Model.DTO.System
{
    public record UserLoginDTO
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
