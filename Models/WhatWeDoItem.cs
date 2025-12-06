using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class WhatWeDoItem
    {

        public int Id { get;set; }

        [Required, StringLength(150)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? IconClass { get;set; }

        [Required(ErrorMessage ="Image data is required")]
        [Display(Name ="Image File")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
    }
}
