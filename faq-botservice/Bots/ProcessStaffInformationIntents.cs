using EchoBot.Controllers;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EchoBot.Helper;

namespace EchoBot.Bots
{
    public class ProcessStaffInformationIntents
    {
        private const string ENTITY_PERSON_NACHNAME = "personNachname";

        private ILogger _logger;
        private StaffInformationController staffInformationController;

        public ProcessStaffInformationIntents(ILogger logger)
        {
            this._logger = logger;

            staffInformationController = new StaffInformationController();
        }

        public async Task ProcessIntentGetEmailAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetEmailAsync");

            if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
            {
                EntityModel em = luisResult.ConnectedServiceResult.Entities[0];
                string entityPersonName = TextFormatHelper.RemoveWhitespaceBeforeAfterHyphen(em.Entity);
                string email = staffInformationController.GetEmailFromStaffPerson(entityPersonName);
                string name = staffInformationController.GetNameFromStaffPerson(entityPersonName);

                if (!ENTITY_PERSON_NACHNAME.Equals(em.Type))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine passende Person zu der Anfrage finden."), cancellationToken);
                else if (string.IsNullOrEmpty(email))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine Email zu {name} finden."), cancellationToken);
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Die Email von {name} ist {email}."), cancellationToken);
            }
            else
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich brauche einen Namen zu dem ich eine Email finden soll."), cancellationToken);
        }

        public async Task ProcessIntentGetPhonenumberAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetPhonenumberAsync");

            if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
            {
                EntityModel em = luisResult.ConnectedServiceResult.Entities[0];
                string entityPersonName = TextFormatHelper.RemoveWhitespaceBeforeAfterHyphen(em.Entity);
                string phonenumber = staffInformationController.GetPhonenumberFromStaffPerson(entityPersonName);
                string name = staffInformationController.GetNameFromStaffPerson(entityPersonName);

                if (!ENTITY_PERSON_NACHNAME.Equals(em.Type))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine passende Person zu der Anfrage finden."), cancellationToken);
                else if (string.IsNullOrEmpty(phonenumber))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine Nummer zu {name} finden."), cancellationToken);
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Die Telefonnummer von {name} ist {phonenumber}."), cancellationToken);
            }
            else
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich brauche einen Namen zu dem ich eine Telefonnummer finden soll."), cancellationToken);
        }

        public async Task ProcessIntentGetOfficeHoursAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetOfficeAsync");

            if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
            {
                EntityModel em = luisResult.ConnectedServiceResult.Entities[0];
                string entityPersonName = TextFormatHelper.RemoveWhitespaceBeforeAfterHyphen(em.Entity);
                string officeHours = staffInformationController.GetOfficeHoursFromStaffPerson(entityPersonName);
                string name = staffInformationController.GetNameFromStaffPerson(entityPersonName);

                if (!ENTITY_PERSON_NACHNAME.Equals(em.Type))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine passende Person zu der Anfrage finden."), cancellationToken);
                else if (string.IsNullOrEmpty(officeHours))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich Sprechzeiten von {name} finden."), cancellationToken);
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Die Sprechzeiten von {name} sind {officeHours}."), cancellationToken);
            }
            else
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich brauche einen Namen zu dem ich Sprechzeiten finden soll."), cancellationToken);
        }

        public async Task ProcessIntentGetDepartmentAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetDepartmentAsync");

            if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
            {
                EntityModel em = luisResult.ConnectedServiceResult.Entities[0];
                string entityPersonName = TextFormatHelper.RemoveWhitespaceBeforeAfterHyphen(em.Entity);
                string department = staffInformationController.GetDepartmentFromStaffPerson(entityPersonName);
                string name = staffInformationController.GetNameFromStaffPerson(entityPersonName);

                if (!ENTITY_PERSON_NACHNAME.Equals(em.Type))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine passende Person zu der Anfrage finden."), cancellationToken);
                else if(string.IsNullOrEmpty(department))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keinen Fachbereich zu {name} finden."), cancellationToken);
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Der Fachbereich von {name} ist {department}."), cancellationToken);
            }
            else
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich brauche einen Namen zu dem ich einen Fachbereich finden soll."), cancellationToken);
        }

        public async Task ProcessIntentGetRoomAsync(ITurnContext turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessIntentGetRoomAsync");

            if (luisResult.ConnectedServiceResult.Entities.Count() > 0)
            {
                EntityModel em = luisResult.ConnectedServiceResult.Entities[0];
                string entityPersonName = TextFormatHelper.RemoveWhitespaceBeforeAfterHyphen(em.Entity);
                string room = staffInformationController.GetRoomnumberFromStaffPerson(entityPersonName);
                string name = staffInformationController.GetNameFromStaffPerson(entityPersonName);

                if (!ENTITY_PERSON_NACHNAME.Equals(em.Type))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keine passende Person zu der Anfrage finden."), cancellationToken);
                else if (string.IsNullOrEmpty(room))
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Leider konnte ich keinen Raum zu {name} finden."), cancellationToken);
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Der Raum von {name} ist {room}."), cancellationToken);
            }
            else
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ich brauche einen Namen zu dem ich einen Raum finden soll."), cancellationToken);
        }
    }
}
