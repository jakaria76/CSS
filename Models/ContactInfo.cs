using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class ContactInfo
    {
        public int Id { get; set; }

        [Required,EmailAddress]
        public string? Email { get; set; }

        [Required,Phone]
        public string? Phone { get; set; }
        [Required,StringLength(300)]
        public string? Address { get; set; }

        [StringLength(300)]
        public string? FacebookUrl { get; set; }

        [StringLength(300)]
        public string? WebsiteUrl { get; set; }
    }
}
