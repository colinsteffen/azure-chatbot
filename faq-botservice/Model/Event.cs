using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Model
{
    public class Event
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PlaceOfStudy Place { get; set; }
        public string TargetGroup { get; set; }
        public string Organizer { get; set; }
        public string Registration { get; set; }
    }
}
