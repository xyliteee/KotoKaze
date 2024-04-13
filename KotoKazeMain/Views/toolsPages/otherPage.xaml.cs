using KotoKaze.Dynamic;
using KotoKaze.Static;
using KotoKaze.Views.toolsPages.BCDPages;
using KotoKaze.Views.toolsPages.otherPages;
using KotoKaze.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static KotoKaze.Static.FileManager;


namespace KotoKaze.Views.toolsPages
{
    /// <summary>
    /// otherPage.xaml 的交互逻辑
    /// </summary>
    public partial class otherPage : Page
    {
        private readonly NetWorkBackgroundTask ADBDOWNLOAD = new(new Network.Downloader()) {Title = "ADB组件下载"};
        public otherPage()
        {
            InitializeComponent();
        }

        private async void ADBbutton_Click(object sender, RoutedEventArgs e)
        {
            string adbPath = Path.Combine(WorkDirectory.BinDirectory, "platform-tools\\adb.exe");
            if (Path.Exists(adbPath)) 
            {
                GlobalData.ToolsPageInstance.secondActionFrame.Navigate(new ADBPage());
                GlobalData.ToolsPageInstance.ShowSecondPage(true);
                GlobalData.MainWindowInstance.backButton.Visibility = Visibility.Visible;
                return;
            }
            if (GlobalData.TasksList.Contains(ADBDOWNLOAD)) 
            {
                KotoMessageBoxSingle.ShowDialog("正在下载ADB组件，耐心等待......");
                return;
            }

            var rr = KotoMessageBox.ShowDialog("ADB组件缺失,是否下载？\n这可能需要特殊的网络环境。");
            if (!rr.IsYes){ return; }
            GlobalData.TasksList.Add(ADBDOWNLOAD);
            ADBDOWNLOAD.Description = "正在准备下载......";
            string adbZipFile = Path.Combine(WorkDirectory.softwareTempDirectory, "adb.zip");
            bool isSuccessful;
            for (int times = 0; times < 5; times++) 
            {
                if (times != 0)
                {
                    ADBDOWNLOAD.Description = $"下载出错，第{times}次重试";
                    await Task.Delay(1000);
                }
                isSuccessful = await ADBDOWNLOAD.downloader.DownloadAsync("https://dl.google.com/android/repository/platform-tools-latest-windows.zip", adbZipFile);
                if (isSuccessful) {break;}
                if (times == 4) 
                {
                    ADBDOWNLOAD.SetFinal(() => { KotoMessageBoxSingle.ShowDialog("下载错误，已生成日志文件"); });
                    return;
                }
            }
            ADBDOWNLOAD.Description = "正在解压....";
            isSuccessful = await UnzipAsync(adbZipFile,WorkDirectory.BinDirectory);
            if (!isSuccessful || !Path.Exists(adbPath))
            {
                ADBDOWNLOAD.SetFinal(() => { KotoMessageBoxSingle.ShowDialog("解压错误，已生成日志文件"); });
                ADBDOWNLOAD.SetFinal();
                return;
            }
            ADBDOWNLOAD.SetFinal(() => { KotoMessageBoxSingle.ShowDialog("ADB组件安装完成"); });

        }
    }
}
