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
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Media;
using System.Threading;

namespace _3D_ver03
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Window
    {
        private double ShouldersDistanceMin;
        private double ShouldersDistanceMax;
        private double ShouldersWidth;
        private double HeadAngleFrontNBack;
        private double HeadAngleLeftNRightMin;
        private double HeadAngleLeftNRightMax;
        private double WaistAngle;
        private double HeadAngleSideMin;
        private double HeadAngleSideMax;
        private int Timer;
        private int SetTime;
        private bool isSelecting = false;
        private static string fileName = null;
        int frequency = Default.TuneFrequency;
		private double wrongtime;
        [DllImport("kernel32.dll", EntryPoint = "Beep")]
        public static extern int Beep(int dwFreq, int dwDuration);

        public Settings()
        {
            InitializeComponent();

            ExternalFunctions.ConfigDisable(3); //先隐藏画面，防止占用过多资源

            m_LstBoxTune.SelectedIndex = (frequency - 1000) / 500;

            m_TxtBoxShouldersDistanceMin.Text = MainWindow.config[0].ToString();
            m_TxtBoxShouldersDistanceMax.Text = MainWindow.config[1].ToString();
            m_TxtBoxShouldersWidth.Text = MainWindow.ShouldersWidth.ToString();
            m_TxtBoxHeadAngleFrontNBack.Text = MainWindow.config[2].ToString();
            m_TxtBoxHeadAngleLeftNRightMin.Text = MainWindow.config[3].ToString();
            m_TxtBoxHeadAngleLeftNRightMax.Text = MainWindow.config[4].ToString();
			m_TxtBoxWaistAngle.Text = MainWindow.config[5].ToString();
            m_TxtBoxHeadAngleSideMin.Text = MainWindow.config[6].ToString();
            m_TxtBoxHeadAngleSideMax.Text = MainWindow.config[7].ToString();
            m_TxtBoxTimer.Text = MainWindow.config[8].ToString();
            m_TxtBoxSetTime.Text = MainWindow.SetTime.ToString();
            m_ChcBoxDetail.IsChecked = MainWindow.isShowDetails;
            m_ChcBoxUseFile.IsChecked = !MainWindow.isUseDefaultTunes;
			m_TxtBoxSetWrongTime.Text = MainWindow.WrongTime[0].ToString();
            if ((bool)m_ChcBoxUseFile.IsChecked)
            {
                m_LstBoxTune.IsEnabled = false;
                m_TxtBoxTuneFile.IsEnabled = true;
            }
            else
            {
                m_LstBoxTune.IsEnabled = true;
                m_TxtBoxTuneFile.IsEnabled = false;
                fileName = null;
                m_TxtBoxTuneFile.Text = "";
            }
            m_TxtBoxTuneFile.Text = MainWindow.tuneFileAddress;
            isSelecting = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ExternalFunctions.ConfigEnable(3);
        }

        private void m_BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            m_TxtBoxShouldersDistanceMin.BorderBrush = Brushes.Gray;
            m_TxtBoxShouldersDistanceMax.BorderBrush = Brushes.Gray;
            m_TxtBoxShouldersWidth.BorderBrush = Brushes.Gray;
            m_TxtBoxHeadAngleFrontNBack.BorderBrush = Brushes.Gray;
            m_TxtBoxHeadAngleLeftNRightMin.BorderBrush = Brushes.Gray;
            m_TxtBoxHeadAngleLeftNRightMax.BorderBrush = Brushes.Gray;
            m_TxtBoxWaistAngle.BorderBrush = Brushes.Gray;
            m_TxtBoxHeadAngleSideMin.BorderBrush = Brushes.Gray;
            m_TxtBoxHeadAngleSideMax.BorderBrush = Brushes.Gray;
            m_TxtBoxTimer.BorderBrush = Brushes.Gray;
            m_TxtBoxSetTime.BorderBrush = Brushes.Gray;
			m_TxtBoxSetWrongTime.BorderBrush = Brushes.Gray;
            bool isInputWrong = false;
            if (!double.TryParse(m_TxtBoxShouldersDistanceMin.Text.Trim(), out ShouldersDistanceMin))
            {
                m_TxtBoxShouldersDistanceMin.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!double.TryParse(m_TxtBoxShouldersDistanceMax.Text.Trim(), out ShouldersDistanceMax))
            {
                m_TxtBoxShouldersDistanceMax.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!double.TryParse(m_TxtBoxShouldersWidth.Text.Trim(), out ShouldersWidth))
            {
                m_TxtBoxShouldersWidth.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!double.TryParse(m_TxtBoxHeadAngleFrontNBack.Text.Trim(), out HeadAngleFrontNBack))
            {
                m_TxtBoxHeadAngleFrontNBack.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!double.TryParse(m_TxtBoxHeadAngleLeftNRightMin.Text.Trim(), out HeadAngleLeftNRightMin))
            {
                m_TxtBoxHeadAngleLeftNRightMin.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!double.TryParse(m_TxtBoxHeadAngleLeftNRightMax.Text.Trim(), out HeadAngleLeftNRightMax))
            {
                m_TxtBoxHeadAngleLeftNRightMax.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!double.TryParse(m_TxtBoxWaistAngle.Text.Trim(), out WaistAngle))
            {
                m_TxtBoxWaistAngle.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!double.TryParse(m_TxtBoxHeadAngleSideMin.Text.Trim(), out HeadAngleSideMin))
            {
                m_TxtBoxHeadAngleSideMin.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!double.TryParse(m_TxtBoxHeadAngleSideMax.Text.Trim(), out HeadAngleSideMax))
            {
                m_TxtBoxHeadAngleSideMax.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!int.TryParse(m_TxtBoxTimer.Text.Trim(), out Timer))
            {
                m_TxtBoxTimer.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
            if (!int.TryParse(m_TxtBoxSetTime.Text.Trim(), out SetTime))
            {
                m_TxtBoxSetTime.BorderBrush = Brushes.Red;
                isInputWrong = true;
            }
			if (!double.TryParse(m_TxtBoxSetWrongTime.Text.Trim(), out wrongtime))
			{
				m_TxtBoxSetWrongTime.BorderBrush = Brushes.Red;
				isInputWrong = true;
			}
            if (isInputWrong)
            {
                MessageBox.Show("输入错误\n请重试！");
                return;
            }
			
            MainWindow.config[0] = ShouldersDistanceMin;
            MainWindow.config[1] = ShouldersDistanceMax;
            MainWindow.ShouldersWidth = ShouldersWidth;
            MainWindow.config[2] = HeadAngleFrontNBack;
            MainWindow.config[3] = HeadAngleLeftNRightMin;
            MainWindow.config[4] = HeadAngleLeftNRightMax;
            MainWindow.config[5] = WaistAngle;
            MainWindow.config[6] = HeadAngleSideMin;
            MainWindow.config[7] = HeadAngleSideMax;
            MainWindow.config[8] = Timer;
			for (int i = 0; i < ExternalFunctions.GetMaxTimeSize();i++ )
				MainWindow.WrongTime[i] = wrongtime;
			ExternalFunctions.SetMaxTime(MainWindow.WrongTime);
            ExternalFunctions.SetConfig(MainWindow.config);
            MainWindow.SetTime = SetTime;
            if (!(bool)m_ChcBoxUseFile.IsChecked)
            {
                MainWindow.TuneFrequency = frequency;
                MainWindow.isUseDefaultTunes = true;
            }
            else
            {
                MainWindow.tuneFileAddress = fileName;
                if (fileName != "" && fileName != null)
                    MainWindow.isUseDefaultTunes = false;
            }
            MainWindow.isShowDetails = (bool)m_ChcBoxDetail.IsChecked;
            MainWindow.tuneFileAddress = m_TxtBoxTuneFile.Text;
            MainWindow.isNeedToRefresh = true;



            this.Close();
        }

        private void m_BtnGoback_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void m_LstBoxTuneSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (isSelecting)
            {
                frequency = 1000 + m_LstBoxTune.SelectedIndex * 500;
                Beep(frequency, 500);
            }
        }

        private void m_TxtBoxTuneFile_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxTuneFile.Text = "";
            fileName = "";
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Filter = "sound files (*.wav)|*.wav";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                m_TxtBoxTuneFile.Text = fileName;
                Thread t = new Thread(new ParameterizedThreadStart(PlayTune));
                t.Start(fileName);
            }
        }

        private void m_ChcBoxUseFile_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)m_ChcBoxUseFile.IsChecked)
            {
                m_LstBoxTune.IsEnabled = false;
                m_TxtBoxTuneFile.IsEnabled = true;
            }
            else
            {
                m_LstBoxTune.IsEnabled = true;
                m_TxtBoxTuneFile.IsEnabled = false;
                fileName = null;
            }
        }

        public static void PlayTune(object tuneAddress)
        {
            using (SoundPlayer player = new SoundPlayer())
            {
                player.SoundLocation = tuneAddress.ToString();
                player.PlaySync();
            }
        }

        private void m_BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            m_TxtBoxShouldersDistanceMin.Text = Default.ShouldersDistanceMin.ToString();
            m_TxtBoxShouldersDistanceMax.Text = Default.ShouldersDistanceMax.ToString();
            m_TxtBoxShouldersWidth.Text = Default.ShouldersWidth.ToString();
            m_TxtBoxHeadAngleFrontNBack.Text = Default.HeadAngleFrontNBack.ToString();
            m_TxtBoxHeadAngleLeftNRightMin.Text = Default.HeadAngleLeftNRightMin.ToString();
            m_TxtBoxHeadAngleLeftNRightMax.Text = Default.HeadAngleLeftNRightMax.ToString();
            m_TxtBoxWaistAngle.Text = Default.WaistAngle.ToString();
            m_TxtBoxHeadAngleSideMin.Text = Default.HeadAngleSideMin.ToString();
            m_TxtBoxHeadAngleSideMax.Text = Default.HeadAngleSideMax.ToString();
            m_TxtBoxTimer.Text = Default.Timer.ToString();
            m_TxtBoxSetTime.Text = Default.SetTimeMinute.ToString();
			m_TxtBoxSetWrongTime.Text = Default.WrongTime.ToString();
            m_ChcBoxDetail.IsChecked = false;
            isSelecting = true;
            m_TxtBoxTuneFile.Text = "";
            m_ChcBoxUseFile.IsChecked = false;
            m_LstBoxTune.SelectedIndex = 0;
            m_LstBoxTune.IsEnabled = true;
            m_TxtBoxTuneFile.IsEnabled = false;
            fileName = null;
            frequency = Default.TuneFrequency;
            MainWindow.isShowDetails = false;
        }

        private void m_TxtBoxShouldersDistanceMin_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxShouldersDistanceMin.Text = "";
        }

        private void m_TxtBoxShouldersDistanceMax_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxShouldersDistanceMax.Text = "";
        }

        private void m_TxtBoxShouldersWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxShouldersWidth.Text = "";
        }

        private void m_TxtBoxHeadAngleFrontNBack_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxHeadAngleFrontNBack.Text = "";
        }

        private void m_TxtBoxHeadAngleLeftNRightMin_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxHeadAngleLeftNRightMin.Text = "";
        }

        private void m_TxtBoxHeadAngleLeftNRightMax_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxHeadAngleLeftNRightMax.Text = "";
        }

        private void m_TxtBoxWaistAngle_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxWaistAngle.Text = "";
        }

        private void m_TxtBoxHeadAngleSideMin_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxHeadAngleSideMin.Text = "";
        }

        private void m_TxtBoxHeadAngleSideMax_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxHeadAngleSideMax.Text = "";
        }

        private void m_TxtBoxTimer_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxTimer.Text = "";
        }

        private void m_TxtBoxSetTime_GotFocus(object sender, RoutedEventArgs e)
        {
            m_TxtBoxSetTime.Text = "";
        }

        private void m_TxtBoxShouldersDistanceMin_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxShouldersDistanceMin.Text == "")
                m_TxtBoxShouldersDistanceMin.Text = Default.ShouldersDistanceMin.ToString();
        }

        private void m_TxtBoxShouldersDistanceMax_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxShouldersDistanceMax.Text == "")
                m_TxtBoxShouldersDistanceMax.Text = Default.ShouldersDistanceMax.ToString();
        }

        private void m_TxtBoxShouldersWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxShouldersWidth.Text == "")
                m_TxtBoxShouldersWidth.Text = Default.ShouldersWidth.ToString();
        }

        private void m_TxtBoxHeadAngleFrontNBack_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxHeadAngleFrontNBack.Text == "")
                m_TxtBoxHeadAngleFrontNBack.Text = Default.HeadAngleFrontNBack.ToString();
        }

        private void m_TxtBoxHeadAngleLeftNRightMin_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxHeadAngleLeftNRightMin.Text == "")
                m_TxtBoxHeadAngleLeftNRightMin.Text = Default.HeadAngleLeftNRightMin.ToString();
        }

        private void m_TxtBoxHeadAngleLeftNRightMax_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxHeadAngleLeftNRightMax.Text == "")
                m_TxtBoxHeadAngleLeftNRightMax.Text = Default.HeadAngleLeftNRightMax.ToString();
        }

        private void m_TxtBoxWaistAngle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxWaistAngle.Text == "")
                m_TxtBoxWaistAngle.Text = Default.WaistAngle.ToString();
        }

        private void m_TxtBoxHeadAngleSideMin_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxHeadAngleSideMin.Text == "")
                m_TxtBoxHeadAngleSideMin.Text = Default.HeadAngleSideMin.ToString();
        }

        private void m_TxtBoxHeadAngleSideMax_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxHeadAngleSideMax.Text == "")
                m_TxtBoxHeadAngleSideMax.Text = Default.HeadAngleSideMax.ToString();
        }

        private void m_TxtBoxTimer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxTimer.Text == "")
                m_TxtBoxTimer.Text = Default.Timer.ToString();
        }

        private void m_TxtBoxSetTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_TxtBoxSetTime.Text == "")
                m_TxtBoxSetTime.Text = Default.SetTimeMinute.ToString();
        }

		private void m_TxtBoxSetWrongTime_GotFocus(object sender, RoutedEventArgs e)
        {
			m_TxtBoxSetWrongTime.Text = "";        
		}

		private void m_TxtBoxSetWrongTime_LostFocus(object sender, RoutedEventArgs e)
		{
			if (m_TxtBoxSetWrongTime.Text == "")
				m_TxtBoxSetWrongTime.Text = Default.SetTimeMinute.ToString();
		}

    }
}

