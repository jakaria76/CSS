using System;
using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class Notice
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; }

        [Required]
        public byte[] FileData { get; set; }

        [Required]
        public string FileName { get; set; }

        // Always has Date + Time
        public DateTime PublishDate { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
