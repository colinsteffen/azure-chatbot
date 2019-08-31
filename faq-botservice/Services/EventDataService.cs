using FAQBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Services
{
    public static class EventDataService
    {
        private const string URL_EVENTS = "https://www.fh-bielefeld.de/hochschule/veranstaltungen/";

        public static List<Event> LoadUpcomingEvents()
        {
            //TODO NOT IMPLEMENTET YET

            return new List<Event>();
        }
    }
}
