using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LaiLaiBear
{
    /// <summary>
    /// SkinSelector.xaml 的交互逻辑
    /// </summary>
    public partial class SkinSelector : Window
    {
        public SkinSelector()
        {
            InitializeComponent();
            Brush Hospitalbrush;
            BrushConverter HospitalbrushConverter = new BrushConverter();
            Hospitalbrush = (Brush)HospitalbrushConverter.ConvertFromString(ConfigurationManager.AppSettings["SkinColor"].ToString());
            SkinColor.Fill = Hospitalbrush;
            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                LanguageSpecificStringDictionary fontdics = font.FamilyNames;
                //判断该字体是不是中文字体
                if (fontdics.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
                {
                    string fontfamilyname = null;
                    if (fontdics.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out fontfamilyname))
                    {
                       
                        SkinFont.Items.Add(fontfamilyname);
                    }
                }
            }
            SkinFont.SelectedItem = ConfigurationManager.AppSettings["SkinFont"].ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
           
            config.AppSettings.Settings["SkinFont"].Value = SkinFont.SelectedItem.ToString();
            config.AppSettings.Settings["SkinColor"].Value = "#" + SkinColor.Fill.ToString().Substring(3, 6);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Color_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(colorDialog.Color);
                SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromRgb(sb.Color.R, sb.Color.G, sb.Color.B));
                (sender as Rectangle).Fill = solidColorBrush;
            }
        }
    }
}
