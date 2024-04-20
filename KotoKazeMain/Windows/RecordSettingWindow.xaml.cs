using KotoKaze.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static FileControl.FileManager.IniManager;

namespace KotoKaze.Windows
{
    /// <summary>
    /// RecordSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RecordSettingWindow : Window
    {
        public RecordSettingWindow()
        {
            InitializeComponent();
            SetFreqListDefault();
        }
        private void SetFreqListDefault() 
        {
            if (GlobalData.RefreshTime == 2) FreqList.SelectedIndex = 0;
            else if(GlobalData.RefreshTime == 1) FreqList.SelectedIndex = 1;
            else if (GlobalData.RefreshTime == 0.5) FreqList.SelectedIndex = 2;

        }
        public static void ShowSettingPage() 
        {
            RecordSettingWindow RecordSettingWindow = new() 
            {
                Owner = GlobalData.MainWindowInstance
            };
            KotoMessageBox.RunShow(RecordSettingWindow);
            RecordSettingWindow.ShowDialog();
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            KotoMessageBox.RunClose(this);
            base.OnClosing(e);
        }

        private void FreqList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = ((ComboBox)sender).SelectedIndex; 
            switch (selectedIndex)
            {
                case 0:
                    GlobalData.RefreshTime = 2;
                    break;
                case 1:
                    GlobalData.RefreshTime = 1;
                    break;
                case 2:
                    GlobalData.RefreshTime = 0.5;
                    break;
            }
            IniFileWrite("Application.ini", "SETTING", "REFRESH_TIME", GlobalData.RefreshTime.ToString());
        }
    }
}
