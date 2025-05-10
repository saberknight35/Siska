using System.ComponentModel.DataAnnotations;

namespace Siska.Admin.Model.DTO.System
{
    public class ChangePasswordDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
