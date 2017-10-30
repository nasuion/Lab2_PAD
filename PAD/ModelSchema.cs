using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PAD
{
  [XmlRoot(ElementName = "Product")]
    public class Product
    {
      [XmlElement(ElementName = "FullName")]
      public String FullName { get; set; }
      [XmlElement(ElementName = "Model")]
      public String Model {get; set;}
      [XmlElement(ElementName = "Price")]
      public Double Price { get; set; }

      [XmlElement(ElementName = "Guarantee")]
      public String Guarantee { get; set; }

      public Product(String fullName, String model, Double price, String guarantee)
      {
          this.FullName = fullName;
          this.Model = model;
          this.Price = price;
          this.Guarantee = guarantee;
      }
      public Product()
      {

      }

      public override string ToString()
      {
          return "Product{" +
                    "fullName='" + FullName + '\'' +
                    ", model='" + Model + '\'' +
                    ", price=" + Price +
                    ", guarantee='" + Guarantee +"'"+
              
              '}';
      }
      public static string Deserialize<T>(T value)
      {
          Console.WriteLine(" ************************************ \n");
          Console.WriteLine(">> ModelClass : " + value);
          var xmlserializer = new XmlSerializer(typeof(T));
          var stringWriter = new StringWriter();

          string message;
          MemoryStream stream = new MemoryStream();

          xmlserializer.Serialize(stream, value);
          stream.Position = 0;
          StreamReader sr = new StreamReader(stream);
          message = sr.ReadToEnd();
          Console.WriteLine(">> XML : " + message);
          if (xmlserializer.CanDeserialize(new XmlTextReader(new StringReader(message))))
              Console.WriteLine(">> XMLSchema: This product is valid");
          else Console.WriteLine(">> XMLSchema: This products is invalid");
          return message;

      }
}
  public class Products
  {
      public List<Product> data { get; set; }
  }
}
