using System.ComponentModel.DataAnnotations;

namespace CSS.ViewModels
{
    public class OtpVerifyViewModel
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string? OtpCode { get; set; }
    }
}
