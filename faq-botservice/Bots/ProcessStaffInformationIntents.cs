using EchoBot.Controllers;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class ProcessStaffInformationIntents
    {
        private const string ENTITY_PERSON_NACHNAME = "personNachname";

        private StaffInformationController staffInformationController;

        public ProcessStaffInformationIntents()
        {
            staffInformationController = new StaffInformationController();
        }

        public async Task ProcessIntentGetEmailAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
            {
                EntityModel em = luisResult.ConnectedServiceResult.Entities[0];
                string email = staffInformationController.GetEmailFromStaffPerson(em.Entity);

                if (string.IsNullOrEmpty(email) || !ENTITY_PERSON_NACHNAME.Equals(em.Type))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine passende Person zu der Anfrage finden."), cancellationToken);
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Die Email von {em.Entity} ist {email}."), cancellationToken);
            }
            else
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich brauche einen Namen zu dem ich eine Email finden soll."), cancellationToken);
        }

        public async Task ProcessIntentGetPhonenumberAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
            {
                EntityModel em = luisResult.ConnectedServiceResult.Entities[0];
                string phonenumber = staffInformationController.GetPhonenumberFromStaffPerson(em.Entity);

                if (string.IsNullOrEmpty(phonenumber) || !ENTITY_PERSON_NACHNAME.Equals(em.Type))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine passende Person zu der Anfrage finden."), cancellationToken);
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Die Telefonnummer von {em.Entity} ist {phonenumber}."), cancellationToken);
            }
            else
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich brauche einen Namen zu dem ich eine Telefonnummer finden soll."), cancellationToken);
        }
    }
}
