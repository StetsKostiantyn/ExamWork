using System;
using System.Net;

namespace UDPChat
{
    public partial class MainWindow
    {
        public class User
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public IPAddress Address { get; set; }
            public DateTime Time { get; set; }

            public string TextString { get { return $"{Name}\n{Address.ToString()}\n{Text}"; } }
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
        }
    }
}
