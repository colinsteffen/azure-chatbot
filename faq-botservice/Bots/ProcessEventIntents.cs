using FAQBot.Controllers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Bots
{
    public class ProcessEventIntents
    {
        private ILogger _logger;
        private EventController eventController;

        public ProcessEventIntents(ILogger logger)
        {
            this._logger = logger;

            eventController = new EventController();
        }
    }
}
