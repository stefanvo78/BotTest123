using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using ShopBot.Repository;
using ShopBot.Models;
using AdaptiveCards;
using ShopBot.CustomCards;
using System.Threading;
using System.Linq;

namespace ShopBot.Dialogs
{
    [Serializable]
    public class ManageBasketDialog : IDialog<object>
    {
        private const string BasketContents = "Show products in basket";
        private const string ProductRemoval = "Remove single products";
        private const string EmptyBasket = "Clear basket";

        public Task StartAsync(IDialogContext context)
        {
            var options = new List<string>
    {
        BasketContents,
        ProductRemoval,
        EmptyBasket
    };
            PromptDialog.Choice(context, AfterOptionSelection, options, "Manage basket:");
            return Task.CompletedTask;
        }

        private async Task AfterOptionSelection(IDialogContext context, IAwaitable<string> result)
        {
            var optionSelected = await result;
            await ExecuteAction(context, optionSelected);
        }

        private async Task ExecuteAction(IDialogContext context, string optionSelected)
        {
            switch (optionSelected)
            {
                case BasketContents:
                    await ShowBasketContentsCard(context);
                    break;
                case ProductRemoval:
                    await ShowProductRemovalCard(context);
                    context.Wait(RemoveProductMessageRecievedAsync);
                    break;
                case EmptyBasket:
                    //TODO
                    break;
            }
        }

        private async Task ShowBasketContentsCard(IDialogContext context)
        {
            IList<Product> products;
            context.ConversationData.TryGetValue(BotStateRepository.ProductsInBasket, out products);

            Attachment attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = CardFactory.GetProductsBasketCard(products)
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply, CancellationToken.None);
            context.Done("Basket contents viewed");
        }

        private async Task ShowProductRemovalCard(IDialogContext context)
        {
            IList<Product> products;
            context.ConversationData.TryGetValue(BotStateRepository.ProductsInBasket, out  products);

            Attachment attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = CardFactory.GetDeleteProductsFromBasketCard(products)
            };
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply, CancellationToken.None);
        }

        private async Task RemoveProductMessageRecievedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var removed = ProductRemover.Of(message.Value).Remove(context);
            await context.PostAsync(removed.Any() ?
                "Items removed from basket: \n\n* " + string.Join(" \n\n* ", removed) :
                "No products removed");
            context.Done("Items deleted");
        }
    }
}