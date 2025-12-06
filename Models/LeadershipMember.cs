using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class LeadershipMember
    {
        public int Id { get; set; }

        [Required,StringLength(150)]
        public string? Name { get; set; }

        [Required, StringLength(150)]
        public string? Position { get; set; }

        [StringLength(150)]
        public string? ShortMessage { get; set; }

        [Required(ErrorMessage ="Image data is required")]
        [Display(Name ="Image File")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();


    }
}
