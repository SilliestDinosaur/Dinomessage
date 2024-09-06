using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Dinomessage

{
    public class MessageSender : IDisposable  // Implement IDisposable
    {
        private const int Port = 11000;
        private const string MulticastGroupAddress = "224.0.0.1";
        private UdpClient udpClient;

        public MessageSender()
        {
            udpClient = new UdpClient();
            udpClient.JoinMulticastGroup(IPAddress.Parse(MulticastGroupAddress));
        }

        public void SendMessage(string message)
        {
            try
            {
                IPEndPoint multicastEndPoint = new IPEndPoint(IPAddress.Parse(MulticastGroupAddress), Port);
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                udpClient.Send(buffer, buffer.Length, multicastEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }

        public void Dispose()  // Ensure that Dispose() method is correctly implemented
        {
            if (udpClient != null)
            {
                udpClient.Dispose();  // Close the underlying UdpClient properly
                udpClient = null;  // Set to null after disposing
            }
        }
    }
}
