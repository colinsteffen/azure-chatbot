using EchoBot.Model;
using EchoBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Controllers
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
