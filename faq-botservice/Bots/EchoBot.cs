// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using Microsoft.Bot.Builder.AI.QnA;
using EchoBot;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using EchoBot.Model;
using EchoBot.Controllers;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private const string INTENT_STAFF_EMAIL = "getEmail";
        private const string INTENT_STAFF_PHONENUMBER = "getPhonenumber";
        private const string INTENT_STAFF_ROOM = "getRoom";

        private const string ENTITY_PERSON_NACHNAME = "personNachname";

        public StaffInformationController staffInformationController;

        private ILogger<EchoBot> _logger;
        private IBotServices _botServices;

        public EchoBot(IBotServices botServices, ILogger<EchoBot> logger)
        {
            _logger = logger;
            _botServices = botServices;

            staffInformationController = new StaffInformationController();
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
            var recognizerResult = await _botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);

            // Top intent tell us which cognitive service to use.
            var topIntent = recognizerResult.GetTopScoringIntent();

            // Next, we call the dispatcher with the top intent.
            await DispatchToTopIntentAsync(turnContext, topIntent.intent, recognizerResult, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Willkommen zum Bot der FH Bielefeld!"), cancellationToken);
                }
            }
        }

        private async Task DispatchToTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, RecognizerResult recognizerResult, CancellationToken cancellationToken)
        {
            switch (intent)
            {
                case "getEmail":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"GetEmail"), cancellationToken);
                    await ProcessGetStaffInformationAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                    break;
                case "getQnA":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"getQnA"), cancellationToken);
                    await ProcessFHBielefeldQnAAsync(turnContext, cancellationToken);
                    break;
                default:
                    _logger.LogInformation($"Dispatch unrecognized intent: {intent}.");
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Dispatch unrecognized intent: {intent}."), cancellationToken);
                    break;
            }
        }

        private async Task ProcessFHBielefeldQnAAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessSampleQnAAsync");

            var results = await _botServices.FHBielefeldQnA.GetAnswersAsync(turnContext);
            if (results.Any())
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(results.First().Answer), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system."), cancellationToken);
            }
        }

        private async Task ProcessGetStaffInformationAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessGetStaffInformationAsync");

            var result = luisResult.ConnectedServiceResult;
            var topIntent = result.TopScoringIntent.Intent;
            await turnContext.SendActivityAsync(MessageFactory.Text($"GetStaffInformation top intent {topIntent}."), cancellationToken);
            if (luisResult.Entities.Count > 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"GetStaffInformation entities were found in the message:\n\n{string.Join("\n\n", result.Entities.Select(i => i.Entity))}"), cancellationToken);
            }

            if (INTENT_STAFF_EMAIL.Equals(topIntent))
                await ProcessIntentGetEmailAsync(turnContext, luisResult, cancellationToken); 
        }

        private async Task ProcessIntentGetEmailAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            if(luisResult.ConnectedServiceResult.Entities.Count() > 0)
            {
                EntityModel em = luisResult.ConnectedServiceResult.Entities[0];
                string email = staffInformationController.GetEmailFromStaffPerson(em.Entity);

                if(string.IsNullOrEmpty(email) || !ENTITY_PERSON_NACHNAME.Equals(em.Type))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine passende Person zu der Anfrage finden."), cancellationToken);
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Die Email von {em.Entity} ist {email}."), cancellationToken);
            }
            else
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich brauche einen Namen zu dem ich eine Email finden soll."), cancellationToken);
        }
    }
}
