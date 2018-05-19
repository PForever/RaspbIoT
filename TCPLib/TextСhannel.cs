using System.Text;

namespace TCPLib
{
    public class TextChannel
    {
        public void SandDatagram(string datagram)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(datagram);
            Connecter.Send(bytes);
        }

        public string ReceiverDatagram()
        {
            byte[] bytes = Connecter.Receiver().Result;
            return Encoding.UTF8.GetString(bytes);
        }
    }
}