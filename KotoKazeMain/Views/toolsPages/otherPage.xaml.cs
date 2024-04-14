using KotoKaze.Dynamic;
using KotoKaze.Static;
using KotoKaze.Views.toolsPages.BCDPages;
using KotoKaze.Views.toolsPages.otherPages;
using KotoKaze.Windows;
using static KotoKaze.Dynamic.BackgroundTaskList;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static KotoKaze.Static.FileManager;
using static KotoKaze.Dynamic.Network.Downloader;
using System.Diagnostics;


namespace KotoKaze.Views.toolsPages
{
    /// <summary>
    /// otherPage.xaml 的交互逻辑
    /// </summary>
    public partial class otherPage : Page
    {
        public readonly NetworkBackgroundTask ADBINSTALL = new(new Network.Downloader("ADB Download")) {Title = "ADB组件下载"};
        public otherPage()
        {
            InitializeComponent();
        }

        private async void ADBbutton_Click(object sender, RoutedEventArgs e)
        {
            string adbPath = Path.Combine(WorkDirectory.BinDirectory, "platform-tools\\adb.exe");//ADB组件的路径
            if (Path.Exists(adbPath)) //如果存在ADB组件，则导航到ADB页面
            {
                GlobalData.ToolsPageInstance.secondActionFrame.Navigate(new ADBPage());
                GlobalData.ToolsPageInstance.ShowSecondPage(true);
                GlobalData.MainWindowInstance.backButton.Visibility = Visibility.Visible;
                return;
            }
            if (IsTaskRunning(ADBINSTALL) || IsTaskRunning(GlobalData.ToolsPageInstance.otherPage.ADBINSTALL)) //检查当前任务列表是否包含ADB安装任务
            {
                KotoMessageBoxSingle.ShowDialog("正在下载ADB组件，耐心等待......");
                return;
            }

            var rr = KotoMessageBox.ShowDialog("ADB组件缺失,是否下载?");
            if (!rr.IsYes){ return; }
            GlobalData.TasksList.Add(ADBINSTALL);
            ADBINSTALL.Description = "正在准备下载......";
            string adbZipFile = Path.Combine(WorkDirectory.softwareTempDirectory, "adb.zip");//ADB压缩包的下载路径
            int isDownloadSuccessful;
            if (!Path.Exists(adbZipFile)) //如果在下载路径发现了该文件，跳过下载
            {
                for (int times = 0; times < 5; times++)
                {
                    if (times != 0)
                    {
                        ADBINSTALL.Description = $"下载出错，第{times}次重试";
                        await Task.Delay(1000);
                    }
                    string ADBINSTALLUrl = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip";
                    isDownloadSuccessful = await ADBINSTALL.downloader.DownloadAsync(ADBINSTALLUrl, adbZipFile);
                    if (isDownloadSuccessful == isSucessful) break;
                    if (isDownloadSuccessful == isCancle) return;
                    if (times == 4 && isDownloadSuccessful == isError)
                    {
                        ADBINSTALL.SetFinal(() => { KotoMessageBoxSingle.ShowDialog("下载出错，建议检查网络状况"); });
                        return;
                    }
                }
            }
            ADBINSTALL.Description = "正在解压....";
            bool isSuccessful = await UnzipAsync(adbZipFile,WorkDirectory.BinDirectory,"ADB UnZip");
            if (!isSuccessful || !Path.Exists(adbPath))
            {
                ADBINSTALL.SetFinal(() => { KotoMessageBoxSingle.ShowDialog("解压错误，已生成日志文件"); });
                return;
            }
            ADBINSTALL.SetFinal(() => { KotoMessageBoxSingle.ShowDialog("ADB组件安装完成"); });

        }
    }
}
