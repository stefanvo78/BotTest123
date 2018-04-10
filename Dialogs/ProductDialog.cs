using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ShopBot.Models;
using ShopBot.Services;
using System.Linq;

namespace ShopBot.Dialogs
{
    [Serializable]
    public class ProductDialog : IDialog<MessageBag<Product>>
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
                    context.Wait(ProductOptionsReceivedAsync);
                    break;
                case "2":
                    await context.PostAsync("Name of the product you want to remove:");
                    context.Wait(PromptProductRemoval);
                    break;
            }
        }
        private async Task ProductOptionsReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var products = AzureSearch.CreateClient()
                .WithIndex(AzureSearch.Products)
                .Find<Product>(message.Text);

            if (products.Any())
            {
                PromptDialog.Choice(context, ProductSelectionReceivedAsync, products, "Add to basket:");
            }
            else
            {
                await context.PostAsync("No products found please try with another query.");
                context.Wait(ProductOptionsReceivedAsync);
            }
        }

        private async Task ProductSelectionReceivedAsync(IDialogContext context, IAwaitable<Product> result)
        {
            var product = await result;
            context.Done(MessageBag.Of(product, MessageType.ProductOrder));
        }

        private async Task PromptProductRemoval(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            context.Done(new MessageBag<string>(message.Text, MessageType.ProductRemoval));
        }
    }
}