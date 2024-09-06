using System;                 // For basic functionalities (Exception, etc.)
using System.Net;             // For working with IP addresses and endpoints
using System.Net.Sockets;     // For using UdpClient and Socket related operations
using System.Text;            // For encoding/decoding strings (UTF-8)
using System.Threading;       // For threading (if using background threads)


namespace Dinomessage
{
    public partial class Form1 : Form
    {
        private MessageSender sender;
        private MessageReceiver receiver;

        public Form1()
        {
            InitializeComponent();
            InitializeMessaging();
        }

        private void InitializeMessaging()
        {
            sender = new MessageSender();  // Properly instantiate the MessageSender
            receiver = new MessageReceiver(UpdateMessageList);
            receiver.StartListening();
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                this.sender.SendMessage(message);
                txtMessage.Clear();
            }
        }

        // This method will be called to update the UI with received messages
        private void UpdateMessageList(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateMessageList), message);
                return;
            }
            lstMessages.Items.Add(message);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            receiver.StopListening();  // Stop the receiver gracefully
            this.sender.Dispose();      // Dispose the MessageSender
        }
    }
}
