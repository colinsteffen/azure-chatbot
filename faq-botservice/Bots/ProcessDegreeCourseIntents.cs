using EchoBot.Controllers;
using EchoBot.Dialogs;
using EchoBot.Helper;
using EchoBot.Model;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class ProcessDegreeCourseIntents
    {
        private ILogger<EchoBot> _logger;
        private DegreeCourseController degreeCourseController;

        public bool WaitingForInformation { get; private set; }
        private string WaitingMethod;
        private string WaitingVariable;

        private int LastModuleId;
        private int LastDegreeCourseId;
        private int LastDepartmentId;
        private string LastDegreeLevel;

        public ProcessDegreeCourseIntents(ILogger<EchoBot> logger)
        {
            this._logger = logger;
            WaitingForInformation = false;

            LastModuleId = -1;
            LastDegreeCourseId = -1;

            degreeCourseController = new DegreeCourseController();
        }

        //Register all Methods that can wait for further Information
        public async Task ProcessWaitingAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessWaitingAsync");

            string message = turnContext.Activity.Text;

            if (WaitingMethod.Equals("")) ; //Do Something
        }

        //Process Intents

        public async Task ProcessIntentGetCoursesAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetCoursesAsync");

            if (luisResult != null) {
                if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
                {
                    foreach (EntityModel em in luisResult.ConnectedServiceResult.Entities)
                    {
                        if (EntityHelper.ENTITY_DEGREE.Equals(em.Type))
                            LastDegreeLevel = em.Entity;
                        else if (EntityHelper.ENTITY_DEPARTMENT.Equals(em.Type))
                            LastDepartmentId = degreeCourseController.GetDepartmentIdFromName(em.Entity);
                    }

                    List<DegreeCourse> degreeCoursesFiltered = degreeCourseController.GetFilteredDegreeCourses(LastDepartmentId, LastDegreeLevel);
                    await turnContext.SendActivityAsync(MessageFactory.Text(DegreeCourseIntentDialogs.DialogGetDegreeCourses(degreeCoursesFiltered)), cancellationToken);
                }
            }
        }

        public async Task ProcessIntentGetModuleCommissionerAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleCommissionerAsync");

            //TODO
        }

        public async Task ProcessIntentGetModuleContentAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleContentAsync");

            //TODO
        }

        public async Task ProcessIntentGetModuleInformationAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleInformationAsync");

            //TODO
        }

        public async Task ProcessIntentGetModuleLanguageAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleLanguageAsync");

            //TODO
        }

        public async Task ProcessIntentGetModuleMethodOfExaminationAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleCommissionerAsync");

            //TODO
        }
    }
}
