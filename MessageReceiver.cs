using System;                 // For basic functionalities (Exception, etc.)
using System.Net;             // For working with IP addresses and endpoints
using System.Net.Sockets;     // For using UdpClient and Socket related operations
using System.Text;            // For encoding/decoding strings (UTF-8)
using System.Threading;       // For threading (if using background threads)


namespace Dinomessage
{
    public class MessageReceiver : IDisposable
    {
        private const int Port = 11000;
        private const string MulticastGroupAddress = "224.0.0.1";

        private UdpClient udpClient;
        private Thread receiveThread;
        private bool isListening;
        private Action<string> messageReceivedCallback;

        public MessageReceiver(Action<string> messageReceivedCallback)
        {
            this.messageReceivedCallback = messageReceivedCallback;
            udpClient = new UdpClient(Port);
            udpClient.JoinMulticastGroup(IPAddress.Parse(MulticastGroupAddress));
        }

        public void StartListening()
        {
            isListening = true;
            receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ReceiveMessages()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, Port);

            while (isListening)
            {
                try
                {
                    if (udpClient.Available > 0)
                    {
                        byte[] data = udpClient.Receive(ref remoteEndPoint);
                        string message = Encoding.UTF8.GetString(data);
                        messageReceivedCallback?.Invoke(message);
                    }
                    else
                    {
                        Thread.Sleep(100);  // Avoid busy-waiting
                    }
                }
                catch (SocketException ex)
                {
                    // Catch and handle errors properly when closing.
                    Console.WriteLine("Socket error: " + ex.Message);
                }
                catch (ObjectDisposedException)
                {
                    // If the udpClient is closed, just exit the loop.
                    break;
                }
            }
        }

        public void StopListening()
        {
            isListening = false;
            udpClient.Close();  // Close UdpClient to unblock Receive()
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Join();  // Wait for the thread to stop gracefully
            }
        }

        public void Dispose()
        {
            StopListening();
            udpClient.Dispose();
        }
    }
}
