using EchoBot.Controllers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class ProcessDegreeCourseIntents
    {
        private ILogger<EchoBot> _logger;
        private DegreeCourseController degreeCourseController;

        public ProcessDegreeCourseIntents(ILogger<EchoBot> logger)
        {
            this._logger = logger;

            degreeCourseController = new DegreeCourseController();
        }
    }
}
