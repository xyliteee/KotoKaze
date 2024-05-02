using KotoKaze.Dynamic;
using KotoKaze.Static;
using KotoKaze.Windows;
using SevenZip.Compression.LZ;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using XyliteeeMainForm.Views;
using FileControl;


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
            _ = new Mutex(true, "ElectronicNeedleTherapySystem", out bool ret);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if (!ret)
            {

                MessageBox.Show("The applicaiton is already running now","Launching Warring");
                Environment.Exit(0);
            }
            else
            {
                DataInit();
            }
        }
        void DataInit()
        {
            var UpdateLevel = DispatcherPriority.Background;
            StartLoadingWindow s = new();
            BlurEffect? blurEffect = new() { KernelType = KernelType.Gaussian };
            s.BackgroundImage.Effect = blurEffect;

            void UISetting()
            {
                s.LoadinText.Content = "正在进行应用程序设置";
                Dispatcher.Invoke(() =>
                {
                    InitializeComponent();
                    FileManager.WorkDirectory.CreatWorkDirectory();
                    FileManager.WorkDirectory.CreatWorkFile();
                }, UpdateLevel);
                s.leftBar.Value = 10;
                s.rightBar.Value = 10;
                blurEffect.Radius = 20;

                s.LoadinText.Content = "正在初始化信息页面";
                Dispatcher.Invoke(() =>
                {
                    homePage = new();
                    GlobalData.HomePageInstance = homePage;
                }, UpdateLevel);
                s.leftBar.Value = 50;
                s.rightBar.Value = 50;
                blurEffect.Radius = 16;

                s.LoadinText.Content = "正在初始化清理页面";
                Dispatcher.Invoke(() =>
                {
                    cleanPage = new();
                    GlobalData.CleanPageInstance = cleanPage;
                }, UpdateLevel);
                s.leftBar.Value = 60;
                s.rightBar.Value = 60;
                blurEffect.Radius = 10;

                s.LoadinText.Content = "正在初始化测试页面";
                Dispatcher.Invoke(() =>
                {
                    PCTestPage = new();
                    GlobalData.PCTestPageInstance = PCTestPage;
                }, UpdateLevel);
                s.leftBar.Value = 70;
                s.rightBar.Value = 70;
                blurEffect.Radius = 6;

                s.LoadinText.Content = "正在初始化工具页面";
                Dispatcher.Invoke(() =>
                {
                    toolsPage = new();
                    GlobalData.ToolsPageInstance = toolsPage;
                }, UpdateLevel);
                s.leftBar.Value = 80;
                s.rightBar.Value = 80;
                blurEffect.Radius = 4;

                s.LoadinText.Content = "正在初始化设置页面";
                Dispatcher.Invoke(() =>
                {
                    settingPage = new();
                    GlobalData.SettingPageInstance = settingPage;
                }, UpdateLevel);
                s.leftBar.Value = 90;
                s.rightBar.Value = 90;
                blurEffect.Radius = 2;

                s.LoadinText.Content = "即将启动......";
                Dispatcher.Invoke(() =>
                {
                    mainWindow = new(homePage, cleanPage, PCTestPage, toolsPage, settingPage);
                    GlobalData.MainWindowInstance = mainWindow;
                }, UpdateLevel);
                s.leftBar.Value = 100;
                s.rightBar.Value = 100;
                blurEffect = null;
            }
            s.Show();
            UISetting();
            s.Close();
            mainWindow.Show();
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            KotoMessageBoxSingle.ShowDialog("发生了致命性错误，已写入日志");
            FileManager.LogManager.LogWrite("Unknown Error", (e.ExceptionObject as Exception).Message);
        }
    }

}
