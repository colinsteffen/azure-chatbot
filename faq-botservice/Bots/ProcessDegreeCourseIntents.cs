using EchoBot.Controllers;
using EchoBot.Dialogs;
using EchoBot.Helper;
using EchoBot.Model;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
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
        private ILogger _logger;
        private DegreeCourseController degreeCourseController;
        protected readonly Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;

        public ProcessDegreeCourseIntents(ILogger logger, Dialog dialog, BotState conversationState, BotState userState)
        {
            this._logger = logger;
            this.Dialog = dialog;
            this.ConversationState = conversationState;
            this.UserState = userState;

            degreeCourseController = new DegreeCourseController();
        }

        //Register all Methods that can wait for further Information
        public async Task ProcessWaitingAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessWaitingAsync");

            string message = turnContext.Activity.Text;

            if (StateHelper.DegreeCourseIntent.WaitingMethod.Equals("ProcessIntentGetModuleCommissionerAsync"))
                await ProcessIntentGetModuleCommissionerAsync(turnContext, null, cancellationToken);
        }

        public async Task SetToNotWaiting()
        {
            StateHelper.DegreeCourseIntent.WaitingForInformation = false;
            StateHelper.DegreeCourseIntent.WaitingVariable = "";
            StateHelper.DegreeCourseIntent.WaitingMethod = "";
        }

        //Process Intents

        public async Task ProcessIntentGetCoursesAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetCoursesAsync");

            if (luisResult != null) {
                if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
                {
                    foreach (EntityModel em in luisResult.ConnectedServiceResult.Entities)
                    {
                        if (EntityHelper.ENTITY_DEGREE.Equals(em.Type))
                            StateHelper.DegreeCourseIntent.LastDegreeLevel = em.Entity;
                        else if (EntityHelper.ENTITY_DEPARTMENT.Equals(em.Type))
                            StateHelper.DegreeCourseIntent.LastDepartmentId = degreeCourseController.GetDepartmentIdFromName(em.Entity);
                    }

                    List<DegreeCourse> degreeCoursesFiltered = degreeCourseController.GetFilteredDegreeCourses(StateHelper.DegreeCourseIntent.LastDepartmentId, StateHelper.DegreeCourseIntent.LastDegreeLevel);
                    await turnContext.SendActivityAsync(MessageFactory.Text(DegreeCourseIntentDialogs.DialogGetDegreeCourses(degreeCoursesFiltered)), cancellationToken);
                }
            }
        }

        public async Task ProcessIntentGetModuleCommissionerAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleCommissionerAsync");
            if (StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                StateHelper.DegreeCourseIntent.LastModuleId = degreeCourseController.getModuleIdFromName(turnContext.Activity.Text);
                await SetToNotWaiting();
            }

            if (luisResult != null)
            {
                if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
                {
                    foreach (EntityModel em in luisResult.ConnectedServiceResult.Entities)
                    {
                        if (EntityHelper.ENTITY_MODULE.Equals(em.Type))
                            StateHelper.DegreeCourseIntent.LastModuleId = degreeCourseController.getModuleIdFromName(em.Entity);
                    }
                }

                if (string.IsNullOrEmpty(StateHelper.DegreeCourseIntent.LastModuleId))
                {
                    // Run the Dialog with the new message Activity.
                    //await this.Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken); //Todo 

                    StateHelper.DegreeCourseIntent.WaitingForInformation = true;
                    StateHelper.DegreeCourseIntent.WaitingVariable = "ModuleId";
                    StateHelper.DegreeCourseIntent.WaitingMethod = "ProcessIntentGetModuleCommissionerAsync";

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Für welches Modul soll ich den Modulbeauftragten suchen?"), cancellationToken);
                }
            }

            if(!StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                Module searchedModule = degreeCourseController.getModule(StateHelper.DegreeCourseIntent.LastModuleId);
                await turnContext.SendActivityAsync(MessageFactory.Text($"Das Modul {searchedModule.Title} wird von {searchedModule.Commissioner} betreut."), cancellationToken);
            }
        }

        public async Task ProcessIntentGetModuleContentAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleContentAsync");

            //TODO
        }

        public async Task ProcessIntentGetModuleInformationAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleInformationAsync");

            //TODO
        }

        public async Task ProcessIntentGetModuleLanguageAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleLanguageAsync");

            //TODO
        }

        public async Task ProcessIntentGetModuleMethodOfExaminationAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleCommissionerAsync");

            //TODO
        }
    }
}
