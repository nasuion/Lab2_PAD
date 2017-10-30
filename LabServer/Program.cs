using Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LabServer
{
    class Program
    {
        static int Main(string[] args)
        {
            foreach (var s in args)
            {
                Console.WriteLine(">> Arguments: {0}", s);
            }

            if (args.Length < 2)
            {
                Console.WriteLine(">> Need more than 2 args");
                return 1;
            }
            UdpSimple server = new UdpSimple();
            server.Start();
            UdpHelper udp_helper = new UdpHelper(args[0], args[1]);
            server.ReceiveEvent += udp_helper.ReceiveUpdData;
            server.StartListening();
            int nr;
            int.TryParse(args[0], out nr);
            string[] neighbours = args.Skip(2).ToArray<string>();
            TcpTransport tcp = new TcpTransport(nr, neighbours);

            Console.ReadKey();

            return 0;
        }
    }
    public class UdpHelper
    {
        private readonly string port, conectionNumber;

        public UdpHelper(string port, string number)
        {
            this.port = port;
            this.conectionNumber = number;
        }

        public void ReceiveUpdData(object sender, ReceiveInfoEventArgs e)
        {

            {
                Console.WriteLine(">> From {0} received:\n{1} ", e.ip, e.message);
            }
            IOperation receiver = new TransportService(new IPEndPoint(
                                   IPAddress.Parse("127.0.0.1"), 32321));

            Task t = Task.Factory.StartNew(async () =>
            {
                await receiver.AsyncWrite(conectionNumber + ":" + port);

                string m;
                while ((m = await receiver.AsyncRead()) != "quit r")
                {
                    Console.WriteLine(m);
                }
            });
            t.Wait();
        }
    }

}
