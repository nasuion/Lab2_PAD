using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PAD
{   //[XmlRoot(ElementName="bookstore xmlns=\"http://www.contoso.com/books\"")]
    [XmlRoot(ElementName = "book")]
    public class Product
    {
      [XmlElement(ElementName = "title")]
        public String title { get; set; }
      [XmlElement(ElementName = "author")]
      public String author { get; set; }
      [XmlElement(ElementName = "price")]
      public Double Price { get; set; }


      public Product(String title, String author, Double price)
      {
          this.title = title;
          this.author = author;
          this.Price = price;
        
      }
      public Product()
      {

      }

      public override string ToString()
      {
          return "book{" +
                    "title='" + title + '\'' +
                    ", author='" + author + '\'' +
                    ", price=" + Price +
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
         
         
          return message;
      }
}
  public class Products
  {
      public List<Product> data { get; set; }
  }
}
