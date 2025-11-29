using System;
using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class EventRegistration
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        // BASIC INFO
        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string Mobile { get; set; } = "";

        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? InstituteName { get; set; }
        public string? ClassName { get; set; }

        public bool WillVolunteer { get; set; }
        public string? WhyJoin { get; set; }
        public string? PaymentMethod { get; set; }

        // IMAGE
        public byte[]? UserImage { get; set; }
        public string? UserImageType { get; set; }

        public string? UserId { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
