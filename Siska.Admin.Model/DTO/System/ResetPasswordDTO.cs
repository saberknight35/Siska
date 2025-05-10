using System.ComponentModel.DataAnnotations;

namespace Siska.Admin.Model.DTO.System
{
    public class ResetPasswordDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
