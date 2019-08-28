using EchoBot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Dialogs
{
    public class ModuleDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<Module> _moduleAccessor;
        private const string ModuleInfo = "value-moduleInfo";

        public ModuleDialog(UserState userState) : base(nameof(ModuleDialog))
        {
            _moduleAccessor = userState.CreateProperty<Module>("Module");

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                NameStepAsync,
                AcknowledgementStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[ModuleInfo] = new Module();

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
            //return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        private async Task<DialogTurnResult> AcknowledgementStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var moduleInfo = (Module)stepContext.Values[ModuleInfo];
            moduleInfo.Title = (string)stepContext.Result;
            // Set the user's company selection to what they entered in the review-selection dialog.
            //var userProfile = (Module)stepContext.Values[ModuleInfo];
            //userProfile.CompaniesToReview = stepContext.Result as List<string> ?? new List<string>();

            // Thank them for participating.
            //await stepContext.Context.SendActivityAsync(
            //MessageFactory.Text($"Thanks for participating, {((UserProfile)stepContext.Values[UserInfo]).Name}."),
            //cancellationToken);

            // Exit the dialog, returning the collected user information.
            return await stepContext.EndDialogAsync(stepContext.Values[ModuleInfo], cancellationToken);
        }
    }
}
