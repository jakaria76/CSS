using System;
using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class Video
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "YouTube URL")]
        [DataType(DataType.Url)]
        public string? YouTubeUrl { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        // Optional: store extracted video id for faster embed
        [StringLength(50)]
        public string? YouTubeId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: order index for custom ordering in home page
        public int SortOrder { get; set; } = 0;
    }
}
