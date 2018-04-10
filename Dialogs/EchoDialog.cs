using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using ShopBot.Dialogs;
using ShopBot;
using ShopBot.Models;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.Contains("products"))
            {
                context.Call(new ProductDialog(), ResumeAfterProductDialog);
            }
            else if (message.Text.Contains("basket"))
            {
                context.Call(new ManageBasketDialog(), ResumeAfterManageBasketDialog);
            }
            else
            {
                await context.PostAsync("Welcome, how can we help you?");
                await RootActions(context);
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

        private static async Task RootActions(IDialogContext context)
        {
            await context.PostAsync("Looking for products? Managing your Basket? Or want to checkout?");
        }

        private async Task ResumeAfterProductDialog(IDialogContext context, IAwaitable<MessageBag<Product>> result)
        {
            var message = await result;
            switch (message.Type)
            {
                case MessageType.ProductOrder:
                    await context.PostAsync($"The user ordered the product \"{message.Content}\"");
                    break;
                case MessageType.ProductRemoval:
                    await context.PostAsync($"The user removed the product {message.Content}");
                    break;
                case MessageType.ProductDialogCancelled:
                    await context.PostAsync("Okay where are we headed next");
                    break;
            }

            await RootActions(context);
            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterManageBasketDialog(IDialogContext context, IAwaitable<object> result)
        {
            await RootActions(context);
        }

    }
}