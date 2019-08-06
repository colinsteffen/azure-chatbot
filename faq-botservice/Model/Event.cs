using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Model
{
    public class Event
    {
        public String Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PlaceOfStudy Place { get; set; }
        public String TargetGroup { get; set; }
        public String Organizer { get; set; }
        public String Registration { get; set; }
    }
}
