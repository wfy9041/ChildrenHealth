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
using System.Windows.Shapes;
using WPFMediaKit.DirectShow.Controls;

namespace LaiLaiBear
{
    /// <summary>
    /// SamplePic.xaml 的交互逻辑
    /// </summary>
    public partial class SamplePic : Window
    {
        private readonly List<string> SampleFiles = new List<string>();
        private string BBBH;
        public delegate void SamplePicture(List<string> PicName);
        public event SamplePicture SampleEvent;
        private int Count;
        public SamplePic(string BBBH,int count)
        {
            InitializeComponent();
            this.BBBH = BBBH;
            Count = 2-count;
            if (MultimediaUtil.VideoInputDevices.Any())
            {
                cobVideoSource.ItemsSource = MultimediaUtil.VideoInputNames;
                cobVideoSource.SelectedIndex = 0;
                cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex];
            }
        }

        private void cobVideoSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cobVideoSource.SelectedIndex < 0)
                return;
            cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex];
        }

        private void cameraCaptureElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SamplePicPanel.Children.Count < Count)
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)cameraCaptureElement.ActualWidth, (int)cameraCaptureElement.ActualWidth, 96, 96, PixelFormats.Default);
                bmp.Render(cameraCaptureElement);
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                MemoryStream ms = new MemoryStream();
                encoder.Save(ms);
                ms.Position = 0;
                SamplePicControl samplePicControl = new SamplePicControl();
                samplePicControl.stream = ms;
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = ms;
                imageSource.EndInit();
                samplePicControl.Picture.Source = imageSource;
                SamplePicPanel.Children.Add(samplePicControl);

                //string fileName = Environment.CurrentDirectory + "\\SampleLib\\" + $"{BBBH}#@#{Guid.NewGuid().ToString().Substring(0, 8)}.jpg";
                //FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                //encoder.Save(fileStream);
                //fileStream.Flush();
                //fileStream.Close();
                //SampleFiles.Add(fileName);
                //SamplePicControl samplePicControl = new SamplePicControl();
                //samplePicControl.Picture.Source = new BitmapImage(new Uri(fileName, UriKind.Absolute));
                //samplePicControl.PicFileName = fileName;
                //SamplePicPanel.Children.Add(samplePicControl);
            }
            else
            {
                MessageBox.Show($"最多{Count}张", "提示");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pic in SamplePicPanel.Children)
            {
                SamplePicControl spc = pic as SamplePicControl;
                if (string.IsNullOrWhiteSpace(spc.PicNote.Text))
                {
                    MessageBox.Show("请先输入图片名称", "错误");
                    return;
                }
            }


            foreach (var child in SamplePicPanel.Children)
            {
                SamplePicControl spc = child as SamplePicControl;
                if (!System.IO.File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\SampleLib\\"))
                {
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}\\SampleLib\\");
                }
                string PicFile = $"{Environment.CurrentDirectory}\\SampleLib\\{BBBH}#{spc.PicNote.Text}#{Guid.NewGuid().ToString().Substring(0, 8)}.jpg";
                FileStream fileStream = new FileStream(PicFile, FileMode.OpenOrCreate);
                spc.stream.Position = 0;
                spc.stream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
                SampleFiles.Add(PicFile);
            }
            SampleEvent(SampleFiles);
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
