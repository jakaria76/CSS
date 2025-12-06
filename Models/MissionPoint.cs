using System.ComponentModel.DataAnnotations;

namespace CSS.Models
{
    public class MissionPoint
    {
        public int Id { get; set; }

        [Required,StringLength(250)]
        public string? Text { get;set; }
    }
}
