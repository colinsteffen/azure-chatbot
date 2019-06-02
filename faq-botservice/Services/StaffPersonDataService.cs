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
        private const string URL_FH = "https://www.fh-bielefeld.de";
        private const string URL_STAFF = "/personenverzeichnis.html";

        public static List<StaffPerson> LoadBasicStaffList()
        {
            var web = new HtmlWeb();
            var doc = web.Load(URL_FH + URL_STAFF);

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

        public static void LoadStaffPersonInformation(StaffPerson sp)
        {
            var web = new HtmlWeb();
            var staffLink = sp.Link.Replace(".html", "");
            var doc = web.Load(URL_FH + staffLink);

            HtmlNode node;

            //Office Hours
            node = doc.DocumentNode.SelectSingleNode("//div[@class='officehours']");
            if (node != null)
            {
                var sprechzeitenString = node.InnerHtml.ToString();
                sp.OfficeHours = Regex.Match(sprechzeitenString, @"<div><p>(.+)</p></div>", RegexOptions.Singleline)
                    .Groups[1].Value;
            }

            node = doc.DocumentNode.SelectSingleNode("//div[@class='institution']");
            if (node != null)
            {
                var institutionString = node.InnerHtml.ToString();

                //Department
                var departmentString = institutionString.Substring(12);
                if (!string.IsNullOrEmpty(departmentString))
                    sp.Department = departmentString;
            }

            node = doc.DocumentNode.SelectSingleNode("//div[@class='kontakt_rechts']");
            if (node != null)
            {
                //Room
                var contactString = node.InnerHtml.ToString();

                var roomString = Regex.Match(contactString, @"Raum (.+)<br>Telefon", RegexOptions.Singleline)
                    .Groups[1].Value;
                if (!string.IsNullOrEmpty(roomString))
                    sp.Room = roomString;
            }

            //Lectures
            //NOT IMPLEMENTET YET

            //Publications
            //NOT IMPLEMENTET YET
        }
    }
}
