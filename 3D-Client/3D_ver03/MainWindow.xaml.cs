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
using System.Threading;
using System.Windows.Threading;
using System.Media;
using System.Diagnostics;
using System.Management;


namespace _3D_ver03
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static double[] config;
		public static double[] WrongTime;
		public static double ShouldersWidth;
        public static int SetTime = Default.SetTimeMinute;
        public static bool isUseDefaultTunes = Default.isUseDefaultTunes;
        public static int TuneFrequency = Default.TuneFrequency;
        public static string tuneFileAddress;
        public static bool isShowDetails = false;
        public static bool isNeedToRefresh = false;
        private Thread t;
        private Thread playbeep;
        private Thread playfiletune;
        //private bool flag1 = false;
        //private bool flag2 = false;
        private DispatcherTimer countDownTimer;
        private ProcessCount processCount;
        private object thislock;

        public event CountDownHandler CountDown;
        public delegate bool CountDownHandler();

        ASCIIEncoding asciiEncoding = new ASCIIEncoding();
        public MainWindow()
        {
            InitializeComponent();
            thislock = new object();
            config = new double[ExternalFunctions.GetConfigSize()];
            ExternalFunctions.GetConfig(config);
			WrongTime = new double[ExternalFunctions.GetMaxTimeSize()];
			ExternalFunctions.GetMaxTime(WrongTime);

            m_GridCameraWindow.Children.Add(new CameraWindow());


            ThreadStart ts = new ThreadStart(DisplayInfos);
            t = new Thread(ts);
            t.Start();

            playbeep = new Thread(new ParameterizedThreadStart(_Beep));

            playfiletune = new Thread(new ParameterizedThreadStart(_PlayTune));


            countDownTimer = new DispatcherTimer();
            countDownTimer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
            countDownTimer.Tick += new EventHandler(timer_Tick);

            HourArea.Text = Default.SetTimeHour.ToString();
            MinuteArea.Text = Default.SetTimeMinute.ToString();
            SecondArea.Text = Default.SetTimeSecond.ToString();

            Int32 hour = Convert.ToInt32(HourArea.Text);
            Int32 minute = Convert.ToInt32(MinuteArea.Text);
            Int32 second = Convert.ToInt32(SecondArea.Text);

            //处理倒计时的类
            processCount = new ProcessCount(hour * 3600 + minute * 60 + second);
            CountDown += new CountDownHandler(processCount.ProcessCountDown);

            ExternalFunctions.ConfigDisable(4);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (OnCountDown())
            {
                HourArea.Text = processCount.GetHour();
                MinuteArea.Text = processCount.GetMinute();
                SecondArea.Text = processCount.GetSecond();
            }
            else
            {
                using (SoundPlayer player = new SoundPlayer())
                {
                    player.SoundLocation = System.Environment.CurrentDirectory + "//Notify_Timeup.wav";
                    player.Play();
                }
                countDownTimer.Stop();
				ExternalFunctions.OnKeyDown((uint)asciiEncoding.GetBytes("S")[0]);
				MessageBox.Show("时间到了，请起身休息一下:)");
            }
        }

        public bool OnCountDown()
        {
            if (CountDown != null)
                return CountDown();

            return false;
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            //t.Abort();
            //playbeep.Abort();
            //playfiletune.Abort();
            Process pss = Process.GetCurrentProcess();
            Process[] myProcesses = Process.GetProcessesByName(pss.ProcessName);
            foreach (Process p in myProcesses)
            {
                p.Kill();
            }
        }

        private void DisplayInfos()
        {
            while (true)
            {
                Dispatcher.Invoke((Action)delegate
                {
                    m_TxtBlcShouldersDistance.Text = ExternalFunctions.infos[0].ToString();
                    m_TxtFurthestDistance.Text = ExternalFunctions.infos[1].ToString();
                    m_TxtBlcHeadAngleFrontNBack.Text = ExternalFunctions.infos[2].ToString();
                    m_TxtBlcHeadAngleLeftNRight.Text = ExternalFunctions.infos[3].ToString();
                    m_TxtBlcWaistAngle.Text = ExternalFunctions.infos[4].ToString();
                    m_TxtBlcHeadAngleSide.Text = ExternalFunctions.infos[5].ToString();
                    int errorcode = (int)Math.Floor(ExternalFunctions.infos[6] + 0.1);
                    string errorstring = null;
                    switch (errorcode)
                    {
                        case -1:
                            errorstring = "等待中";
                            break;
                        case 0:
                            errorstring = "正确";
                            break;
                        case 1:
                            errorstring = "头部前倾(正面)";
                            break;
                        case 2:
                            errorstring = "头部右倾(正面)";
                            break;
                        case 3:
                            errorstring = "头部左倾(正面)";
                            break;
                        case 4:
                            errorstring = "身体前倾(侧面)";
                            break;
                        case 5:
                            errorstring = "头部前倾(侧面)";
                            break;
                    }
                    if (errorcode != 0 && errorcode != -1)
                    {
                        if (isUseDefaultTunes)
                        {
                            if (playbeep.ThreadState == System.Threading.ThreadState.Stopped)
                                playbeep = new Thread(new ParameterizedThreadStart(_Beep));
                            if (playbeep.ThreadState != System.Threading.ThreadState.Running && playbeep.ThreadState != System.Threading.ThreadState.WaitSleepJoin)
                                playbeep.Start(TuneFrequency);
                        }
                        else
                        {
                            if (playfiletune.ThreadState == System.Threading.ThreadState.Stopped)
                                playfiletune = new Thread(new ParameterizedThreadStart(_PlayTune));
                            if (playfiletune.ThreadState != System.Threading.ThreadState.Running && playfiletune.ThreadState != System.Threading.ThreadState.WaitSleepJoin)
                                playfiletune.Start(tuneFileAddress);
                        }
                    }
                    m_TxtBlcWrongPosMessage.Text = errorstring;
                    int positioncode = (int)Math.Floor(ExternalFunctions.infos[7] + 0.1);
                    string positionstring = null;
                    if (positioncode == 0)
                        positionstring = "侧面";
                    else
                        positionstring = "正面";
                    m_TxtBlcPosState.Text = positionstring;
                });
            }
        }

        private void _Beep(object frequency)
        {
            lock (thislock)
            {
                Settings.Beep((int)frequency, 500);
                Thread.Sleep(5000);
            }
        }

        private void _PlayTune(object tuneAddress)
        {
            lock (thislock)
            {
                Settings.PlayTune(tuneAddress);
                Thread.Sleep(5000);
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (m_BtnStartTimer.Content.ToString().Trim() == "开始计时")
            {
                processCount.TotalSecond = SetTime * 60;
                HourArea.Text = processCount.GetHour();
                MinuteArea.Text = processCount.GetMinute();
                SecondArea.Text = processCount.GetSecond();
            }
            if (isNeedToRefresh)
            {
                if (isShowDetails)
                    m_GridHiddenInfo.Visibility = Visibility.Visible;
                else
                    m_GridHiddenInfo.Visibility = Visibility.Collapsed;
                isNeedToRefresh = false;
            }
        }

        private void m_BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings wm = new Settings();
            wm.Show();
        }

        private void m_BtnMedia_Click(object sender, RoutedEventArgs e)
        {
            Media wm = new Media();
            wm.Show();
        }

        private void m_BtnHidePage_Click(object sender, RoutedEventArgs e)
        {
            if (m_BtnHidePage.Content.ToString().Trim() == "隐藏画面")
            {
                ExternalFunctions.ConfigDisable(3);
                m_BtnHidePage.Content = "显示画面";
            }
            else
            {
                ExternalFunctions.ConfigEnable(3);
                m_BtnHidePage.Content = "隐藏画面";
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ExternalFunctions.OnKeyDown((uint)asciiEncoding.GetBytes(e.Key.ToString())[e.Key.ToString().Length - 1]);
        }

        private void m_BtnActivateSideMonitor_Click(object sender, RoutedEventArgs e)
        {
            if (m_BtnActivateSideMonitor.Content.ToString().Trim() == "启动侧身监测")
            {
                ExternalFunctions.ConfigEnable(4);
                m_BtnActivateSideMonitor.Content = "暂停侧身监测";
            }
            else
            {
                ExternalFunctions.ConfigDisable(4);
                m_BtnActivateSideMonitor.Content = "启动侧身监测";
            }
        }

        private void m_BtnStartTimer_Click(object sender, RoutedEventArgs e)
        {
            if (m_BtnStartTimer.Content.ToString().Trim() == "开始计时")
            {
                m_BtnStartTimer.Content = "暂停计时";
                countDownTimer.Start();
            }
            else
            {
                m_BtnStartTimer.Content = "开始计时";
                countDownTimer.Stop();
            }
        }
    }
}
