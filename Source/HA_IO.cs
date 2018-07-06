/*
MIT License

Copyright (c) 2018 Patti4832

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
