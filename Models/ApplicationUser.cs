using Microsoft.AspNetCore.Identity;
using System;

namespace CSS.Models
{
    public class ApplicationUser : IdentityUser
    {
        // BASIC
        public string? FullName { get; set; }
        public string? FullNameBn { get; set; }
        public string? MemberType { get; set; }    // Committee / Volunteer / Admin
        public string? CommitteePosition { get; set; }
        public DateTime? MemberSince { get; set; }

        // PERSONAL
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // CONTACT
        public string? AlternativeMobile { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? District { get; set; }
        public string? Upazila { get; set; }
        public string? FacebookLink { get; set; }
        public string? WhatsAppNumber { get; set; }

        // BLOOD / DONATION
        public string? BloodGroup { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public DateTime? NextAvailableDonationDate { get; set; }
        public string? DonationEligibility { get; set; }
        public int? TotalDonationCount { get; set; }
        public string? PreferredDonationLocation { get; set; }

        // EDUCATION - SCHOOL
        public string? SchoolName { get; set; }
        public string? SchoolGroup { get; set; }
        public int? SchoolPassingYear { get; set; }

        // EDUCATION - COLLEGE
        public string? CollegeName { get; set; }
        public string? CollegeGroup { get; set; }
        public int? CollegePassingYear { get; set; }

        // EDUCATION – UNIVERSITY
        public string? UniversityName { get; set; }
        public string? Department { get; set; }
        public string? StudentId { get; set; }
        public int? CurrentYear { get; set; }
        public int? CurrentSemester { get; set; }

        // BIO
        public string? ShortBio { get; set; }
        public string? WhyJoined { get; set; }
        public string? FutureGoals { get; set; }
        public string? Hobbies { get; set; }

        // SOCIAL
        public string? Facebook { get; set; }
        public string? PortfolioWebsite { get; set; }

        // SYSTEM
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? AccountStatus { get; set; }

        // PROFILE IMAGE
        public string? ProfileImagePath { get; set; }
        public byte[]? ProfileImageData { get; set; }

        // LOCATION
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LocationDms { get; set; }

        // ================================
        // 🔥 PUSH NOTIFICATION — WEB PUSH
        // ================================
        public string? PushEndpoint { get; set; }
        public string? PushP256dh { get; set; }
        public string? PushAuth { get; set; }

        
    }
}
