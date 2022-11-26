using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public MainWindow()
        {
            InitializeComponent();

            IPAddress[] iPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip4 in iPAddresses.Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
            {
                users.Add(new User(ip4));
            }
            users.Add(new User(IPAddress.Parse("127.0.0.1")));

            foreach (var item in users)
            {
                ContactsList.Items.Add(item);
            }
            Messeges.Add(new Messege() { Text = "hi\nhow are you", Time = DateTime.Now, Alignment = HorizontalAlignment.Left });
            Messeges.Add(new Messege() { Text = "add", Time = DateTime.Now, Alignment = HorizontalAlignment.Right });
            foreach (var item in Messeges)
            {
                MessageList.Items.Add(item);
            }
        }

        private void ContactsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public class User
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public IPAddress Address { get; set; }
            public DateTime Time { get; set; }

            public string TextString { get { return $"{Name}\n\n{Address.ToString()}"; } }
            public string TimeString
            {
                get
                {
                    if (DateTime.Now - Time < TimeSpan.FromDays(1))
                    {
                        return Time.ToString("HH:mm");
                    }
                    else
                    {
                        if (DateTime.Now - Time < TimeSpan.FromDays(7))
                        {
                            return Time.ToString("ddd");
                        }
                        else
                        {
                            return Time.ToString("dd.mm.yyyy");
                        }
                    }
                }
            }

            public User(IPAddress address, string name = "unknown")
            {
                Address = address;
                Name = name;
            }
        }

        public class Messege
        {
            public string Text { get; set; }
            public DateTime Time { get; set; }
            public HorizontalAlignment Alignment { get; set; }
            public string TimeString
            {
                get { return Time.ToString("HH:mm"); }
            }
        }
    }
}
