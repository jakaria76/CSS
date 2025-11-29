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

        // Nullable DateTime — required for Create form
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public string? Venue { get; set; }

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

        // Gallery images
        public virtual List<EventImage>? Images { get; set; }

        // Registrations for this event
        public virtual List<EventRegistration>? Registrations { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
