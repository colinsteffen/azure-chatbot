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

            if (StateHelper.DegreeCourseIntent.WaitingMethod.Equals("ProcessIntentGetModuleCommissionerAsync"))
                await ProcessIntentGetModuleCommissionerAsync(turnContext, null, cancellationToken);
            else if(StateHelper.DegreeCourseIntent.WaitingMethod.Equals("ProcessIntentGetModuleContentAsync"))
                await ProcessIntentGetModuleContentAsync(turnContext, null, cancellationToken);
            else if (StateHelper.DegreeCourseIntent.WaitingMethod.Equals("ProcessIntentGetModuleInformationAsync"))
                await ProcessIntentGetModuleInformationAsync(turnContext, null, cancellationToken);
            else if (StateHelper.DegreeCourseIntent.WaitingMethod.Equals("ProcessIntentGetModuleLanguageAsync"))
                await ProcessIntentGetModuleLanguageAsync(turnContext, null, cancellationToken);
            else if (StateHelper.DegreeCourseIntent.WaitingMethod.Equals("ProcessIntentGetCourseAsync"))
                await ProcessIntentGetCourseAsync(turnContext, null, cancellationToken);
            else if (StateHelper.DegreeCourseIntent.WaitingMethod.Equals("ProcessIntentGetModulesAsync"))
                await ProcessIntentGetModulesAsync(turnContext, null, cancellationToken);
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

        public async Task ProcessIntentGetCourseAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetCourseAsync");

            if (StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                if(StateHelper.DegreeCourseIntent.WaitingVariable.Equals("StudyModel")) StateHelper.DegreeCourseIntent.LastStudyModel = turnContext.Activity.Text;
                else if(StateHelper.DegreeCourseIntent.WaitingVariable.Equals("DegreeLevel")) StateHelper.DegreeCourseIntent.LastDegreeLevel = turnContext.Activity.Text;

                await SetToNotWaiting();
            }

            if (luisResult != null)
            {
                if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
                {
                    foreach (EntityModel em in luisResult.ConnectedServiceResult.Entities)
                    {
                        if (EntityHelper.ENTITY_STUDY_MODEL.Equals(em.Type)) StateHelper.DegreeCourseIntent.LastStudyModel = em.Entity;
                        if (EntityHelper.ENTITY_DEGREE.Equals(em.Type)) StateHelper.DegreeCourseIntent.LastDegreeLevel = em.Entity;
                        if (EntityHelper.ENTITY_COURSE.Equals(em.Type)) StateHelper.DegreeCourseIntent.LastDegreeCourseTitle = em.Entity;
                    }
                }
            }

            List<int> dcIds = degreeCourseController.GetDegreeCourseIdsFromName(StateHelper.DegreeCourseIntent.LastDegreeCourseTitle, StateHelper.DegreeCourseIntent.LastStudyModel, StateHelper.DegreeCourseIntent.LastDegreeLevel);
            if (dcIds.Count > 1)
            {
                if (string.IsNullOrEmpty(StateHelper.DegreeCourseIntent.LastDegreeLevel))
                {
                    StateHelper.DegreeCourseIntent.WaitingForInformation = true;
                    StateHelper.DegreeCourseIntent.WaitingVariable = "DegreeLevel";
                    StateHelper.DegreeCourseIntent.WaitingMethod = "ProcessIntentGetCourseAsync";

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Nach welchem Studienabschluss suchen Sie? \n\n (Master oder Bachelor)"), cancellationToken);
                }
                else if (string.IsNullOrEmpty(StateHelper.DegreeCourseIntent.LastStudyModel))
                {
                    StateHelper.DegreeCourseIntent.WaitingForInformation = true;
                    StateHelper.DegreeCourseIntent.WaitingVariable = "StudyModel";
                    StateHelper.DegreeCourseIntent.WaitingMethod = "ProcessIntentGetCourseAsync";

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Nach welchem Studienmodell suchen Sie? \n\n (Vollzeitstudiengang, praxisintegrierter Studiengang oder Verbundstudiengang (berufsbegleitend))"), cancellationToken);
                }
            } 
            else StateHelper.DegreeCourseIntent.LastDegreeCourseId = dcIds.First();

            if (!StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                DegreeCourse searchedDegreeCourse = degreeCourseController.GetDegreeCourse(StateHelper.DegreeCourseIntent.LastDegreeCourseId);
                string text = $"Ich konnte die folgenden Informationen zu dem Studiengang {searchedDegreeCourse.Title} finden:";
                text += $"\n- Studienmodell: {searchedDegreeCourse.StudyModel}";
                text += $"\n- Studiendauer: {searchedDegreeCourse.DurationOfStudy}";
                text += $"\n- Abschluss: {searchedDegreeCourse.DegreeLevel}";
                if(searchedDegreeCourse.NumerusClausus) text += $"\n- Numerus Clausus: ja";
                else text += $"\n- Numerus Clausus: nein";
                await turnContext.SendActivityAsync(MessageFactory.Text(text), cancellationToken);
            }
        }

        public async Task ProcessIntentGetModulesAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModulesAsync");

            if (StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                if (StateHelper.DegreeCourseIntent.WaitingVariable.Equals("StudyModel")) StateHelper.DegreeCourseIntent.LastStudyModel = turnContext.Activity.Text;
                else if (StateHelper.DegreeCourseIntent.WaitingVariable.Equals("DegreeLevel")) StateHelper.DegreeCourseIntent.LastDegreeLevel = turnContext.Activity.Text;

                await SetToNotWaiting();
            }

            if (luisResult != null)
            {
                if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
                {
                    foreach (EntityModel em in luisResult.ConnectedServiceResult.Entities)
                    {
                        if (EntityHelper.ENTITY_STUDY_MODEL.Equals(em.Type)) StateHelper.DegreeCourseIntent.LastStudyModel = em.Entity;
                        if (EntityHelper.ENTITY_DEGREE.Equals(em.Type)) StateHelper.DegreeCourseIntent.LastDegreeLevel = em.Entity;
                        if (EntityHelper.ENTITY_COURSE.Equals(em.Type)) StateHelper.DegreeCourseIntent.LastDegreeCourseTitle = em.Entity;
                    }
                }
            }

            List<int> dcIds = degreeCourseController.GetDegreeCourseIdsFromName(StateHelper.DegreeCourseIntent.LastDegreeCourseTitle, StateHelper.DegreeCourseIntent.LastStudyModel, StateHelper.DegreeCourseIntent.LastDegreeLevel);
            if (dcIds.Count > 1)
            {
                if (string.IsNullOrEmpty(StateHelper.DegreeCourseIntent.LastDegreeLevel))
                {
                    StateHelper.DegreeCourseIntent.WaitingForInformation = true;
                    StateHelper.DegreeCourseIntent.WaitingVariable = "DegreeLevel";
                    StateHelper.DegreeCourseIntent.WaitingMethod = "ProcessIntentGetModulesAsync";

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Nach welchem Studienabschluss suchen Sie? \n\n (Master oder Bachelor)"), cancellationToken);
                }
                else if (string.IsNullOrEmpty(StateHelper.DegreeCourseIntent.LastStudyModel))
                {
                    StateHelper.DegreeCourseIntent.WaitingForInformation = true;
                    StateHelper.DegreeCourseIntent.WaitingVariable = "StudyModel";
                    StateHelper.DegreeCourseIntent.WaitingMethod = "ProcessIntentGetModulesAsync";

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Nach welchem Studienmodell suchen Sie? \n\n (Vollzeitstudiengang, praxisintegrierter Studiengang oder Verbundstudiengang (berufsbegleitend))"), cancellationToken);
                }
            }
            else StateHelper.DegreeCourseIntent.LastDegreeCourseId = dcIds.First();

            if (!StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                DegreeCourse searchedDegreeCourse = degreeCourseController.GetDegreeCourse(StateHelper.DegreeCourseIntent.LastDegreeCourseId);

                string text = $"Ich konnte die folgenden Module zu dem Studiengang {searchedDegreeCourse.Title} finden:";
                foreach(string id in searchedDegreeCourse.ModuleIds)
                {
                    Module m = degreeCourseController.getModule(id);
                    text += $"\n-{m.Title}";
                }
                await turnContext.SendActivityAsync(MessageFactory.Text(text), cancellationToken);
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
                            StateHelper.DegreeCourseIntent.LastModuleId = degreeCourseController.getModuleIdFromName(TextFormatHelper.RemoveWhitespaceBeforeAfterHyphen(em.Entity));
                    }
                }

                if (string.IsNullOrEmpty(StateHelper.DegreeCourseIntent.LastModuleId))
                {
                    StateHelper.DegreeCourseIntent.WaitingForInformation = true;
                    StateHelper.DegreeCourseIntent.WaitingVariable = "ModuleId";
                    StateHelper.DegreeCourseIntent.WaitingMethod = "ProcessIntentGetModuleContentAsync";

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Für welches Modul soll ich den Inhalt suchen?"), cancellationToken);
                }
            }

            if (!StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                Module searchedModule = degreeCourseController.getModule(StateHelper.DegreeCourseIntent.LastModuleId);
                string text = $"Das Modul {searchedModule.Title} hat die folgenden Inhalte:";
                foreach (string s in searchedModule.Content)
                    text += $"\n-{s}";
                await turnContext.SendActivityAsync(MessageFactory.Text(text), cancellationToken);
            }
        }

        public async Task ProcessIntentGetModuleInformationAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleInformationAsync");

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
                            StateHelper.DegreeCourseIntent.LastModuleId = degreeCourseController.getModuleIdFromName(TextFormatHelper.RemoveWhitespaceBeforeAfterHyphen(em.Entity));
                    }
                }

                if (string.IsNullOrEmpty(StateHelper.DegreeCourseIntent.LastModuleId))
                {
                    StateHelper.DegreeCourseIntent.WaitingForInformation = true;
                    StateHelper.DegreeCourseIntent.WaitingVariable = "ModuleId";
                    StateHelper.DegreeCourseIntent.WaitingMethod = "ProcessIntentGetModuleInformationAsync";

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Für welches Modul soll ich Informationen suchen?"), cancellationToken);
                }
            }

            if (!StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                Module searchedModule = degreeCourseController.getModule(StateHelper.DegreeCourseIntent.LastModuleId);
                string text = $"Ich habe folgende Informationen zu dem Modul {searchedModule.Title} gefunden:";
                text += $"\n-Beauftragter: {searchedModule.Commissioner}";
                text += $"\n-Abschluss: {searchedModule.DegreeLevel}";
                text += $"\n-Credit Points: {searchedModule.CreditPoints}";
                text += $"\n-Inhalt:";
                foreach (string s in searchedModule.Content)
                    text += $"\n-{s}";
                await turnContext.SendActivityAsync(MessageFactory.Text(text), cancellationToken);
            }
        }

        public async Task ProcessIntentGetModuleLanguageAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleLanguageAsync");

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
                            StateHelper.DegreeCourseIntent.LastModuleId = degreeCourseController.getModuleIdFromName(TextFormatHelper.RemoveWhitespaceBeforeAfterHyphen(em.Entity));
                    }
                }

                if (string.IsNullOrEmpty(StateHelper.DegreeCourseIntent.LastModuleId))
                {
                    StateHelper.DegreeCourseIntent.WaitingForInformation = true;
                    StateHelper.DegreeCourseIntent.WaitingVariable = "ModuleId";
                    StateHelper.DegreeCourseIntent.WaitingMethod = "ProcessIntentGetModuleLanguageAsync";

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Für welches Modul soll ich die Sprache suchen?"), cancellationToken);
                }
            }

            if (!StateHelper.DegreeCourseIntent.WaitingForInformation)
            {
                Module searchedModule = degreeCourseController.getModule(StateHelper.DegreeCourseIntent.LastModuleId);
                await turnContext.SendActivityAsync(MessageFactory.Text($"Das Modul {searchedModule.Title} wird in der Sprache {searchedModule.Language} gehalten."), cancellationToken);
            }
        }

        public async Task ProcessIntentGetModuleMethodOfExaminationAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetModuleCommissionerAsync");

            //TODO
        }
    }
}
