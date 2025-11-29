using CSS.Models;
using System.Collections.Generic;

namespace CSS.ViewModels
{
    public class EventListVM
    {
        public List<Event> Featured { get; set; } = new();
        public List<Event> Upcoming { get; set; } = new();
        public List<Event> Past { get; set; } = new();

        public List<string> Categories { get; set; } = new();
        public string? SelectedCategory { get; set; }
    }
}
