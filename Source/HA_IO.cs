using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HA_IO
{
    public class IO
    {
        public int version = 1;

        public void Init()
        {

        }

        public void Log(string text)
        {
            Console.WriteLine("[LOG] " + text);
        }

        public void DebugLog(string text)
        {
            Console.WriteLine("[DEBUG] " + text);
        }
    }

    public class Server
    {
        IO class_IO = new IO();

        private bool active = false;
        private int p;
        private string t;

        public Server(int port, string token)
        {
            p = port;
            t = token;
        }

        TcpListener _server;
        TcpClient _client;
        Stream _stream;

        public void Loop()
        {
            string request = "";
            byte[] message = new byte[4096];
            int bytesRead = 0;
            bool receivedError = false;

            try
            {
                _server = new TcpListener(p);
                _server.Start();
                active = true;
            }
            catch
            {
                active = false;
            }

            class_IO.Log("Waiting for clients ...");

            if (active)
            {
                try
                {
                    _client = _server.AcceptTcpClient();
                    _stream = _client.GetStream();
                }
                catch { }
            }



            while (active)
            {
                class_IO.Log("Connection from '" + ((IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString() + "'");
                try
                {
                    bytesRead = _stream.Read(message, 0, 4096);
                    receivedError = false;
                }
                catch
                {
                    receivedError = true;
                }

                if (bytesRead == 0 || receivedError)
                {
                    _client.Close();
                    _server.Stop();
                    Loop();
                    break;
                }
                else
                {
                    try
                    {
                        ASCIIEncoding encoder = new ASCIIEncoding();
                        request = encoder.GetString(message, 0, bytesRead);
                        Process(request);
                    }
                    catch { }
                }
            }
        }

        private bool Send(string text)
        {
            bool successful = true;
            byte[] szData;
            szData = System.Text.Encoding.ASCII.GetBytes(text.ToCharArray());

            foreach (byte b in szData)
            {
                try
                {
                    _stream.WriteByte(b);
                }
                catch
                {
                    successful = false;
                }
            }

            return successful;
        }

        private void Process(string request)
        {
            if (request.Contains("info"))
                Send("success");
            //Check token and process all actions
        }
    }
}
