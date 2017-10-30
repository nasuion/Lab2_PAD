using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace PAD
{
    class ParserJson
    {
        public static List<Product> ParseString(string data)
        {
            string collection = data.Replace("][", ",");
            List<Product> products = new List<Product>();
            products.AddRange(JsonConvert.DeserializeObject<List<Product>>(collection));
            return products;
        }
    }
}
