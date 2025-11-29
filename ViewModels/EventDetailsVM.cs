using CSS.Models;
using System.Collections.Generic;

namespace CSS.ViewModels
{
    public class EventDetailsVM
    {
        public Event Event { get; set; } = null!;
        public List<EventImage>? Images { get; set; }
        public int RegisteredCount { get; set; }
    }
}
