using CSS.Models;

namespace CSS.ViewModels
{
    public class AboutVM
    {

        public List<Advisor>? Advisors { get; set; } 
        public List<PreviousPresident>? PreviousPresidents { get; set; }
        public List<LeadershipMember>? Leadership { get; set; }
        public OrganizationOverview? Overview { get; set; }
        public List<MissionPoint>? MissionPoints { get; set; }
        public List<WhatWeDoItem>? whatWeDoItems { get; set; }
        public List<OurStory>? OurStoryList { get; set; }
        public ContactInfo? ContactInfo { get; set; }

    }
}
