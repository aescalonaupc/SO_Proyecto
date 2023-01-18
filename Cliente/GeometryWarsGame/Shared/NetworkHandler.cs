using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Shared
{
    public static class NetworkHandler
    {
        /// <summary>
        /// Internal socket for networking
        /// </summary>
        private static Socket? socket = null;

        /// <summary>
        /// Internal thread for networking
        /// </summary>
        private static Thread? thread = null;

        /// <summary>
        /// If networking should be running or not
        /// </summary>
        private static volatile bool running = false;

        /// <summary>
        /// Event to send incoming messages to listeners
        /// </summary>
        public static event NetworkMessageEvent? OnNetworkMessage;
        public delegate void NetworkMessageEvent(string message);

        /// <summary>
        /// Read incoming messages and forward them to listeners
        /// </summary>
        private static void NetworkLoop()
        {
            if (thread == null || socket == null)
            {
                return;
            }

            while (running)
            {
                byte[] buffer = new byte[8 * 1024]; // 8 kb!
                int readBytes;

                try
                {
                    readBytes = socket.Receive(buffer);
                } catch (Exception)
                {
                    continue;
                }

                if (readBytes <= 0)
                {
                    running = false;
                    continue;
                }

                string[] messages = Encoding.ASCII.GetString(buffer).Split('\0')[0].Split('$');

                foreach (string m in messages)
                {
                    string message = m.Trim();

                    if (message.Length <= 0)
                    {
                        continue;
                    }

                    OnNetworkMessage?.Invoke(message);
                }
            }
        }

        /// <summary>
        /// Send data to remote host
        /// </summary>
        /// <param name="data"></param>
        public static void Send(string data)
        {
            if (socket == null)
            {
                return;
            }

            // Using try/catch since `.Send()` might arise SocketException (SendTimeout != 0)
            Task.Run(() => { try { socket.Send(Encoding.ASCII.GetBytes(data + "$")); } catch (Exception) { } }).ConfigureAwait(false);
        }

        /// <summary>
        /// Initializes and starts networking
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="receiveTimeout"></param>
        /// <param name="sendTimeout"></param>
        /// <returns></returns>
        public static bool Initialiaze(string ip, int port, int receiveTimeout = 0, int sendTimeout = 0)
        {
            if (socket != null || thread != null || ip.Length <= 0 || port <= 0)
            {
                return false;
            }

            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = receiveTimeout,
                    SendTimeout = sendTimeout,
                };

                thread = new Thread(NetworkLoop)
                {
                    Priority = ThreadPriority.AboveNormal
                };

                socket.Connect(ipEndPoint);

                running = true;
                thread.Start();
            } catch (Exception) { return false; }

            return true;
        }

        /// <summary>
        /// Stops networking thread and socket
        /// </summary>
        public static void Stop()
        {
            if (thread == null || socket == null)
            {
                return;
            }

            running = false;
            thread.Join();

            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        /// <summary>
        /// Returns if networking is ready to be used
        /// </summary>
        /// <returns></returns>
        public static bool IsReady()
        {
            return socket != null && socket.Connected && thread != null && thread.IsAlive;
        }

        /// <summary>
        /// Returns internal socket used for networking
        /// </summary>
        /// <returns></returns>
        public static Socket? GetSocket() { return socket; }

        /// <summary>
        /// Returns the internal thread used for networking
        /// </summary>
        /// <returns></returns>
        public static Thread? GetThread() { return thread; }

    }
}
