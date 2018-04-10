using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Bot.Builder.FormFlow;

namespace ShopBot.Models
{
    [Serializable]
    public class ProductQuery
    {
        [Required] 
        public string ProductName { get; set; }

        [Required] 
        public string Sort { get; set; }

        [Required] 
        [Range(1, 60)] 
        public int Limit { get; set; }

        public static ProductQuery Parse(dynamic o)
        {
            try
            {
                return new ProductQuery
                {
                    ProductName = o.ProductName.ToString(),
                    Sort = o.Sort.ToString(),
                    Limit = int.Parse(o.Limit.ToString())
                };
            }
            catch
            {
                throw new InvalidCastException("ProductQuery could not be read");
            }
        }
    }
}