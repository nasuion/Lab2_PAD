using Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace PAD
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            UdpSimple server = new UdpSimple();
            Thread.Sleep(1000);
            server.Start();
            string message = ">> Get connection number";
            server.Send(message);
            Console.WriteLine(">> Request sent");
            server.Stop();
            BrokerService broker = new BrokerService(32321);
            Task t = Task.Factory.StartNew(async () =>
            {
                int time = 6;
                while (time > 0)
                {
                    message = await broker.AsyncRead();
                    Console.WriteLine(">> Received: " + message);
                    time--;
                }
                Thread.Sleep(150);
                string m = "get data";
                broker.Protocol = "tcp";
                await broker.AsyncWrite(m);
                string rawData = await broker.AsyncRead();
                List<Product> product = ParserJson.ParseString(rawData);

                Console.WriteLine(">> Starting to validate XML");
                string xmlMessage = "";

                
                product.ForEach(x =>
                {
                    xmlMessage = Product.Deserialize(x);
                    //System.IO.File.WriteAllText(@"C:\Users\nasui\Documents\Visual Studio 2013\Projects\Lab2_PAD\XMLValidator\bin\Debug\test.xml", message);
             
                });
                
                Console.WriteLine("\n>> Order list by Price");

                product = product.OrderBy(x => x.Price).ToList();
                product.ForEach(x => Console.WriteLine(">> "+x));
            });
           // XmlWriter xmlWriter = XmlWriter.Create("test.xml");
           
            t.Wait();
            Console.ReadKey();
        }
    }
}
