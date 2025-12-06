using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class OrganizationOverview
    {
        public int Id { get; set; }

        [Required]
        public string? Description { get; set; }

        [Range(1900, 2100)]
        public int FoundedYear { get; set; }

        [StringLength(1500)]
        public string? Purpose { get; set; }
    }
}
