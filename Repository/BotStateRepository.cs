using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using ShopBot.Models;

namespace ShopBot.Repository
{
    public class BotStateRepository
    {
        public const string ProductsInBasket = "ProductsInBasket";
        
        public static void AddProductToBasket(IDialogContext context, Product resultFromProductDialog)
        {
            IList<Product> products;
            context.ConversationData.TryGetValue(ProductsInBasket, out products);
            (products ?? (products = new List<Product>())).Add(resultFromProductDialog);
            context.ConversationData.SetValue(ProductsInBasket, products);
        }

        public static void RemoveProductFromBasket(IDialogContext context, string name)
        {
            IList<Product> products;
            if (context.ConversationData.TryGetValue(ProductsInBasket, out products))
            {
                var listWithoutProduct =
                    products.Where(product => !product.Name.Equals(name)).ToList();

                context.ConversationData.SetValue(ProductsInBasket, listWithoutProduct);
            }
        }

        public static void RemoveProductsFromBasket(IDialogContext context, List<string> namesList)
        {
            IList<Product> products;
            if (context.ConversationData.TryGetValue(ProductsInBasket, out products))
            {
                var listWithoutProduct =
                    products.Where(product => !namesList.Contains(product.Name)).ToList();

                context.ConversationData.SetValue(ProductsInBasket, listWithoutProduct);
            }
        }

        public static void DeleteAllProducts(IDialogContext context)
        {

            context.ConversationData.SetValue(ProductsInBasket, new List<Product>());
        }

        public static IList<Product> GetAllProducts(IDialogContext context)
        {
            IList<Product> products;
            if (context.ConversationData.TryGetValue(ProductsInBasket, out products))
            {
                return products;
            }
            return new List<Product>();
        }
    }
}