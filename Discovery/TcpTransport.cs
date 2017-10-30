using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Discovery
{
    public class TcpTransport:IOperation
    {
        private TcpListener server;
        int port;
        private bool serverIsRunning;
        private string[] neighbours;

        public TcpTransport(int port, string[]neighbours)
        {
            this.port = port;
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            this.neighbours = neighbours;
            startServer();
        }

        ~TcpTransport()
        {
            server.Server.Close();
        }

        private void startServer()
        {
            try
            {
                server.Start();
                serverIsRunning = true;
                listen();
            }
            catch (SocketException e)
            {
                Console.Write(e.Message);
            }

        }

        private async void listen()
        {
            while (serverIsRunning)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine(">> Connected!\n>> Soket " + client.Client.RemoteEndPoint);
                String data = null;
                Byte[] bytes = new Byte[2048];

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;
                Task t = Task.Run(async () =>
                 {
                     NetworkStream _stream = client.GetStream();
                     while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                     {
                        // Translate data bytes to a ASCII string.
                        data = Encoding.ASCII.GetString(bytes, 0, i);

                         if (data == "get data")
                         {
                             data = CollectData();
                             data += LoadJson();
                         }
                         Console.WriteLine(">> Received: {0}", data);

                         if (data == "load")
                         {
                             data = LoadJson();
                         }
                        // Process the data sent by the client.

                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                         Console.WriteLine(">> Sent: {0}", data);
                     }
                     // Shutdown and end connection
                     client.Close();
                 });
                t.Wait();
            }
            server.Server.Close();
        }


        private string ReadStoredData(string port)
        {
            int portNr;
            int.TryParse(port,out portNr);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNr);
            TcpClient tcpConn = new TcpClient();
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes("load");
            string stringData="";
            //ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            try
            {
                tcpConn.Connect(ipep);
                tcpConn.Client.Send(bytes);
                SocketAsyncEventArgs args=new SocketAsyncEventArgs();
                //tcpConn.Client.Receive(bytes);
                byte[] data = new byte[4092];
                int receivedDataLength = tcpConn.Client.Receive(data);
                stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
                Console.WriteLine(">> Read from neigbours:" + stringData);
                tcpConn.Close();
            }
            catch
            {
                Console.WriteLine(">> A problem occurs");
            }
            return stringData;
        }
        public string LoadJson()
        {
            string filePath = @"LabServer\" + port + ".json";
            Console.WriteLine(">> Path: "+filePath);
            if (File.Exists(filePath))
            { 
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    Console.WriteLine(">> Read from file:" + json);
                    return json;
                }
            } return ">> NO FILE EXIST";
        }

        private string CollectData()
        {
            string data="";
            Task t = Task.Factory.StartNew(async () =>
            {
                neighbours.ToList().ForEach(x => { data += ReadStoredData(x); });
            });
            t.Wait();            
            return data;
        }

        public async Task<string> AsyncRead()
        {
            //NetworkStream stream = client.GetStream();
            TcpClient client = await server.AcceptTcpClientAsync();
            //var rec = await server.;
            return "abc";//ASCIIEncoding.ASCII.GetString(rec.Buffer, 0, rec.Buffer.Length);
        }

        public Task AsyncWrite(string message)
        {
            throw new NotImplementedException();
        }
    }
    
}
