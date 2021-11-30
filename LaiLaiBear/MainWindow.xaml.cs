using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Runtime.InteropServices;
using SHDocVw;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Configuration;

namespace LaiLaiBear
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
       
        private bool NewUser;
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
        internal interface IServiceProvider
        {
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object QueryService(ref Guid guidService, ref Guid riid);
        }
        static readonly Guid SID_SWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");

        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Media.Brush SkinBrush;
            BrushConverter ReportbrushbrushConverter = new BrushConverter();
            SkinBrush = (System.Windows.Media.Brush)ReportbrushbrushConverter.ConvertFromString(ConfigurationManager.AppSettings["SkinColor"].ToString());
            Background = SkinBrush;
            this.FontFamily = new System.Windows.Media.FontFamily(ConfigurationManager.AppSettings["SkinFont"].ToString());
            double size = (double)(this.Height / 1080 * 20);
            this.FontSize = size;
            SearchStartDate.SelectedDate = DateTime.Today.AddMonths(-1);
            SearchEndDate.SelectedDate = DateTime.Today;
            ResfList();
            TextReader text = new StreamReader(@".\model2p.html");
            Report.NavigateToString(text.ReadToEnd());
        }

        private void ResfList()
        {
            using (LLBContext context = new LLBContext())
            {
                if (SearchStartDate.SelectedDate == null)
                    SearchStartDate.SelectedDate = DateTime.Today.AddMonths(-1);
                if (SearchEndDate.SelectedDate == null)
                    SearchEndDate.SelectedDate = DateTime.Today.AddDays(1);

                int start = (int)(SearchStartDate.SelectedDate.Value - new DateTime(1970, 1, 1)).TotalSeconds;
                int End = (int)(SearchEndDate.SelectedDate.Value - new DateTime(1970, 1, 1)).TotalSeconds;
                List<user> users = context.user.Where(x => x.SJRQ >= start && x.SJRQ <= End).ToList();
                AllData.ItemsSource = users;
                NoPrintData.ItemsSource = users.Where(x => x.isPrintf == false).ToList();
                PrintData.ItemsSource = users.Where(x => x.isPrintf == true).ToList();
            }
        }

        private void GenerateReport(user user, List<ParasiteImageControl> images)
        {
            TextReader text = new StreamReader(@".\model2p.html");
            StringBuilder report = new StringBuilder(text.ReadToEnd());
            text.Close();
            report.Replace("[#Hospital#]", ConfigurationManager.AppSettings["Hospital"].ToString() + "(测试)");
            report.Replace("[#Report#]", ConfigurationManager.AppSettings["Report"].ToString() + "(测试)");
            report.Replace("[#HospitalFont#]", ConfigurationManager.AppSettings["HospitalFont"].ToString());
            report.Replace("[#ReportFont#]", ConfigurationManager.AppSettings["ReportFont"].ToString());
            report.Replace("[#HospitalColor#]", ConfigurationManager.AppSettings["HospitalColor"].ToString());
            report.Replace("[#ReportColor#]", ConfigurationManager.AppSettings["ReportColor"].ToString());
            report.Replace("[#姓名#]", user.Name);
            report.Replace("[#性别#]", user.Sex);
            report.Replace("[#年龄#]", user.Age);
            report.Replace("[#科室#]", user.KS);
            report.Replace("[#住院号#]", user.ZYH);
            report.Replace("[#标本编号#]", user.BBBH);
            report.Replace("[#床号#]", user.CH);
            report.Replace("[#送检医师#]", user.SJYS);
            report.Replace("[#送检时间#]", new DateTime(1970, 1, 1).AddSeconds(user.SJRQ).ToShortDateString());
            report.Replace("[#检验师#]", user.JYYS);
            report.Replace("[#审核医生#]", user.SHYS);
            report.Replace("[#报告日期#]", new DateTime(1970, 1, 1).AddSeconds(user.BGRQ).ToShortDateString());

            report.Replace("[#白细胞WBC#]", user.XY_bxb);
            report.Replace("[#红细胞RBC#]", user.XY_hxb);
            report.Replace("[#血红蛋白HGB#]", user.XY_xhdb);
            report.Replace("[#血小板PLT#]", user.XY_xxb);

            report.Replace("[#尿红细胞计数#]", user.NY_nhxbjs);
            report.Replace("[#尿白细胞计数#]", user.NY_nbxbjs);
            report.Replace("[#未分类结晶#]", user.NY_wfljj);
            report.Replace("[#病理管型#]", user.NY_blgx);
            report.Replace("[#诊断意见#]", user.diagnose);

            if (user.BBMYD.Equals("3"))
            {
                report.Replace("[#满意#]", "&radic; ");
                report.Replace("[#基本满意#]", "");
                report.Replace("[#需要重新采样#]", "");
            }
            else if (user.BBMYD.Equals("2"))
            {
                report.Replace("[#满意#]", "");
                report.Replace("[#基本满意#]", "&radic; ");
                report.Replace("[#需要重新采样#]", "");
            }
            else if (user.BBMYD.Equals("1"))
            {
                report.Replace("[#满意#]", "");
                report.Replace("[#基本满意#]", "");
                report.Replace("[#需要重新采样#]", "&radic; ");
            }
            else
            {
                report.Replace("[#满意#]", "");
                report.Replace("[#基本满意#]", "");
                report.Replace("[#需要重新采样#]", "");
            }

            if (user.FB_bxb.Equals("A"))
                report.Replace("[#粪便白细胞#]", "阴性（&emsp;）&emsp;阳性（&radic;）");
            else if (user.FB_bxb.Equals("K"))
                report.Replace("[#粪便白细胞#]", "阴性（&radic;）&emsp;阳性（&emsp;）");
            else
                report.Replace("[#粪便白细胞#]", "阴性（&emsp;）&emsp;阳性（&emsp;）");

            if (user.FB_hxb.Equals("A"))
                report.Replace("[#粪便红细胞#]", "阴性（&emsp;）&emsp;阳性（&radic;）");
            else if (user.FB_bxb.Equals("K"))
                report.Replace("[#粪便红细胞#]", "阴性（&radic;）&emsp;阳性（&emsp;）");
            else
                report.Replace("[#粪便红细胞#]", "阴性（&emsp;）&emsp;阳性（&emsp;）");

            if (user.FB_cn.Equals("S"))
                report.Replace("[#粪便虫卵#]", "未见（&emsp;）&emsp;己见（&radic;）");
            else if (user.FB_cn.Equals("H"))
                report.Replace("[#粪便虫卵#]", "未见（&radic;）&emsp;己见（&emsp;）");
            else
                report.Replace("[#粪便虫卵#]", "未见（&emsp;）&emsp;己见（&emsp;）");

            if (user.FB_bn.Equals("S"))
                report.Replace("[#粪便包囊#]", "未见（&emsp;）&emsp;己见（&radic;）");
            else if (user.FB_bn.Equals("H"))
                report.Replace("[#粪便包囊#]", "未见（&radic;）&emsp;己见（&emsp;）");
            else
                report.Replace("[#粪便包囊#]", "未见（&emsp;）&emsp;己见（&emsp;）");

            if (user.FB_jj.Equals("S"))
                report.Replace("[#粪便结晶#]", "未见（&emsp;）&emsp;己见（&radic;）");
            else if (user.FB_jj.Equals("H"))
                report.Replace("[#粪便结晶#]", "未见（&radic;）&emsp;己见（&emsp;）");
            else
                report.Replace("[#粪便结晶#]", "未见（&emsp;）&emsp;己见（&emsp;）");


            if (images.Count < 2)
            {
                List<string> addName = new List<string>() { "\\img\\蛔虫.PNG", "\\img\\绦虫.PNG", "\\img\\钩虫.PNG", "\\img\\蛲虫.PNG" };
                int index = 0;
                for (int has = images.Count; has < 2; has++, index++)
                {
                    ParasiteImageControl parasite = new ParasiteImageControl();
                    FileInfo file = new FileInfo(Environment.CurrentDirectory +  addName[index]);
                    parasite.Picture.Source = new BitmapImage(new Uri(file.FullName, UriKind.Absolute));
                    parasite.ParasiteName.Text = file.Name.Replace(file.Extension, "");
                    parasite.FullFileName = file.FullName;
                    parasite.ParasiteCount.Text = "0";
                    images.Add(parasite);
                }
            }

            report.Replace("[#img1#]", images[0].FullFileName);
            report.Replace("[#虫名1#]", images[0].ParasiteName.Text);
            report.Replace("[#数量1#]", images[0].ParasiteCount.Text);

            report.Replace("[#img2#]", images[1].FullFileName);
            report.Replace("[#虫名2#]", images[1].ParasiteName.Text);
            report.Replace("[#数量2#]", images[1].ParasiteCount.Text);


            Report.NavigateToString(report.ToString());
        }

        private void GenerateReport()
        {
            TextReader text = new StreamReader(@".\model2p.html");
            StringBuilder report = new StringBuilder(text.ReadToEnd());
            text.Close();
            report.Replace("[#Hospital#]", ConfigurationManager.AppSettings["Hospital"].ToString() + "(测试)");
            report.Replace("[#Report#]", ConfigurationManager.AppSettings["Report"].ToString() + "(测试)");
            report.Replace("[#HospitalFont#]", ConfigurationManager.AppSettings["HospitalFont"].ToString());
            report.Replace("[#ReportFont#]", ConfigurationManager.AppSettings["ReportFont"].ToString());
            report.Replace("[#HospitalColor#]", ConfigurationManager.AppSettings["HospitalColor"].ToString());
            report.Replace("[#ReportColor#]", ConfigurationManager.AppSettings["ReportColor"].ToString());
            report.Replace("[#姓名#]", C_Name.Text);
            if(C_Sex_M.IsChecked.Value)
            report.Replace("[#性别#]","男");
            else
                report.Replace("[#性别#]", "女");
            report.Replace("[#年龄#]", C_Age.Text);
            report.Replace("[#科室#]", C_Ks.Text);
            report.Replace("[#住院号#]", C_Zyh.Text);
            report.Replace("[#标本编号#]", C_Bbbh.Text);
            report.Replace("[#床号#]", C_Ch.Text);
            report.Replace("[#送检医师#]", C_Sjys.Text);
            report.Replace("[#送检时间#]", C_Sjrq.Text);
            report.Replace("[#检验师#]", C_JJYS.Text);
            report.Replace("[#审核医生#]", C_SHYS.Text);
            report.Replace("[#报告日期#]", C_bgrq.Text);

            report.Replace("[#白细胞WBC#]", C_xy_bxb.Text);
            report.Replace("[#红细胞RBC#]", C_xy_hxb.Text);
            report.Replace("[#血红蛋白HGB#]", C_xy_xhdb.Text);
            report.Replace("[#血小板PLT#]", C_xy_xxb.Text);

            report.Replace("[#尿红细胞计数#]", C_ny_nhxbjs.Text);
            report.Replace("[#尿白细胞计数#]", C_ny_nbxbjs.Text);
            report.Replace("[#未分类结晶#]", C_ny_wfljj.Text);
            report.Replace("[#病理管型#]", C_ny_blgx.Text);
            string diagnose = C_Diagnose.Text.Replace("\r\n", "<br>");
            report.Replace("[#诊断意见#]", diagnose);

            if (C_My.IsChecked.Value)
            {
                report.Replace("[#满意#]", "&radic; ");
                report.Replace("[#基本满意#]", "");
                report.Replace("[#需要重新采样#]", "");
            }
            else if (C_Jbmy.IsChecked.Value)
            {
                report.Replace("[#满意#]", "");
                report.Replace("[#基本满意#]", "&radic; ");
                report.Replace("[#需要重新采样#]", "");
            }
            else if (C_Xycxcy.IsChecked.Value)
            {
                report.Replace("[#满意#]", "");
                report.Replace("[#基本满意#]", "");
                report.Replace("[#需要重新采样#]", "&radic; ");
            }
            else
            {
                report.Replace("[#满意#]", "");
                report.Replace("[#基本满意#]", "");
                report.Replace("[#需要重新采样#]", "");
            }

            if (fb_bxb_A.IsChecked.Value)
                report.Replace("[#粪便白细胞#]", "阴性（&emsp;）&emsp;阳性（&radic;）");
            else if (fb_bxb_K.IsChecked.Value)
                report.Replace("[#粪便白细胞#]", "阴性（&radic;）&emsp;阳性（&emsp;）");
            else
                report.Replace("[#粪便白细胞#]", "阴性（&emsp;）&emsp;阳性（&emsp;）");

            if (fb_hxb_A.IsChecked.Value)
                report.Replace("[#粪便红细胞#]", "阴性（&emsp;）&emsp;阳性（&radic;）");
            else if (fb_hxb_K.IsChecked.Value)
                report.Replace("[#粪便红细胞#]", "阴性（&radic;）&emsp;阳性（&emsp;）");
            else
                report.Replace("[#粪便红细胞#]", "阴性（&emsp;）&emsp;阳性（&emsp;）");

            if (fb_cl_S.IsChecked.Value)
                report.Replace("[#粪便虫卵#]", "未见（&emsp;）&emsp;己见（&radic;）");
            else if (fb_cl_H.IsChecked.Value)
                report.Replace("[#粪便虫卵#]", "未见（&radic;）&emsp;己见（&emsp;）");
            else
                report.Replace("[#粪便虫卵#]", "未见（&emsp;）&emsp;己见（&emsp;）");

            if (fb_bl_S.IsChecked.Value)
                report.Replace("[#粪便包囊#]", "未见（&emsp;）&emsp;己见（&radic;）");
            else if (fb_bl_H.IsChecked.Value)
                report.Replace("[#粪便包囊#]", "未见（&radic;）&emsp;己见（&emsp;）");
            else
                report.Replace("[#粪便包囊#]", "未见（&emsp;）&emsp;己见（&emsp;）");

            if (fb_jj_S.IsChecked.Value)
                report.Replace("[#粪便结晶#]", "未见（&emsp;）&emsp;己见（&radic;）");
            else if (fb_jj_H.IsChecked.Value)
                report.Replace("[#粪便结晶#]", "未见（&radic;）&emsp;己见（&emsp;）");
            else
                report.Replace("[#粪便结晶#]", "未见（&emsp;）&emsp;己见（&emsp;）");

            List<ParasiteImageControl> images = new List<ParasiteImageControl>();
            foreach (var i in Parasites.Children)
            {
                images.Add(i as ParasiteImageControl);
            }


            if (images.Count < 2)
            {
                List<string> addName = new List<string>() { "\\img\\蛔虫.PNG", "\\img\\绦虫.PNG", "\\img\\钩虫.PNG", "\\img\\蛲虫.PNG" };
                int index = 0;
                for (int has = images.Count; has < 2; has++, index++)
                {
                    ParasiteImageControl parasite = new ParasiteImageControl();
                    FileInfo file = new FileInfo(Environment.CurrentDirectory + addName[index] );
                    parasite.Picture.Source = new BitmapImage(new Uri(file.FullName, UriKind.Absolute));
                    parasite.ParasiteName.Text = file.Name.Replace(file.Extension, "");
                    parasite.FullFileName = file.FullName;
                    parasite.ParasiteCount.Text = "0";
                    images.Add(parasite);
                }
            }

            report.Replace("[#img1#]", images[0].FullFileName);
            report.Replace("[#虫名1#]", images[0].ParasiteName.Text);
            report.Replace("[#数量1#]", images[0].ParasiteCount.Text);

            report.Replace("[#img2#]", images[1].FullFileName);
            report.Replace("[#虫名2#]", images[1].ParasiteName.Text);
            report.Replace("[#数量2#]", images[1].ParasiteCount.Text);


            Report.NavigateToString(report.ToString());
        }

        private void Printf_Click(object sender, RoutedEventArgs e)
        {
            IServiceProvider serviceProvider = null;
            if (Report.Document != null)
            {
                serviceProvider = (IServiceProvider)Report.Document;
                using(LLBContext bear = new LLBContext())
                {
                   LaiLaiBear.user user = bear.user.FirstOrDefault(x => x.BBBH.Equals(C_Bbbh.Text));
                    if(user!=null)
                    {
                        user.isPrintf = true;
                        bear.SaveChanges();
                    }    
                }
            }
            Guid serviceGuid = SID_SWebBrowserApp;
            Guid iid = typeof(IWebBrowser2).GUID;
            IWebBrowser2 myWebBrowser2 = (IWebBrowser2)serviceProvider.QueryService(ref serviceGuid, ref iid);
            object NullValue = null;
            myWebBrowser2.ExecWB(OLECMDID.OLECMDID_PRINT, OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref NullValue, ref NullValue);
        }

        private void FastPrintf_Click(object sender, RoutedEventArgs e)
        {
            IServiceProvider serviceProvider = null;
            if (Report.Document != null)
            {
                serviceProvider = (IServiceProvider)Report.Document;
                using (LLBContext bear = new LLBContext())
                {
                    LaiLaiBear.user user = bear.user.FirstOrDefault(x => x.BBBH.Equals(C_Bbbh.Text));
                    if (user != null)
                    {
                        user.isPrintf = true;
                        bear.SaveChanges();
                    }
                }
            }
            Guid serviceGuid = SID_SWebBrowserApp;
            Guid iid = typeof(IWebBrowser2).GUID;
            IWebBrowser2 myWebBrowser2 = (IWebBrowser2)serviceProvider.QueryService(ref serviceGuid, ref iid);
            object NullValue = null;
            myWebBrowser2.ExecWB(OLECMDID.OLECMDID_PRINT, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref NullValue, ref NullValue);
        }

        private void BeachPrintf_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                TextReader text = new StreamReader(@"Z:\OneDrive\ChildrenHealth\LaiLaiBear\report.html");
                Report.NavigateToString(text.ReadToEnd());
                IServiceProvider serviceProvider = null;
                if (Report.Document != null)
                {
                    serviceProvider = (IServiceProvider)Report.Document;
                }
                Guid serviceGuid = SID_SWebBrowserApp;
                Guid iid = typeof(IWebBrowser2).GUID;
                IWebBrowser2 myWebBrowser2 = (IWebBrowser2)serviceProvider.QueryService(ref serviceGuid, ref iid);
                object NullValue = null;
                myWebBrowser2.ExecWB(OLECMDID.OLECMDID_PRINT, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref NullValue, ref NullValue);
            }
        }

        private void AddSamplePic_Click(object sender, RoutedEventArgs e)
        {
            if (Parasites.Children.Count < 2)
            {
                ParasiteListWindow parasiteListWindow = new ParasiteListWindow();
                parasiteListWindow.AddParasite += ParasiteListWindow_AddParasite;
                parasiteListWindow.ShowDialog();
            }
            else
                MessageBox.Show("最多添加2个", "提示");
        }

        private void SamplePic_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(C_Bbbh.Text))
            {
                if (Parasites.Children.Count < 2)
                {
                    SamplePic samplePic = new SamplePic(C_Bbbh.Text,Parasites.Children.Count);
                    samplePic.SampleEvent += SamplePic_SampleEvent;
                    samplePic.ShowDialog();
                }
                else
                    MessageBox.Show("最多添加2个", "提示");
            }
            else
            {
                MessageBox.Show("请输入标本编号", "提示");
            }
        }

        private void SamplePic_SampleEvent(List<string> PicName)
        {
            foreach (var vfile in PicName)
            {
                if (Parasites.Children.Count < 2)
                {
                    FileInfo file = new FileInfo(vfile);
                    ParasiteImageControl parasite = new ParasiteImageControl();
                    FileStream fileStream = new FileStream(vfile, FileMode.Open, FileAccess.Read);
                    fileStream.Position = 0;
                    MemoryStream memoryStream = new MemoryStream();
                    fileStream.CopyTo(memoryStream);
                    fileStream.Dispose();
                    memoryStream.Position = 0;
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = memoryStream;
                    imageSource.EndInit();
                    parasite.Picture.Source = imageSource;
                    parasite.ParasiteName.Text = file.Name.Replace(file.Extension, "").Split('#')[1];
                    parasite.FullFileName = vfile;
                    Parasites.Children.Add(parasite);
                }
            }
        }

        /// <summary>
        /// 增加图样
        /// </summary>
        /// <param name="FullFilePath">地图地址</param>
        private void ParasiteListWindow_AddParasite(string FullFilePath)
        {
            FileInfo file = new FileInfo(FullFilePath);
            ParasiteImageControl parasite = new ParasiteImageControl();
            FileStream fileStream = new FileStream(FullFilePath,FileMode.Open,FileAccess.Read);
            fileStream.Position = 0;
            MemoryStream memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            fileStream.Dispose();
            memoryStream.Position = 0;
            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = memoryStream;
            imageSource.EndInit();
            parasite.Picture.Source = imageSource;
            if(file.FullName.Contains("SampleLib"))
                parasite.ParasiteName.Text = file.Name.Replace(file.Extension, "").Split('#')[1];
            else
            parasite.ParasiteName.Text = file.Name.Replace(file.Extension, "");
            parasite.FullFileName = FullFilePath;
            Parasites.Children.Add(parasite);
        }

        private void Srach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int start = (int)(SearchStartDate.SelectedDate.Value - new DateTime(1970, 1, 1)).TotalSeconds;
                int end = (int)(SearchEndDate.SelectedDate.Value.AddDays(1) - new DateTime(1970, 1, 1)).TotalSeconds;
                using (LLBContext context = new LLBContext())
                {
                    List<user> users = context.user.Where(x => x.SJRQ >= start && x.SJRQ <= end).ToList();
                    if (!string.IsNullOrWhiteSpace(SearchName.Text))
                        users = users.Where(x => x.Name.Contains(SearchName.Text)).ToList();
                    AllData.ItemsSource = users;
                    NoPrintData.ItemsSource = users.Where(x => x.isPrintf == false).ToList();
                    PrintData.ItemsSource = users.Where(x => x.isPrintf == true).ToList();
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("请设置正确的开始时间和结束时间");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            GC.Collect();

        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            NewUser = true;
            C_Bbbh.IsEnabled = true;
            C_Name.Focus();
            Parasites.Children.Clear();
            using (LLBContext context = new LLBContext())
            {
                user user = context.user.OrderByDescending(x => x.ID).FirstOrDefault();
                if (user == null)
                    user = new user() { BBBH = DateTime.Today.ToString("yy0000") };
                var res = Regex.Match(user.BBBH, @"\d+$").Value;
                string IDheader = user.BBBH.Substring(0, user.BBBH.Length - res.Length);
                if (res.Length != 0)
                {
                    int id = int.Parse(res);
                    id += 1;
                    C_Bbbh.Text = IDheader + id.ToString().PadLeft(res.Length, '0');
                    Clear();
                }
            }
        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(C_Bbbh.Text))
            {
                MessageBox.Show("请输入[标本编号]", "错误", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrWhiteSpace(C_Name.Text))
            {
                MessageBox.Show("请输入[姓名]", "错误", MessageBoxButton.OK);
                return;
            }

            using (LLBContext db = new LLBContext())
            {
                user user = db.user.FirstOrDefault(x => x.BBBH.Equals(C_Bbbh.Text));
                if (NewUser && user != null)
                {
                    MessageBox.Show($"[标本编号]:{C_Bbbh.Text}已有记录", "错误", MessageBoxButton.OK);
                    return;
                }
                if (NewUser && user == null)
                {
                    user = new user();
                }
                user.Age = C_Age.Text;
                user.Name = C_Name.Text;
                user.BBBH = C_Bbbh.Text;
                user.CH = C_Ch.Text;
                user.BGRQ = (int)(C_bgrq.SelectedDate.Value - new DateTime(1970, 1, 1)).TotalSeconds;
                user.JYYS = C_JJYS.Text;
                user.KS = C_Ks.Text;
                user.ZYH = C_Zyh.Text;

                if (C_Sex_M.IsChecked.Value)
                    user.Sex = "男";
                else
                    user.Sex = "女";
                user.SHYS = C_SHYS.Text;
                user.SJRQ = (int)(C_Sjrq.SelectedDate.Value - new DateTime(1970, 1, 1)).TotalSeconds;
                user.SJYS = C_Sjys.Text;
                user.XY_bxb = C_xy_bxb.Text;
                user.XY_hxb = C_xy_hxb.Text;
                user.XY_xhdb = C_xy_xhdb.Text;
                user.XY_xxb = C_xy_xxb.Text;
                user.NY_nhxbjs = C_ny_nhxbjs.Text;
                user.NY_nbxbjs = C_ny_nbxbjs.Text;
                user.NY_wfljj = C_ny_wfljj.Text;
                user.NY_blgx = C_ny_blgx.Text;
                user.diagnose = C_Diagnose.Text;

                if (C_My.IsChecked.Value)
                    user.BBMYD = "3";
                else if (C_Jbmy.IsChecked.Value)
                    user.BBMYD = "2";
                else if (C_Xycxcy.IsChecked.Value)
                    user.BBMYD = "1";
                //粪便 红细胞
                if (fb_hxb_A.IsChecked.Value)
                    user.FB_hxb = "A";
                else
                    user.FB_hxb = "K";
                //粪便 白细胞
                if (fb_bxb_A.IsChecked.Value)
                    user.FB_bxb = "A";
                else
                    user.FB_bxb = "K";

                //粪便 虫卵
                if (fb_cl_S.IsChecked.Value)
                    user.FB_cn = "S";
                else
                    user.FB_cn = "H";

                //粪便 包囊
                if (fb_bl_S.IsChecked.Value)
                    user.FB_bn = "S";
                else
                    user.FB_bn = "H";

                //粪便 结晶
                if (fb_jj_S.IsChecked.Value)
                    user.FB_jj = "S";
                else
                    user.FB_jj = "H";
                if (NewUser)
                    db.user.Add(user);

                List<image> image = db.image.Where(x => x.BBBH.Equals(C_Bbbh.Text)).ToList();

                foreach (var a in image)
                {
                    db.image.Remove(a);
                }

                string str2 = Environment.CurrentDirectory ;
                foreach (var pic in Parasites.Children)
                {
                    ParasiteImageControl p = pic as ParasiteImageControl;
                    string FileName = p.FullFileName.Substring(str2.Length, p.FullFileName.Length - str2.Length);
                    db.image.Add(new LaiLaiBear.image() { BBBH = C_Bbbh.Text, Count = p.ParasiteCount.Text, PicName = FileName });
                }
                db.SaveChanges();
            }
            ResfList();
            GC.Collect();
        }

        private void LoadUser(user user)
        {
            NewUser = false;
            C_Bbbh.IsEnabled = false;
            C_Name.Text = user.Name;
            if (user.Sex.Equals("男"))
                C_Sex_M.IsChecked = true;
            else
                C_Sex_W.IsChecked = true;
            C_Age.Text = user.Age;
            C_Bbbh.Text = user.BBBH;
            C_Zyh.Text = user.ZYH;
            C_Ks.Text = user.KS;
            C_Ch.Text = user.CH;
            C_Sjys.Text = user.SJYS;
            C_Sjrq.Text = new DateTime(1970, 1, 1).AddSeconds(user.SJRQ).ToString();
            C_JJYS.Text = user.JYYS;
            C_SHYS.Text = user.SHYS;
            C_bgrq.Text = new DateTime(1970, 1, 1).AddSeconds(user.BGRQ).ToString();
            C_xy_bxb.Text = user.XY_bxb;
            C_xy_hxb.Text = user.XY_hxb;
            C_xy_xhdb.Text = user.XY_xhdb;
            C_xy_xxb.Text = user.XY_xxb;
            C_ny_nhxbjs.Text = user.NY_nhxbjs;
            C_ny_nbxbjs.Text = user.NY_nbxbjs;
            C_ny_wfljj.Text = user.NY_wfljj;
            C_ny_blgx.Text = user.NY_blgx;
            C_Diagnose.Text = user.diagnose;
            if (user.FB_hxb.Equals("A"))
                fb_hxb_A.IsChecked = true;
            else
                fb_hxb_K.IsChecked = true;

            if (user.FB_bxb.Equals("A"))
                fb_bxb_A.IsChecked = true;
            else
                fb_bxb_K.IsChecked = true;

            if (user.FB_cn.Equals("S"))
                fb_cl_S.IsChecked = true;
            else
                fb_cl_H.IsChecked = true;

            if (user.FB_bn.Equals("S"))
                fb_bl_S.IsChecked = true;
            else
                fb_bl_H.IsChecked = true;

            if (user.FB_jj.Equals("S"))
                fb_jj_S.IsChecked = true;
            else
                fb_jj_H.IsChecked = true;

            Parasites.Children.Clear();

            using (LLBContext db = new LLBContext())
            {
                List<image> images = db.image.Where(x => x.BBBH.Equals(user.BBBH)).ToList();

                foreach (var im in images)
                {
                    FileInfo file = new FileInfo(Environment.CurrentDirectory+ im.PicName);
                    ParasiteImageControl parasite = new ParasiteImageControl();
                    FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                    fileStream.Position = 0;
                    MemoryStream memoryStream = new MemoryStream();
                    fileStream.CopyTo(memoryStream);
                    fileStream.Dispose();
                    memoryStream.Position = 0;
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = memoryStream;
                    imageSource.EndInit();
                    parasite.Picture.Source = imageSource;
                    if (file.FullName.Contains("SampleLib"))
                        parasite.ParasiteName.Text = file.Name.Replace(file.Extension, "").Split('#')[1];
                    else
                        parasite.ParasiteName.Text = file.Name.Replace(file.Extension, "");
                    parasite.FullFileName = file.FullName;
                    parasite.ParasiteCount.Text = im.Count;
                    Parasites.Children.Add(parasite);
                }
            }
        }

        private void Clear()
        {
            C_Name.Text = string.Empty;
            C_Sex_M.IsChecked = true;
            C_Age.Text = string.Empty;
            C_Zyh.Text = string.Empty;
            C_Ks.Text = string.Empty;
            C_Ch.Text = string.Empty;
            C_Sjys.Text = string.Empty;
            C_Sjrq.SelectedDate = DateTime.Today;
            //C_JJYS.Text = string.Empty;
            //C_SHYS.Text = string.Empty;
            C_bgrq.SelectedDate = DateTime.Today;
            C_xy_bxb.Text = string.Empty;
            C_xy_hxb.Text = string.Empty;
            C_xy_xhdb.Text = string.Empty;
            C_xy_xxb.Text = string.Empty;
            C_ny_nhxbjs.Text = string.Empty;
            C_ny_nbxbjs.Text = string.Empty;
            C_ny_wfljj.Text = string.Empty;
            C_ny_blgx.Text = string.Empty;
            C_Diagnose.Text = string.Empty;
            C_Jbmy.IsChecked = false;
            C_My.IsChecked = false;
            C_Xycxcy.IsChecked = false;
            fb_hxb_A.IsChecked = false;
            fb_hxb_K.IsChecked = false;
            fb_bxb_A.IsChecked = false;
            fb_bxb_K.IsChecked = false;
            fb_cl_S.IsChecked = false;
            fb_cl_H.IsChecked = false;

            fb_bl_S.IsChecked = false;
            fb_bl_H.IsChecked = false;
            fb_jj_S.IsChecked = false;
            fb_jj_H.IsChecked = false;
        }

        private void Data_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            user user = grid.SelectedItem as user;
            LoadUser(user);
            List<ParasiteImageControl> images = new List<ParasiteImageControl>();
            foreach (var i in Parasites.Children)
            {
                images.Add(i as ParasiteImageControl);
            }
            //GenerateReport(user, images);
            GenerateReport();
            GC.Collect();
        }

        private void workbeen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems[0] as TabItem).Header.ToString().Equals("报告"))
            {
                GenerateReport();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                
                Dispatcher.BeginInvoke(new Action(() => {
                    AllData.UnselectAll();
                    PrintData.UnselectAll();
                    NoPrintData.UnselectAll();
                }));
            }
        }

        private void Data_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            ResfList();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double size = (double)(this.Height / 1080 * 20);
            this.FontSize = size;
        }

        private void SetFont_Click(object sender, RoutedEventArgs e)
        {
            string HospitalName = ConfigurationManager.AppSettings["Hospital"].ToString();
            string HospitalNameFont = ConfigurationManager.AppSettings["HospitalFont"].ToString();
            string ReportName = ConfigurationManager.AppSettings["Report"].ToString();
            string ReportNameFont = ConfigurationManager.AppSettings["ReportFont"].ToString();
            FontSelector font = new FontSelector(HospitalName, HospitalNameFont, ReportName, ReportNameFont);
            font.ShowDialog();
            GenerateReport();
        }
    }
}
