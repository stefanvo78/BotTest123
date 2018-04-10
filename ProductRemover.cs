using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using ShopBot.Models;
using ShopBot.Repository;

namespace ShopBot
{
    public class ProductRemover
    {
        private readonly List<string> _productsToRemove;

        public ProductRemover(List<string> productsToRemove)
        {
            _productsToRemove = productsToRemove;
        }

        public static ProductRemover Of(dynamic response)
        {
            response.Remove("Type");
            var json = response.ToString() as string;
            var dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
            var list = dict.Where(p => p.Value).Select(p => p.Key).ToList();
            return new ProductRemover(list);
        }

        public List<string> Remove(IDialogContext context)
        {
            BotStateRepository.RemoveProductsFromBasket(context, _productsToRemove);
            return _productsToRemove;
        }
    }
}