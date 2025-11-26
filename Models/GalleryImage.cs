using System;
using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Image Category")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Image data is required")]
        [Display(Name = "Image File")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        [Display(Name = "File Type")]
        public string? ContentType { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Uploaded By")]
        public string? UploadedBy { get; set; }
    }
}
