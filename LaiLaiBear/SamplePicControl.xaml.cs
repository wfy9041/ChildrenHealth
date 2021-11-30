using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaiLaiBear
{
    /// <summary>
    /// SamplePicControl.xaml 的交互逻辑
    /// </summary>
    public partial class SamplePicControl : UserControl
    {
        public string PicFileName;
        public Stream stream;
        public SamplePicControl()
        {
            InitializeComponent();
        }
    }
}
