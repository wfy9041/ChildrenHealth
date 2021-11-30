using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LaiLaiBear
{
    /// <summary>
    /// Active.xaml 的交互逻辑
    /// </summary>
    public partial class Active : Window
    {
        public Active()
        {
            InitializeComponent();
            Serial.Text =StartUp.GetCpuID();
        }

        private void Act_Click(object sender, RoutedEventArgs e)
        {
            MD5 mD5 = MD5.Create();
            byte[] vs = mD5.ComputeHash(Encoding.ASCII.GetBytes(Serial.Text.ToUpper() + "llx"));
            string pwd = BitConverter.ToString(vs).Replace("-", "").ToUpper().Substring(0, 10);
            mD5.Dispose();
            if(pwd.Equals(lic.Text.ToUpper()))
            {
                using(LLBContext db =new LLBContext())
                {
                    db.login.FirstOrDefault(x => x.ID == 0).pwd = pwd;
                    db.SaveChanges();
                    MessageBox.Show("授权成功");
                    StartUp.isActive = true;
                    Close();
                }
            }
            else
            {
                MessageBox.Show("授权码错误");
            }
        }
    }
}
