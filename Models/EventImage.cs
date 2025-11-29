using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class EventImage
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Required]
        public byte[] ImageData { get; set; } = null!;

        [Required]
        public string ImageType { get; set; } = "image/jpeg";

        public string? Caption { get; set; }
    }
}
