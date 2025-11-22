using System.ComponentModel.DataAnnotations;

namespace CSS.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}
