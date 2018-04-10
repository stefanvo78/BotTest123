using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using ShopBot.Dialogs;
using ShopBot;
using ShopBot.Models;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    [LuisModel("96bf245b-55da-4a21-82cc-f987b7223d61", "aa2c91181db640078307d445add81255")]
    public class EchoDialog : LuisDialog<object>
    {
        protected int count = 1;

        private const string EntityProduct = "Product";

        [LuisIntent("")]
        [LuisIntent("None")]
        [LuisIntent("Help")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Welcome, how can we help you?");
            await RootActions(context);

            context.Wait(MessageReceived);
        }

        [LuisIntent("SearchProducts")]
        public async Task SelectProducts(IDialogContext context, IAwaitable<IMessageActivity> messageActivity,
            LuisResult result)
        {
            var message = await messageActivity;
            EntityRecommendation product=null;
            if (result.TryFindEntity(EntityProduct, out product))
            {
                context.Call(new ProductDialog(ProductDialogAction.Add, product.Entity),
                    ResumeAfterProductDialog);
            }
            else
            {
                context.Call(new ProductDialog(ProductDialogAction.Add), ResumeAfterProductDialog);
            }
        }

        [LuisIntent("RemoveProducts")]
        public async Task RemoveProducts(IDialogContext context, IAwaitable<IMessageActivity> messageActivity,
            LuisResult result)
        {
            context.Call(new ProductDialog(ProductDialogAction.Remove), ResumeAfterProductDialog);
        }

        [LuisIntent("ManageBasket")]
        public async Task SearchProducts(IDialogContext context, IAwaitable<IMessageActivity> messageActivity,
            LuisResult result)
        {
            context.Call(new ManageBasketDialog(), ResumeAfterManageBasketDialog);
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
            context.Wait(MessageReceived);
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
            context.Wait(MessageReceived);
        }

        private async Task ResumeAfterManageBasketDialog(IDialogContext context, IAwaitable<object> result)
        {
            await RootActions(context);
        }

    }
}