using System;
using Newtonsoft.Json;

namespace ShopBot.Models
{
    [Serializable]
    public class Product
    {
        [JsonProperty("ProductID")]
        public string ProductId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Color")]
        public string Color { get; set; }

        [JsonProperty("ListPrice")]
        public double ListPrice { get; set; }

        [JsonProperty("Size")]
        public string Size { get; set; }

        public override string ToString()
        {
            return $"{Name} (${ListPrice})";
        }
    }
}