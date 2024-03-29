﻿using FAQBot.Model;
using FAQBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Controllers
{
    public class StaffInformationController
    {
        private List<StaffPerson> staff;

        public StaffInformationController()
        {
            staff = StaffPersonDataService.LoadBasicStaffList();
        }

        public string GetEmailFromStaffPerson(string name)
        {
            List<StaffPerson> staff = GetStaffPersonFromName(name);

            if (staff.Count == 0) return "";

            return staff[0].Email;
        }

        public string GetNameFromStaffPerson(string name)
        {
            List<StaffPerson> staff = GetStaffPersonFromName(name);

            if (staff[0] == null) return "";

            return staff[0].Name;
        }

        public string GetPhonenumberFromStaffPerson(string name)
        {
            List<StaffPerson> staff = GetStaffPersonFromName(name);

            if (staff[0] == null) return "";

            return staff[0].Phonenumber;
        }

        public string GetRoomnumberFromStaffPerson(string name)
        {
            List<StaffPerson> staff = GetStaffPersonFromName(name);

            if (staff[0] == null) return "";

            staff[0].CheckExtendedInformation();

            return staff[0].Room;
        }

        public string GetDepartmentFromStaffPerson(string name)
        {
            List<StaffPerson> staff = GetStaffPersonFromName(name);

            if (staff[0] == null) return "";

            staff[0].CheckExtendedInformation();

            return staff[0].Department;
        }

        public string GetOfficeHoursFromStaffPerson(string name)
        {
            List<StaffPerson> staff = GetStaffPersonFromName(name);

            if (staff[0] == null) return "";

            staff[0].CheckExtendedInformation();

            return staff[0].OfficeHours;
        }

        private List<StaffPerson> GetStaffPersonFromName(string name)
        {
            List<StaffPerson> tempStaff = new List<StaffPerson>();
            tempStaff.AddRange(staff.FindAll(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)));

            return tempStaff;
        }
    }
}
