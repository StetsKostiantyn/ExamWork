using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using Color = System.Windows.Media.Color;

namespace UDPChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UdpClient udpClient = new UdpClient();

        List<User> users = new List<User>();
        List<Messege> messeges = new List<Messege>();

        IPAddress myip = GetMyIPAddress();
        List<IPAddress> availableIp = new List<IPAddress>();
        List<UdpClient> receiveClients = new List<UdpClient>();

        AppDbContext context = CreateDbContext();

        public MainWindow()
        {
            InitializeComponent();
            context.Database.EnsureCreated();
            Start();
        }

        private void Start()
        {
            foreach (var item in receiveClients)
            {
                item.Close();
            }
            availableIp.Clear();
            receiveClients.Clear();
            TBUsername.Text = context.Set<Users>().First(x => x.Address == myip.ToString()).Name;

            IPAddress[] iPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip4 in iPAddresses.Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
            {
                if (context.Set<Users>().FirstOrDefault(x => x.Address == ip4.ToString()) == null)
                {
                    context.Set<Users>().Add(new Users { Address = ip4.ToString(), Name = "unknown" });
                }
                availableIp.Add(ip4);
                receiveClients.Add(new UdpClient(new IPEndPoint(ip4, Int32.Parse(TBPort.Text))));
            }
            if (context.Set<Users>().FirstOrDefault(x => x.Address == "127.0.0.1") == null)
            {
                context.Set<Users>().Add(new Users { Address = "127.0.0.1", Name = "telnet" });
            }

            context.SaveChanges();

            availableIp.Add(IPAddress.Parse("127.0.0.1"));
            receiveClients.Add(new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), Int32.Parse(TBPort.Text))));
            FillContactsList();
        }

        private static AppDbContext CreateDbContext()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "localhost",
                InitialCatalog = "UDPChat",
                IntegratedSecurity = true,
                MultipleActiveResultSets = true,
                Encrypt = false
            };

            String connStr = builder.ConnectionString;

            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            DbContextOptions options = optionsBuilder.UseSqlServer(connStr).Options;

            return new AppDbContext(options);
        }

        public void FillContactsList()
        {
            int ind = ContactsList.SelectedIndex;
            ContactsList.Items.Clear();
            users.Clear();
            foreach (var item in context.Set<Users>())
            {
                if (availableIp.Contains(IPAddress.Parse(item.Address)))
                {
                    User user;
                    if (context.Set<Messeges>().OrderBy(x => x.Time).LastOrDefault(x => x.Sender.Address == myip.Address.ToString() || x.Receiver.Address == myip.Address.ToString() || x.Sender.Address == item.Address || x.Receiver.Address == item.Address) != null)
                    {
                        user = new User { Address = IPAddress.Parse(item.Address), Name = item.Name, Time = context.Set<Messeges>().OrderBy(x => x.Time).LastOrDefault(x => x.Sender.Address == myip.Address.ToString() || x.Receiver.Address == myip.Address.ToString() || x.Sender.Address == item.Address || x.Receiver.Address == item.Address).Time, Text = context.Set<Messeges>().OrderBy(x => x.Time).LastOrDefault(x => x.Sender.Address == myip.Address.ToString() || x.Receiver.Address == myip.Address.ToString() || x.Sender.Address == item.Address || x.Receiver.Address == item.Address).Text };
                    }
                    else
                    {
                        user = new User { Address = IPAddress.Parse(item.Address), Name = item.Name, Time = item.Time, Text = "" };
                    }
                    users.Add(user);
                }
            }

            foreach (var item in users)
            {
                ContactsList.Items.Add(item);
            }

            ContactsList.SelectedIndex = ind;
        }
        public void FillMessageList(string chatterIp)
        {
            MessageList.Items.Clear();
            messeges.Clear();
            foreach (var item in context.Set<Messeges>().Where(x => (x.Sender.Address == myip.ToString() && x.Receiver.Address == chatterIp) || (x.Sender.Address == chatterIp && x.Receiver.Address == myip.ToString())).OrderBy(x => x.Time))
            {
                Messege messege;
                if (item.Sender.Address == myip.ToString())
                {
                    messege = new Messege { Text = item.Text, Time = item.Time, Alignment = HorizontalAlignment.Right };
                }
                else
                {
                    messege = new Messege { Text = item.Text, Time = item.Time, Alignment = HorizontalAlignment.Left };
                }
                messeges.Add(messege);
            }

            foreach (var item in messeges)
            {
                MessageList.Items.Add(item);
            }
            scrollMessege.ScrollToEnd();
        }

        private void ContactsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactsList.SelectedItem != null)
            {
                receiveClients[ContactsList.SelectedIndex].BeginReceive(ReceiveCallback, receiveClients[ContactsList.SelectedIndex]);
                FillMessageList((ContactsList.SelectedItem as User).Address.ToString());
                MessageBox.Visibility = Visibility.Visible;
                SendButton.Visibility = Visibility.Visible;
                ChatLabel.Content = (ContactsList.SelectedItem as User).Name;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient client = ar.AsyncState as UdpClient;
            IPEndPoint ep = new IPEndPoint(0, 0);

            byte[] buffer = client.EndReceive(ar, ref ep);

            client.BeginReceive(ReceiveCallback, client);

            Application.Current.Dispatcher.Invoke(() =>
            {
                context.Set<Messeges>().Add(new Messeges { Text = Encoding.ASCII.GetString(buffer), Time = DateTime.Now, SenderId = context.Set<Users>().FirstOrDefault(x => x.Address == ep.Address.ToString()).Id, ReceiverId = context.Set<Users>().FirstOrDefault(x => x.Address == myip.ToString()).Id });
                context.SaveChanges();

                FillMessageList((ContactsList.SelectedItem as User).Address.ToString());
                FillContactsList();

                scrollMessege.ScrollToEnd();
            });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Text != String.Empty)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(MessageBox.Text);
                IPAddress ip = IPAddress.Parse((ContactsList.SelectedItem as User).Address.ToString().Substring(0, (ContactsList.SelectedItem as User).Address.ToString().LastIndexOf('.') + 1) + "255");
                IPEndPoint ep = new IPEndPoint(ip, Int32.Parse(TBPort.Text));
                udpClient.Send(buffer, buffer.Length, ep);

                context.Set<Messeges>().Add(new Messeges { Text = MessageBox.Text, Time = DateTime.Now, SenderId = context.Set<Users>().FirstOrDefault(x => x.Address == myip.ToString()).Id, ReceiverId = context.Set<Users>().FirstOrDefault(x => x.Address == (ContactsList.SelectedItem as User).Address.ToString()).Id });
                context.SaveChanges();

                MessageBox.Clear();

                FillMessageList((ContactsList.SelectedItem as User).Address.ToString());
                FillContactsList();
            }
        }

        public static IPAddress GetMyIPAddress()
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses("");

            foreach (IPAddress hostAddress in hostAddresses)
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(hostAddress) && !hostAddress.ToString().StartsWith("169.254."))
                    return hostAddress;
            }
            return null;
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(this, RoutedEventArgs.Empty as RoutedEventArgs);
            }
            if (e.Key == Key.Escape)
            {
                MessageList.Items.Clear();
                ChatLabel.Content = "";
                MessageBox.Visibility = Visibility.Hidden;
                SendButton.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Options(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(500),
                FillBehavior = FillBehavior.HoldEnd
            };
            if (Options.Width > 0)
            {
                da.To = 0;
            }
            else
            {
                da.From = 0;
                da.To = 300;
            }
            SaveButton.BeginAnimation(Button.WidthProperty, da);
            Options.BeginAnimation(Border.WidthProperty, da);
            e.Handled = true;
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            context.Set<Users>().First(x => x.Address == myip.ToString()).Name = TBUsername.Text;
            Start();
        }
    }
}
