using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using IntelligentCarbinet;
using PublicConfig;

namespace IntelligentCarbinet
{
    //public delegate void invokeCtlString(string str);

    public class UDPServer
    {
        public bool bRunning = false;
        public ManualResetEvent Manualstate = new ManualResetEvent(true);
        public StringBuilder sbuilder = new StringBuilder();
        public Socket serverSocket;
        public int port = 5000;

        byte[] byteData = new byte[1024];
        public void stop_listening()
        {
            try
            {
                if (serverSocket != null)
                {
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.Close();
                    serverSocket = null;
                    this.bRunning = false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("UDPServer -> " + ex.Message);
            }
        }
        public void startUDPListening(int _port)
        {
            this.port = _port;
            if (bRunning == true)
            {
                return;
            }
            try
            {
                bRunning = true;
                //We are using UDP sockets
                serverSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);

                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, _port);

                //Bind this address to the server
                serverSocket.Bind(ipEndPoint);

                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                //The epSender identifies the incoming clients
                EndPoint epSender = (EndPoint)ipeSender;

                //Start receiving data
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
                    SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    string.Format("UDPServer.startUDPListening  -> error = {0}"
                    , ex.Message));
            }
        }
        public void OnReceive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = (EndPoint)ipeSender;

                serverSocket.EndReceiveFrom(ar, ref epSender);

                string strReceived = Encoding.UTF8.GetString(byteData);

                //Debug.WriteLine(
                //    string.Format("UDPServer.OnReceive  -> received = {0}"
                //    , strReceived));

                Array.Clear(byteData, 0, byteData.Length);
                int i = strReceived.IndexOf("\0");
                Manualstate.WaitOne();
                Manualstate.Reset();
                //todo here should deal with the received string
                sbuilder.Append(strReceived.Substring(0, i));
                if (sbuilder.Length > 19600)//内容太多清楚掉
                {
                    sbuilder.Remove(0, sbuilder.Length);
                }
                Manualstate.Set();

                //Start listening to the message send by the user
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender,
                    new AsyncCallback(OnReceive), epSender);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    string.Format("UDPServer.OnReceive  -> error = {0}"
                    , ex.Message));
            }
        }
    }
}
