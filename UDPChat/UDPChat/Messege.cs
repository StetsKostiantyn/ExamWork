using System;
using System.Windows;

namespace UDPChat
{
    public partial class MainWindow
    {
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
