using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace _3D_ver03
{
    /// <summary>
    /// Media.xaml 的交互逻辑
    /// </summary>
    public partial class Media : Window
    {
        public Media()
        {
            InitializeComponent();

            ExternalFunctions.ConfigDisable(3);

            tmrProgress = new DispatcherTimer();
            //设置计时器的时间间隔为1秒
            tmrProgress.Interval = new TimeSpan(0, 0, 1);
            //计时器触发事件处理
            tmrProgress.Tick += SetDisplayMessage;
            SetImageForMediaElement();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ExternalFunctions.ConfigEnable(3);
        }

        //将录像的第一帧作为播放前MediaElement显示的录像截图
        public void SetImageForMediaElement()
        {
            videoScreenMediaElement.Play();
            videoScreenMediaElement.Pause();
            videoScreenMediaElement.Position = TimeSpan.Zero;
        }

        //计时器，定时更新进度条和播放时间
        private DispatcherTimer tmrProgress = new DispatcherTimer();

        //计时器触发事件处理
        private void SetDisplayMessage(Object sender, System.EventArgs e)
        {
            if (videoScreenMediaElement.NaturalDuration.HasTimeSpan)
            {

                TimeSpan currentPositionTimeSpan = videoScreenMediaElement.Position;

                string currentPosition = string.Format("{0:00}:{1:00}:{2:00}", (int)currentPositionTimeSpan.TotalHours, currentPositionTimeSpan.Minutes, currentPositionTimeSpan.Seconds);

                TimeSpan totaotp = videoScreenMediaElement.NaturalDuration.TimeSpan;
                string totalPostion = string.Format("{0:00}:{1:00}:{2:00}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds);

                currentPositionTime.Text = currentPosition;
                playProgressSlider.Value = videoScreenMediaElement.Position.TotalSeconds;

            }
        }



        //当完成媒体加载时发生
        private void videoScreenMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            playProgressSlider.Minimum = 0;
            playProgressSlider.Maximum = videoScreenMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            TimeSpan totaotp = videoScreenMediaElement.NaturalDuration.TimeSpan;
            videoAllTime.Text = "/" + string.Format("{0:00}:{1:00}:{2:00}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds);
            currentPositionTime.Text = "00:00:00";


        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            //启动计时器
            if (!tmrProgress.IsEnabled)
            {
                tmrProgress.Start();
            }
            videoScreenMediaElement.Play();

        }

        //在鼠标拖动Thumb的过程中记录其值。
        private TimeSpan ts = new TimeSpan();
        private void playProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts = TimeSpan.FromSeconds(e.NewValue);
            string currentPosition = string.Format("{0:00}:{1:00}:{2:00}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);

            currentPositionTime.Text = currentPosition;

        }

        //当拖动Thumb的鼠标放开时，从指定时间开始播放
        private void playProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            videoScreenMediaElement.Position = ts;
        }

        private void Media_Closed(object sender, EventArgs e)
        {
            if (tmrProgress.IsEnabled)
            {
                tmrProgress.Stop();
            }

        }

        private void videoScreenMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoScreenMediaElement.Position = TimeSpan.Zero;
            videoScreenMediaElement.Stop();
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            videoScreenMediaElement.Pause();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            videoScreenMediaElement.Stop();
        }

        private void playImage_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Image image = sender as Image;
            Uri uri = new Uri(@"Images/pause.png");
            image.Source = new BitmapImage(uri);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 23;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 20;
        }

        private void playImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 28;
        }

        private void playImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 25;
        }
    }
}
