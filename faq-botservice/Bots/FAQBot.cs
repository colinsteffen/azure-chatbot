// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using FAQBot.Helper;
using Microsoft.Bot.Builder.Dialogs;

namespace FAQBot.Bots
{
    public class FAQBot<T> : ActivityHandler where T : Dialog
    {
        private ProcessStaffInformationIntents processStaffInformationIntents;
        private ProcessDegreeCourseIntents processDegreeCourseIntents;
        private ProcessEventIntents processEventIntents;

        protected readonly Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly ILogger Logger;
        private IBotServices _botServices;

        public FAQBot(IBotServices botServices, ConversationState conversationState, UserState userState, T dialog, ILogger<FAQBot<T>> logger)
        {
            Logger = logger;
            _botServices = botServices;
            this.Dialog = dialog;
            ConversationState = conversationState;
            UserState = userState;

            processStaffInformationIntents = new ProcessStaffInformationIntents(Logger);
            processDegreeCourseIntents = new ProcessDegreeCourseIntents(Logger, this.Dialog, this.ConversationState, this.UserState);
            processEventIntents = new ProcessEventIntents(Logger);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);

            if (StateHelper.DegreeCourseIntent.WaitingForInformation)
                await processDegreeCourseIntents.ProcessWaitingAsync(turnContext, cancellationToken);
            else if (IntentHelper.INTENT_DESCRIBE_FUNCTIONALITY_FAQ.Equals(turnContext.Activity.Text.ToUpper()))
                await turnContext.SendActivityAsync(MessageFactory.Text($"Stell einfach eine Frage an mich. Ich werde die Frage automatisch zuordnen und passend beantworten."), cancellationToken);
            else if (IntentHelper.INTENT_DESCRIBE_FUNCTIONALITY_PERSONAL_DIRECTORY.Equals(turnContext.Activity.Text.ToUpper()))
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich beantworte dir gerne Fragen zu dem Personal der FH Bielefeld. " +
                    $"Schreib einfach eine Frage zu den folgenden Inhalten:" +
                    $"\n\nE-Mail\nTelefonnummer\nFachbereich\nRaum\nSprechzeiten\n\n mit dem Namen des Mitarbeiters."), cancellationToken);
            else if (IntentHelper.INTENT_DESCRIBE_FUNCTIONALITY_COURSES.Equals(turnContext.Activity.Text.ToUpper()))
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich beantworte dir gerne Fragen zu Studiengängen und dazugehörigen Modulen. " +
                    $"\nSchreib einfach eine Frage zu Studiengängen und Modulen der Fachhochschule Bielefeld" +
                    $"\nDies kann z. B. eine Liste mit allen Studiengängen aus einem Fachbereich sein oder der Inhalt eines Moduls."), cancellationToken);
            else
            {
                var recognizerResult = await _botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);
                var topIntent = recognizerResult.GetTopScoringIntent();

                await DispatchToTopIntentAsync(turnContext, topIntent.intent, recognizerResult, cancellationToken);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Willkommen zum Bot der FH Bielefeld!\n\n" +
                        $"Der Bot beantwortet dir zu folgenden Themen Fragen:" +
                        $"\n\n-FAQ Fragen" +
                        $"\n-Informationen aus dem Personenverzeichnis" +
                        $"\n-Informationen zu Studiengängen sowie zugehörifen Modulen" +
                        $"\n\n\n\nWenn du Fragen zu dem Vorgehen hast schreibe 'FAQ', 'Personenverzeichnis' oder 'Studiengänge' um weitere Informationen zu erhalten."), cancellationToken);
            }
        }

        //Gets the best Intent from the connectedt Dispatch
        private async Task DispatchToTopIntentAsync(ITurnContext turnContext, string intent, RecognizerResult recognizerResult, CancellationToken cancellationToken)
        {
            switch (intent)
            {
                case IntentHelper.INTENT_STAFF:
                    await ProcessGetStaffInformationAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                    break;
                case IntentHelper.INTENT_QNA:
                    await ProcessFHBielefeldQnAAsync(turnContext, cancellationToken);
                    break;
                case IntentHelper.INTENT_COURSES:
                    await ProcessGetCourseInformationAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                    break;
                case IntentHelper.INTENT_EVENTS:
                    await ProcessGetEventInformationAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                    break;
                default:
                    Logger.LogInformation($"Dispatch unrecognized intent: {intent}.");
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Dispatch unrecognized intent: {intent}."), cancellationToken);
                    break;
            }
        }

        //Methods for processing the explicit Intents

        private async Task ProcessFHBielefeldQnAAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ProcessSampleQnAAsync");

            var results = await _botServices.FHBielefeldQnA.GetAnswersAsync(turnContext);
            if (results.Any())
                await turnContext.SendActivityAsync(MessageFactory.Text(results.First().Answer), cancellationToken);
            else
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system."), cancellationToken);
        }

        private async Task ProcessGetStaffInformationAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ProcessGetStaffInformationAsync");

            var result = luisResult.ConnectedServiceResult;
            var topIntent = result.TopScoringIntent.Intent;

            if (IntentHelper.INTENT_STAFF_EMAIL.Equals(topIntent))
                await processStaffInformationIntents.ProcessIntentGetEmailAsync(turnContext, luisResult, cancellationToken);
            else if(IntentHelper.INTENT_STAFF_PHONENUMBER.Equals(topIntent))
                await processStaffInformationIntents.ProcessIntentGetPhonenumberAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_STAFF_OFFICE_HOURS.Equals(topIntent))
                await processStaffInformationIntents.ProcessIntentGetOfficeHoursAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_STAFF_DEPARTMENT.Equals(topIntent))
                await processStaffInformationIntents.ProcessIntentGetDepartmentAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_STAFF_ROOM.Equals(topIntent))
                await processStaffInformationIntents.ProcessIntentGetRoomAsync(turnContext, luisResult, cancellationToken);
        }

        private async Task ProcessGetEventInformationAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ProcessGetEventInformationAsync");

            var result = luisResult.ConnectedServiceResult;
            var topIntent = result.TopScoringIntent.Intent;

            //TODO
        }

        private async Task ProcessGetCourseInformationAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ProcessGetCourseInformationAsync");

            var result = luisResult.ConnectedServiceResult;
            var topIntent = result.TopScoringIntent.Intent;

            if (IntentHelper.INTENT_COURSES_COURSES_ALL.Equals(topIntent))
                await processDegreeCourseIntents.ProcessIntentGetCoursesAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_COURSES_MODULE_COMMISSIONER.Equals(topIntent))
                await processDegreeCourseIntents.ProcessIntentGetModuleCommissionerAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_COURSES_MODULE_CONTENT.Equals(topIntent))
                await processDegreeCourseIntents.ProcessIntentGetModuleContentAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_COURSES_MODULE_INFORMATION.Equals(topIntent))
                await processDegreeCourseIntents.ProcessIntentGetModuleInformationAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_COURSES_MODULE_LANGUAGE.Equals(topIntent))
                await processDegreeCourseIntents.ProcessIntentGetModuleLanguageAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_COURSES_METHOD_OF_EXAMINATION.Equals(topIntent))
                await processDegreeCourseIntents.ProcessIntentGetModuleMethodOfExaminationAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_COURSES_COURSE.Equals(topIntent))
                await processDegreeCourseIntents.ProcessIntentGetCourseAsync(turnContext, luisResult, cancellationToken);
            else if (IntentHelper.INTENT_COURSES_MODULES.Equals(topIntent))
                await processDegreeCourseIntents.ProcessIntentGetModulesAsync(turnContext, luisResult, cancellationToken);
        }
    }
}
