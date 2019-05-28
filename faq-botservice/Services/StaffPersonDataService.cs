using EchoBot.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EchoBot.Services
{
    public static class StaffPersonDataService
    {
        private const string URL_STAFF_PATH = "https://www.fh-bielefeld.de/personenverzeichnis.html";

        public static List<StaffPerson> LoadBasicStaffList()
        {
            var web = new HtmlWeb();
            var doc = web.Load(URL_STAFF_PATH);

            List<StaffPerson> staff = new List<StaffPerson>();
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='wrapper_person']"))
            {
                HtmlNode innerNode = node.SelectSingleNode(".//div[@class='person']");

                StaffPerson staffPerson = new StaffPerson();

                HtmlNode nameNode = innerNode.SelectSingleNode(".//div[@class='name']");
                var nameString = nameNode.InnerHtml.ToString();
                staffPerson.Link = Regex.Match(nameString, @"href=(.+)><span>", RegexOptions.Singleline)
                    .Groups[1].Value
                    .Replace("\"", "");
                staffPerson.Name = Regex.Match(nameString, @"</span>(.+)</a>", RegexOptions.Singleline)
                    .Groups[1].Value;

                HtmlNode emailNode = innerNode.SelectSingleNode(".//div[@class='email']");
                var emailString = emailNode.InnerHtml.ToString();
                staffPerson.Phonenumber = Regex.Match(emailString, @"<div>(.+)</div>", RegexOptions.Singleline)
                    .Groups[1].Value;
                staffPerson.Email = Regex.Match(emailString, @"bielefeld.de(.+)</a>", RegexOptions.Singleline)
                    .Groups[1].Value
                    .Replace("\"", "")
                    .Replace(">", "");

                staff.Add(staffPerson);
            }

            return staff;
        }

        public static StaffPerson LoadStaffPersonInformation(StaffPerson sp)
        {
            //NOT IMPLEMENTET YET
            return sp;
        }
    }
}
