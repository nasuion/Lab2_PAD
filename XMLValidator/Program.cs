using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace XMLValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReaderSettings booksSettings = new XmlReaderSettings();
            booksSettings.Schemas.Add("http://www.contoso.com/books", @"C:\Users\nasui\Documents\Visual Studio 2013\Projects\Lab2_PAD\XMLValidator\bin\Debug\books.xsd");
            //booksSettings.Schemas.Add("","books.xsd");
            booksSettings.ValidationType = ValidationType.Schema;
            booksSettings.ValidationEventHandler += new ValidationEventHandler(booksSettingsValidationEventHandler);

            XmlReader books = XmlReader.Create(@"C:\Users\nasui\Documents\Visual Studio 2013\Projects\Lab2_PAD\XMLValidator\bin\Debug\test.xml", booksSettings);

            while (books.Read()) { }
        }
        static void booksSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("You are the best");
                Console.ReadKey();
            }
        }
    }
}
