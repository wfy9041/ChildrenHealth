using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Active
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Act_Click(object sender, RoutedEventArgs e)
        {
            MD5 mD5 = MD5.Create();
            byte[] vs = mD5.ComputeHash(Encoding.ASCII.GetBytes(SerialID.Text.ToUpper() + "llx"));
            Key.Text = BitConverter.ToString(vs).Replace("-", "").ToUpper().Substring(0, 10);
            mD5.Dispose();
        }
    }
}
