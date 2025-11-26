using System.ComponentModel.DataAnnotations;

namespace CSS.ViewModels
{
    public class AdminEditUserVM
    {
        [Required] public string Id { get; set; }

        [Required] public string FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? MemberType { get; set; }
        public string? CommitteePosition { get; set; }

        public string? PresentAddress { get; set; }
        public string? District { get; set; }
        public string? AlternativeMobile { get; set; }

    }
}
