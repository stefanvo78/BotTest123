using System.Web.Configuration;
using Microsoft.Azure.Search;

namespace ShopBot.Services
{
    public static class AzureSearch
    {
        public const string Products = "products";

        public static SearchServiceClient CreateClient()
        {
            var searchServiceName = WebConfigurationManager.AppSettings["SearchServiceName"];
            var adminApiKey = WebConfigurationManager.AppSettings["SearchServiceAdminApiKey"];

            return new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
        }
        
        public static FluentSearchClient WithIndex(this SearchServiceClient searchClient, string index)
        {
            return FluentSearchClient.Of(searchClient.Indexes.GetClient(index));
        }
    }
}