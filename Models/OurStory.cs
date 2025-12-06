using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class OurStory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required, StringLength(1000)]
        public string? Description { get; set; }
    }
}
