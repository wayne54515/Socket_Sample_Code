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

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket server;
        public String host = "127.0.0.1";
        public int port = 20001;

        public MainWindow()
        {
            InitializeComponent();
        }

        //start a socket server
        private void Start(object sender, RoutedEventArgs e)
        { 
            if (server == null)
            {
                receive_msg.Text = "Server Start";
                IPAddress ip = IPAddress.Parse(host);
                //socket()
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //bind()
                server.Bind(new IPEndPoint(ip, port));
                //listen()
                server.Listen(10);

                Thread thread = new Thread(Listen);
                thread.Start();
            }
        }

        //listen to socket client
        private void Listen()
        {
            while (true)
            {
                //accept()
                Socket client = server.Accept();

                Thread receive = new Thread(ReceiveMsg);
                receive.Start(client);
            }
        }

        //receive client message and send to client
        public void ReceiveMsg(object client)
        {
            Socket connection = (Socket)client;
            IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
            receive_msg.Dispatcher.BeginInvoke(
                new Action(() => { receive_msg.Text += "\n" + clientIP + " connect\n"; }), null);
            //send welcome message to client
            connection.Send(Encoding.ASCII.GetBytes("Welcome " + clientIP + "\n"));
            while (true)
            {
                try
                {
                    byte[] result = new byte[1024];
                    //receive message from client
                    int receive_num = connection.Receive(result);
                    String receive_str = Encoding.ASCII.GetString(result, 0, receive_num);
                    if (receive_num > 0)
                    {
                        String send_str = clientIP + " : " + receive_str;

                        //resend message to client
                        connection.Send(Encoding.ASCII.GetBytes("You send: " + receive_str));

                        receive_msg.Dispatcher.BeginInvoke(
                            new Action(() => { receive_msg.Text += send_str; }), null);
                        
                    }
                }
                catch (Exception e)
                {
                    //exception close()
                    Console.WriteLine(e);
                    connection.Shutdown(SocketShutdown.Both);
                    connection.Close();
                    break;
                }
            }
        }

        //close() when close window
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
