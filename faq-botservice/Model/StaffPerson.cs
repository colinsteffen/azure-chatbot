using FAQBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Model
{
    public class StaffPerson
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phonenumber { get; set; }
        public string Link { get; set; }
        public string OfficeHours { get; set; }
        public string Room { get; set; }
        public string Department { get; set; }
        public List<string> Courses { get; set; }
        public List<string> Publications { get; set; }

        public StaffPerson()
        {
            Courses = new List<string>();
            Publications = new List<string>();
        }

        public void CheckExtendedInformation()
        {
            if(string.IsNullOrEmpty(OfficeHours)
                && string.IsNullOrEmpty(OfficeHours)
                && string.IsNullOrEmpty(Room)
                && string.IsNullOrEmpty(Department)
                && Courses.Count == 0
                && Publications.Count == 0)
                StaffPersonDataService.LoadStaffPersonInformation(this);
        }
    }
}
