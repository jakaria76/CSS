using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class PreviousPresident
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = "";

        // ✔ Date Picker Support (Correct Type)
        [Required(ErrorMessage = "Tenure Start date is required.")]
        [DataType(DataType.Date)]
        public DateTime TenureStart { get; set; }

        [Required(ErrorMessage = "Tenure End date is required.")]
        [DataType(DataType.Date)]
        public DateTime TenureEnd { get; set; }

        // ✔ Typo Fixed: LegacyNote
        [StringLength(800)]
        public string? LegacyNote { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
    }
}
