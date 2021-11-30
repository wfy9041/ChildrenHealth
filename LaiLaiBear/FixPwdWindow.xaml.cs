using System;
using System.Collections.Generic;
using System.Linq;
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
    /// FixPwdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FixPwdWindow : Window
    {
        public FixPwdWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            using (LLBContext lLBContext = new LLBContext())
            {
                login login = lLBContext.login.FirstOrDefault(x => x.user.Equals("admin"));
                if (Opwd.Password.Equals(login.pwd))
                {
                    if (Npwd.Password.Equals(Rpwd.Password))
                    {
                        login.pwd = Rpwd.Password;
                        lLBContext.SaveChanges();
                        MessageBox.Show("修改成功");
                        Close();
                    }
                    else
                        MessageBox.Show("两次新密码不一致");
                }
                else
                    MessageBox.Show("旧密码不正确");
            }
        }

        private void CANCEL_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
