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
            Messeges.Add(new Messege() { Text = "hi\ndiahgd", Time = DateTime.Now, Alignment = HorizontalAlignment.Left });
            Messeges.Add(new Messege() { Text = "add", Time = DateTime.Now, Alignment = HorizontalAlignment.Right });
            foreach (var item in Messeges)
            {
                MessageList.Items.Add(item);
            }
        }

        private void ContactsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Метод вызывается, когда меняется индекс выделенного элемента
            //При выделении элемент списка будет подсвечиваться
            //Чтобы убрать это, мы будем менять индекс на -1
            //Чтобы метод не срабатывал повторно, мы проверяем, чтобы индекс был больше или равен 0
            if (ContactsList.SelectedIndex >= 0)
            {
                //Тут будет код загрузки сообщений из чата

                //Сбрасываем индекс
                ContactsList.SelectedIndex = -1;

                //Open(ChatScreen);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //Пока используем тестовые данные
            //if (LoginBox.Text == "admin" && PasswordBox.Password == "12345")
            //{
            //    //Если логин и пароль верные, то переходим на другой экран
            //    Open(ContactsScreen);
            //}
            //else
            //{
            //    //Иначе выводим сообщение об ошибке авторизации
            //    //LoginMessageBlock.Text = "Wrong login or password!";
            //    //LoginMessageBlock.Visibility = Visibility.Visible;
            //}
        }

        //Метод для открытия другого экрана
        private void Open(Border screen)
        {
            //Делаем все экраны невидимыми
            //LoginScreen.Visibility = Visibility.Hidden;
            //ContactsScreen.Visibility = Visibility.Hidden;
            //ChatScreen.Visibility = Visibility.Hidden;

            //Делаем видимым необходимый экран
            screen.Visibility = Visibility.Visible;
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


        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            if (ContactsList.SelectedIndex >= 0)
            {
                //Тут будет код загрузки сообщений из чата

                //Сбрасываем индекс
                ContactsList.SelectedIndex = -1;

                //Open(ChatScreen);
            }
        }

        //private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    usersScroll.Width = mainWindow.Width / 3; 
        //}

        //private void ButtonShowHide_Click(object sender, RoutedEventArgs e)
        //{
        //    DoubleAnimation daScrl = new DoubleAnimation
        //    {
        //        Duration = TimeSpan.FromMilliseconds(500),
        //        FillBehavior = FillBehavior.HoldEnd
        //    };

        //    if (usersScroll.Width > 0)
        //    {
        //        daScrl.To = 90;
        //    }
        //    else
        //    {
        //        daScrl.From = 90;
        //        daScrl.To = mainWindow.Width / 3;
        //    }

        //    usersScroll.BeginAnimation(ScrollViewer.WidthProperty, daScrl);
        //    e.Handled = true;
        //}
    }
}
