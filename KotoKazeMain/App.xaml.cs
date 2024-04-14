using KotoKaze.Dynamic;
using KotoKaze.Static;
using KotoKaze.Windows;
using SevenZip.Compression.LZ;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using XyliteeeMainForm.Views;


namespace XyliteeeMainForm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
#pragma warning disable CS8602
#pragma warning disable CS8604
#pragma warning disable CS8618
    public partial class App : Application
    {
        private homePage homePage;
        private cleanPage cleanPage;
        private PCTestPage PCTestPage;
        private toolsPage toolsPage;
        private settingPage settingPage;
        private MainWindow mainWindow;
        public App()
        {
            var UpdateLevel = DispatcherPriority.Background;
            StartLoadingWindow s = new();
            void DataInit()
            {
                s.LoadinText.Content = "正在进行应用程序设置";
                Dispatcher.Invoke(() =>
                {
                    InitializeComponent();
                    FileManager.WorkDirectory.CreatWorkDirectory();
                    FileManager.WorkDirectory.CreatWorkFile();
                }, UpdateLevel);
                s.progressBar.Width = 120;

                s.LoadinText.Content = "正在初始化信息页面";
                Dispatcher.Invoke(() =>
                {
                    homePage = new();
                    GlobalData.HomePageInstance = homePage;
                }, UpdateLevel);
                s.progressBar.Width = 410;

                s.LoadinText.Content = "正在初始化清理页面";
                Dispatcher.Invoke(() =>
                {
                    cleanPage = new();
                    GlobalData.CleanPageInstance = cleanPage;
                }, UpdateLevel);
                s.progressBar.Width = 460;

                s.LoadinText.Content = "正在初始化测试页面";
                Dispatcher.Invoke(() =>
                {
                    PCTestPage = new();
                    GlobalData.PCTestPageInstance = PCTestPage;
                }, UpdateLevel);
                s.progressBar.Width = 580;

                s.LoadinText.Content = "正在初始化工具页面";
                Dispatcher.Invoke(() =>
                {
                    toolsPage = new();
                    GlobalData.ToolsPageInstance = toolsPage;
                }, UpdateLevel);
                s.progressBar.Width = 610;

                s.LoadinText.Content = "正在初始化设置页面";
                Dispatcher.Invoke(() =>
                {
                    settingPage = new();
                    GlobalData.SettingPageInstance = settingPage;
                }, UpdateLevel);
                s.progressBar.Width = 640;

                s.LoadinText.Content = "即将启动......";
                Dispatcher.Invoke(() =>
                {
                    mainWindow = new(homePage, cleanPage, PCTestPage, toolsPage, settingPage);
                    GlobalData.MainWindowInstance = mainWindow;
                }, UpdateLevel);
                s.progressBar.Width = 720;
            }
            s.Show();
            DataInit();
            s.Close();
            mainWindow.Show();
        }
    }

}
