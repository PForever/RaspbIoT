using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Reflection;
using LogLib;

namespace TCPLib
{
    public static class Connecter
    {
        private static ILogger Logger { get; }
        private const int LocalPort = 8081;
        private const int RemotePort = 8081;
        private static readonly UdpClient Sender;
        private static readonly UdpClient Listener;
        private const string IpAddress = "192.168.1.2";
        private static IPEndPoint LocalEndPoint { get; }
        private static IPEndPoint RemoteEndPoint { get; }
        static Connecter()
        {
            Logger = new Logger();
            LocalEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), RemotePort);
            Sender = new UdpClient();
            Listener = new UdpClient(LocalPort);
        }

        public static void InitializeConnection()
        {
            Sender.Client.Connect(LocalEndPoint);
        }

        public static void Send(byte[] bytes)
        {
            try
            {
                Sender.SendAsync(bytes, bytes.Length, LocalEndPoint);
            }
            catch (Exception e)
            {
                Logger.Wright(LogLvl.Err, "", e.Message);
                throw e;
            }
        }

        public static async Task<byte[]> Receiver()
        {
            UdpReceiveResult result;
            try
            {
                result = await Listener.ReceiveAsync();
            }
            catch (Exception e)
            {
                Logger.Wright(LogLvl.Err, "", e.Message);
                throw e;
            }
            return result.Buffer;
        }
    }
}
