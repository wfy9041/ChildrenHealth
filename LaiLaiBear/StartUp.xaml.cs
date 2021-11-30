using System;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LaiLaiBear
{
    /// <summary>
    /// StartUp.xaml 的交互逻辑
    /// </summary>
    public partial class StartUp : Window
    {
        public static bool isActive= true;
        public StartUp()
        {
            InitializeComponent();
            Logo.Source = ChangeBitmapToBitmapSource( Properties.Resources.logo);
            string cpuid= GetCpuID();
            using (LLBContext db =new LLBContext())
            {
                login login = db.login.FirstOrDefault(x => x.ID == 0);
                if (!login.user.Equals(cpuid))
                {
                    login.user = cpuid;
                    db.SaveChanges();
                }
                MD5 mD5 = MD5.Create();
                byte[] vs= mD5.ComputeHash(Encoding.ASCII.GetBytes(cpuid + "llx"));
                string pwd=  BitConverter.ToString(vs).Replace("-","").ToUpper().Substring(0,10);
                mD5.Dispose();
                if (!pwd.Equals(login.pwd))
                {
                    isActive = false;
                    Active active = new Active();
                    active.ShowDialog();
                    if(!isActive)
                    {
                        Close();
                    }
                }
            }
            this.ForceCursor = true;
            this.Activate();
        }



        /// <summary>
        /// 从Bitmap转换成BitmapSource
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private  BitmapSource ChangeBitmapToBitmapSource(Bitmap bmp)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            using(LLBContext db =new LLBContext() )
            {
                login login = db.login.FirstOrDefault(x => x.user.Equals(user.Text));
                login Limit = db.login.FirstOrDefault(x =>x.user.Equals("LimitTime"));
                if (Limit != null && DateTime.Today > DateTime.Parse("2020-12-31"))
                    MessageBox.Show("已过测试时间，请联系开发人员");
                else
                {
                    if (login is null)
                    {
                        MessageBox.Show("用户名不存在");
                    }
                    else if (login.pwd.Equals(pwd.Password))
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("密码错误");
                    }
                }
            }
        }

        public static string GetCpuID()
        {
            string cpuInfo = "";//cpu序列号
            ManagementClass cimobject = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                //Response.Write ("cpu序列号："+cpuInfo.ToString ());
            }
            return cpuInfo;
        }

        private void pwd_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Enter)
            {
                Login_Click(null,null);
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FixPwdWindow fixPwdWindow = new FixPwdWindow();
            fixPwdWindow.ShowDialog();
        }
    }
}
