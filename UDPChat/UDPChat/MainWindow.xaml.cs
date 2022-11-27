using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UDPChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<User> users = new List<User>();
        List<Messege> Messeges = new List<Messege>();
        UdpClient udpClient = new UdpClient();
        List<UdpClient> receiveClients = new List<UdpClient>();
        public MainWindow()
        {
            InitializeComponent();

            IPAddress[] iPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip4 in iPAddresses.Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
            {
                users.Add(new User(ip4));
                receiveClients.Add(new UdpClient(new IPEndPoint(ip4, 1024)));
            }
            users.Add(new User(IPAddress.Parse("127.0.0.1")));
            receiveClients.Add(new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024)));

            foreach (var item in users)
            {
                ContactsList.Items.Add(item);
            }

            scrollMessege.ScrollToEnd();
            foreach (var item in Messeges)
            {
                MessageList.Items.Add(item);
            }
            MessageBox.Visibility = Visibility.Hidden;
            SendButton.Visibility = Visibility.Hidden;
        }

        private void ContactsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageList.Items.Clear();
            MessageBox.Visibility = Visibility.Visible;
            SendButton.Visibility = Visibility.Visible;
            receiveClients[ContactsList.SelectedIndex].BeginReceive(ReceiveCallback, receiveClients[ContactsList.SelectedIndex]);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient client = ar.AsyncState as UdpClient;
            IPEndPoint ep = new IPEndPoint(0, 0);

            byte[] buffer = client.EndReceive(ar, ref ep);

            client.BeginReceive(ReceiveCallback, client);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Messeges.Add(new Messege() { Text = Encoding.ASCII.GetString(buffer), Alignment = HorizontalAlignment.Right, Time = DateTime.Now });
                MessageList.Items.Add(Messeges.Last());
                scrollMessege.ScrollToEnd();
            });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Text != String.Empty)
            {
                byte[] buffer = Encoding.ASCII.GetBytes($"{MessageBox.Text}");
                IPAddress ip = IPAddress.Parse((ContactsList.SelectedItem as User).Address.ToString().Substring(0, (ContactsList.SelectedItem as User).Address.ToString().LastIndexOf('.') + 1) + "255");
                IPEndPoint ep = new IPEndPoint(ip, 1024);
                udpClient.Send(buffer, buffer.Length, ep);
                MessageBox.Clear();
            }
        }
    }
}
