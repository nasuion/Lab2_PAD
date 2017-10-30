using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discovery
{    
    public interface IOperation
    {
        Task<String> AsyncRead();
        Task AsyncWrite(String message);
    }

    public class TransportService : IOperation
    {
        UdpClient trasport = new UdpClient();

        public TransportService(IPEndPoint broker)
        {
            trasport.Connect(broker);
        }
        public async Task<string> AsyncRead()
        {
            var rec = await trasport.ReceiveAsync();
            return ASCIIEncoding.ASCII.GetString(rec.Buffer, 0, rec.Buffer.Length);
        }

        public async Task AsyncWrite(string message)
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);
            Console.WriteLine(message);
            trasport.SendAsync(bytes, bytes.Length);
        }
    }

    public class BrokerService : IOperation
    {
        UdpClient trasport = new UdpClient();
        TcpClient tcpConn = new TcpClient();
        string protocol;
        public string Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }
        System.Collections.Generic.Dictionary< int,int> receivers=new Dictionary<int,int>();

        public BrokerService(int port = 32123, string protocol = "udp")
        {
            trasport = new UdpClient(port);
            Protocol = protocol;
        }
        public async Task<string> AsyncRead()
        {
            if (Protocol == "udp")
            {
                return await ReadAsyncUDP();
            }
            else
            {
                return await ReadAsyncTcp();
            }
            
        }

        private async Task<string> ReadAsyncUDP()
        {
            var rec = await trasport.ReceiveAsync();
            string s = ASCIIEncoding.ASCII.GetString(rec.Buffer, 0, rec.Buffer.Length);
            Console.WriteLine(s);
            int nr = 0;
            int port = 0;
            string[] args = s.Split(':');
            if (int.TryParse(args[0], out nr))
            {
                int.TryParse(args[0], out nr);
                int.TryParse(args[1], out port);
                receivers.Add(port, nr);

                Console.WriteLine(">> Added " + rec.RemoteEndPoint);
            }
            return "This node are "+args[0]+" connections";
        }

        public async Task AsyncWrite(string message)
        {            
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), receivers.Max(x=>x.Key));
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);            
            try
            {
                tcpConn.Connect(ipep);
                tcpConn.Client.Send(bytes);
                //tcpConn.Client.Receive(bytes);                
            }
            catch
            {
                Console.WriteLine(">> A problem occurs");
            }
        }
        public async Task<string> ReadAsyncTcp()
        {
            byte[] data = new byte[4092];
            int receivedDataLength = tcpConn.Client.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
            Console.WriteLine(">> Received from server:" + stringData);
            tcpConn.Close();
            return stringData;
        }
    }

    public class ReceiveInfoEventArgs : EventArgs
    {
        public string ip { get; set; }
        public byte[] data_bytes { get; set; }
        public string message { get; set; }
        public ReceiveInfoEventArgs(byte[] _data_bytes, string _message, string _ip)
        {
            data_bytes = _data_bytes;
            message = _message;
            ip = _ip;
        }
    }

    public class UdpSimple
    {
        const int PORT_NUMBER = 9050;

        public event EventHandler<ReceiveInfoEventArgs> ReceiveEvent;
        private UdpClient udp = null;

        Thread t = null;
        public void Start()
        {
            if (t != null)
            {
                throw new Exception(">> Already started, stop first");
            }
            udp = new UdpClient();
            udp.ExclusiveAddressUse = false;
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, PORT_NUMBER);
            // Creates an IPAddress to use to join and drop the multicast group.
            IPAddress multicastIpAddress = IPAddress.Parse("239.255.255.255");

            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udp.ExclusiveAddressUse = false;
            udp.Client.Bind(localEP);
            try
            {
                // The packet dies after 50 router hops.
                udp.JoinMulticastGroup(multicastIpAddress, 50);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine(">> Started listening");
            //t = new Thread(new ThreadStart(StartListening));
            //t.Start();
        }
        
        public void Stop()
        {
            try
            {
                if (udp != null)
                    udp.Close();
                Console.WriteLine(">> Stopped listening");
            }
            catch { /* don't care */ }
        }


        IAsyncResult ar_ = null;

        public void StartListening()
        {
            ar_ = udp.BeginReceive(Receive, new object());
        }
        private void Receive(IAsyncResult ar)
        {
            Console.WriteLine(">> Begin receive");
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, PORT_NUMBER);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);

            RaiseReceiveEvent(bytes, message, ip.Address.ToString());

            StartListening();
        }
        public void Send(string message)
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("239.255.255.255"), PORT_NUMBER);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            client.Send(bytes, bytes.Length, ip);
            client.Close();
            Console.WriteLine(">> Sent: \n{0} ", message);
        }

        private void RaiseReceiveEvent(byte[] bytes, string message, string ip)
        {
            Console.WriteLine(message + " ip: " + ip);
            ReceiveInfoEventArgs e = new ReceiveInfoEventArgs(bytes, message, ip);
            EventHandler<ReceiveInfoEventArgs> handler = ReceiveEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
 