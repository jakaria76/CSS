using System.ComponentModel.DataAnnotations;

namespace CSS.ViewModels
{
    public class AdminCreateUserVM
    {
        [Required] public string FullName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }

        public string? Mobile { get; set; }
        public string? AlternativeMobile { get; set; }   // NEW

        public string? MemberType { get; set; }
        public string? CommitteePosition { get; set; }
    }
}
