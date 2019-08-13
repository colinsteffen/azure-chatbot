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
using EchoBot.Helper;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private ProcessStaffInformationIntents processStaffInformationIntents;
        private ProcessDegreeCourseIntents processDegreeCourseIntents;
        private ProcessEventIntents processEventIntents;

        private ILogger<EchoBot> _logger;
        private IBotServices _botServices;

        public EchoBot(IBotServices botServices, ILogger<EchoBot> logger)
        {
            _logger = logger;
            _botServices = botServices;

            processStaffInformationIntents = new ProcessStaffInformationIntents(_logger);
            processDegreeCourseIntents = new ProcessDegreeCourseIntents(_logger);
            processEventIntents = new ProcessEventIntents(_logger);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (processDegreeCourseIntents.WaitingForInformation)
                await processDegreeCourseIntents.ProcessWaitingAsync(turnContext, cancellationToken);
            if (IntentHelper.INTENT_DESCRIBE_FUNCTIONALITY_FAQ.Equals(turnContext.Activity.Text.ToUpper()))
                await turnContext.SendActivityAsync(MessageFactory.Text($"Stell einfach eine Frage an mich. Ich werde die Frage automatisch zuordnen und passend beantworten."), cancellationToken);
            else if (IntentHelper.INTENT_DESCRIBE_FUNCTIONALITY_PERSONAL_DIRECTORY.Equals(turnContext.Activity.Text.ToUpper()))
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich beantworte dir gerne Fragen zu dem Personal der FH Bielefeld. Schreib einfach eine Frage zu den folgenden Inhalten:\n\nE-Mail\nTelefonnummer\nFachbereich\nRaum\nSprechzeiten\n\n mit dem Namen des Mitarbeiters."), cancellationToken);
            else
            {
                // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
                var recognizerResult = await _botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);

                // Top intent tell us which cognitive service to use.
                var topIntent = recognizerResult.GetTopScoringIntent();

                // Next, we call the dispatcher with the top intent.
                await DispatchToTopIntentAsync(turnContext, topIntent.intent, recognizerResult, cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Willkommen zum Bot der FH Bielefeld!\n\nDer Bot beantwortet dir zu folgenden Themen Fragen:\n\n-FAQ Fragen\n-Informationen aus dem Personenverzeichnis\n\n\n\nWenn du Fragen zu dem Vorgehen hast schreibe 'FAQ' oder 'Personenverzeichnis' um weitere Informationen zu erhalten."), cancellationToken);
            }
        }

        //Gets the best Intent from the connectedt Dispatch
        private async Task DispatchToTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, RecognizerResult recognizerResult, CancellationToken cancellationToken)
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
                    _logger.LogInformation($"Dispatch unrecognized intent: {intent}.");
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Dispatch unrecognized intent: {intent}."), cancellationToken);
                    break;
            }
        }

        //Methods for processing the explicit Intents

        private async Task ProcessFHBielefeldQnAAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessSampleQnAAsync");

            var results = await _botServices.FHBielefeldQnA.GetAnswersAsync(turnContext);
            if (results.Any())
                await turnContext.SendActivityAsync(MessageFactory.Text(results.First().Answer), cancellationToken);
            else
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system."), cancellationToken);
        }

        private async Task ProcessGetStaffInformationAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessGetStaffInformationAsync");

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

        private async Task ProcessGetEventInformationAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessGetEventInformationAsync");

            var result = luisResult.ConnectedServiceResult;
            var topIntent = result.TopScoringIntent.Intent;

            //TODO
        }

        private async Task ProcessGetCourseInformationAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessGetCourseInformationAsync");

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
        }
    }
}
