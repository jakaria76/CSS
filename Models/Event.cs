using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = "";

        // Comma-separated categories/tags
        public string? Tag { get; set; }

        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }

        // Event Date-Time
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public string? Venue { get; set; }

        // Location Coordinates
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? OrganizedBy { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }

        public int ExpectedParticipants { get; set; }
        public int VolunteersNeeded { get; set; }

        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        // Banner image
        public byte[]? BannerImage { get; set; }
        public string? BannerImageType { get; set; }

        // PRICE (added for payment)
        [Range(0, 100000, ErrorMessage = "Price must be positive")]
        public decimal Price { get; set; } = 0;  // default value set

        // Gallery images
        public virtual List<EventImage>? Images { get; set; }

        // Registrations
        public virtual List<EventRegistration>? Registrations { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
