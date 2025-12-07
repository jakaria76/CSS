using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class Advisor
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string? Name { get; set; }

        [Required, StringLength(100)]
        public string? Role { get; set; }

        [StringLength(800)]
        public string? Bio { get;set; }

        [Required(ErrorMessage = "Image data is required")]
        [Display(Name = "Image File")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
    }
}
