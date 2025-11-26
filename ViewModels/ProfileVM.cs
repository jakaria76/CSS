using Microsoft.AspNetCore.Http;
using System;

namespace CSS.ViewModels
{
    public class ProfileVM
    {
        public string? Id { get; set; }

        // BASIC
        public string? FullName { get; set; }
        public string? FullNameBn { get; set; }
        public string? MemberType { get; set; }
        public string? CommitteePosition { get; set; }
        public DateTime? MemberSince { get; set; }
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

        // BLOOD
        public string? BloodGroup { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public DateTime? NextAvailableDonationDate { get; set; }
        public string? DonationEligibility { get; set; }
        public int? TotalDonationCount { get; set; }
        public string? PreferredDonationLocation { get; set; }

        // EDUCATION
        public string? SchoolName { get; set; }
        public string? SchoolGroup { get; set; }
        public int? SchoolPassingYear { get; set; }
        public string? CollegeName { get; set; }
        public string? CollegeGroup { get; set; }
        public int? CollegePassingYear { get; set; }
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

        // IMAGE
        public IFormFile? ImageFile { get; set; }
        public string? ProfileImagePath { get; set; }

        // LOCATION
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LocationDms { get; set; }
    }
}
