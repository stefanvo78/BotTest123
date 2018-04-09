using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ShopBot.Dialogs
{
    [Serializable]
    public class ProductDialog : IDialog<MessageBag<string>>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("You have these options: \n\n 1. Order products \n\n 2. Remove products from Basket");
            context.Wait(ActionSelectionReceivedAsync);
        }

        private async Task ActionSelectionReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            switch (message.Text)
            {
                case "1":
                    await context.PostAsync("Name of the product you want to order:");
                    context.Wait(ProductSelectionReceivedAsync);
                    break;
                case "2":
                    await context.PostAsync("Name of the product you want to remove:");
                    context.Wait(PromptProductRemoval);
                    break;
            }
        }

        private async Task ProductSelectionReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            context.Done(MessageBag.Of(message.Text, MessageType.ProductOrder));
        }

        private async Task PromptProductRemoval(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            context.Done(new MessageBag<string>(message.Text, MessageType.ProductRemoval));
        }
    }
}