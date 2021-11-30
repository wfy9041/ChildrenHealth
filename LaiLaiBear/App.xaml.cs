using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LaiLaiBear
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        ILogger logger = LogManager.GetLogger("NLog.config");
        public App()
        {
            // 在异常由应用程序引发但未进行处理时发生。主要指的是UI线程。
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //  当某个异常未被捕获时出现。主要指的是非UI线程
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //可以记录日志并转向错误bug窗口友好提示用户
            if (e.ExceptionObject is System.Exception)
            {
                Exception ex = (System.Exception)e.ExceptionObject;
                MessageBox.Show(ex.Message);
                logger.Error(ex);
            }
        }
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //可以记录日志并转向错误bug窗口友好提示用户
            e.Handled = true;
            MessageBox.Show(e.Exception.Message);
            logger.Error(e.Exception);

        }
        private void ParasiteDelete_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            var cm = mi.Parent as ContextMenu;
            ParasiteImageControl box = cm.PlacementTarget as ParasiteImageControl;
            List<WrapPanel> wrapPanels = FindVisualParent<WrapPanel>(box);
            wrapPanels[0].Children.Remove(box);
            if (box.FullFileName.Contains("SampleLib"))
            {
                FileInfo fileInfo = new FileInfo(box.FullFileName);
                using (LLBContext context = new LLBContext())
                {
                    image image = context.image.FirstOrDefault(x => x.PicName.Contains(fileInfo.Name));
                    if (image != null)
                    {
                        context.image.Remove(image);
                        context.SaveChanges();
                    }
                }
                fileInfo.Delete();
            }
        }

        /// <summary>
        /// 利用VisualTreeHelper寻找指定依赖对象的父级对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> FindVisualParent<T>(DependencyObject obj) where T : DependencyObject
        {
            try
            {
                List<T> TList = new List<T> { };
                DependencyObject parent = VisualTreeHelper.GetParent(obj);
                if (parent != null && parent is T)
                {
                    TList.Add((T)parent);
                    List<T> parentOfParent = FindVisualParent<T>(parent);
                    if (parentOfParent != null)
                    {
                        TList.AddRange(parentOfParent);
                    }
                }
                else if (parent != null)
                {
                    List<T> parentOfParent = FindVisualParent<T>(parent);
                    if (parentOfParent != null)
                    {
                        TList.AddRange(parentOfParent);
                    }
                }
                return TList;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return null;
            }
        }

        private void UserDelete_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            var cm = mi.Parent as ContextMenu;
            DataGrid grid = cm.PlacementTarget as DataGrid;
            user current = grid.SelectedItem as user;
            using (LLBContext llb = new LLBContext())
            {
                List<image> image = llb.image.Where(x => x.BBBH.Equals(current.BBBH)).ToList();
                user user = llb.user.FirstOrDefault(x => x.ID == current.ID);
                llb.user.Remove(user);
                llb.image.RemoveRange(image);
                llb.SaveChanges();
            }
        }

        private void Skin_Click(object sender, RoutedEventArgs e)
        {
            SkinSelector skinSelector = new SkinSelector();
            skinSelector.ShowDialog();
        }
    }
}
