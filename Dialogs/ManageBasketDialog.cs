using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ShopBot.Dialogs
{
    [Serializable]
    public class ManageBasketDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            //ToDo
            return Task.CompletedTask;
        }
    }
}