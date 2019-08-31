using FAQBot.Model;
using FAQBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Controllers
{
    public class EventController
    {
        private List<Event> events;

        public EventController()
        {
            events = EventDataService.LoadUpcomingEvents();
        }
    }
}
