using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSS.ViewModels
{
    public class EventCreateVM
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string? Tag { get; set; }
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }

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

        public bool IsPublished { get; set; }
        public bool IsFeatured { get; set; }

        // File Uploads
        public IFormFile? BannerImage { get; set; }
        public List<IFormFile>? GalleryImages { get; set; }
    }
}
