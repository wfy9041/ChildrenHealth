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
using System.Windows.Shapes;

namespace LaiLaiBear
{
    /// <summary>
    /// ParasiteList.xaml 的交互逻辑
    /// </summary>
    public partial class ParasiteListWindow : Window
    {
        public delegate void AddParasiteDelegate(string FullFilePath);
        public event AddParasiteDelegate AddParasite;

        public ParasiteListWindow()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(".\\img\\");
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            InitializeComponent();
            foreach(FileInfo file in fileInfos)
            {
                ParasiteImageControl parasite = new ParasiteImageControl();
                parasite.Picture.Source = new BitmapImage(new Uri(file.FullName, UriKind.Absolute));
                parasite.ParasiteName.Text = file.Name.Replace(file.Extension,"");
                parasite.ParasiteCount.Visibility = Visibility.Hidden;
                parasite.Uint.Visibility = Visibility.Hidden;
                parasite.FullFileName = file.FullName;
                parasite.MouseDoubleClick += Parasite_MouseDoubleClick;
                PicList.Children.Add(parasite);
            }
        }



        private void Parasite_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ParasiteImageControl parasite = sender as ParasiteImageControl;
            parasite.ParasiteCount.Visibility = Visibility.Visible;
            parasite.Uint.Visibility = Visibility.Visible;
            AddParasite(parasite.FullFileName);
            this.Close();
        }
    }
}
