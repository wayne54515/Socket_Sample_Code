using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket client;
        public String host = "127.0.0.1";
        public int port = 20001;
        public MainWindow()
        {
            InitializeComponent();           
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            //connect to socket server
            IPAddress ip = IPAddress.Parse(host);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(ip, port));
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
        }

        private void Send(object sender, RoutedEventArgs e)
        {
            String text = msg.Text;
            try
            {
                //send message to server
                client.Send(Encoding.ASCII.GetBytes(text + "\n"));
                msg.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                receive_msg.Text += "send Fail\n";
            }
            
        }

        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] result = new byte[1024];
                    int receiveNumber = client.Receive(result);
                    String recStr = Encoding.ASCII.GetString(result, 0, receiveNumber);
                    receive_msg.Dispatcher.BeginInvoke(
                           new Action(() => { receive_msg.Text += recStr; }), null);
                }
                catch (Exception ex)
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    break;
                }
            }

        }
    }
}
