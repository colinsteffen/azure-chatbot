using EchoBot.Controllers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class ProcessEventIntents
    {
        private ILogger<EchoBot> _logger;
        private EventController eventController;

        public ProcessEventIntents(ILogger<EchoBot> logger)
        {
            this._logger = logger;

            eventController = new EventController();
        }
    }
}
