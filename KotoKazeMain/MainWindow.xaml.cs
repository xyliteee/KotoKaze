using KotoKaze.Static;
using static KotoKaze.Static.FileManager.IniManager;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TestContent;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using XyliteeeMainForm.Views;
using KotoKaze.Windows;
using KotoKaze.Dynamic;
using System.Windows.Threading;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
namespace XyliteeeMainForm
{
    public partial class MainWindow : Window
    {

        private homePage homePage;
        private cleanPage cleanPage;
        private PCTestPage PCTestPage;
        private toolsPage toolsPage;
        private settingPage settingPage;
        private Page currentPage;
        private readonly Button[] buttons = new Button[5];
        private readonly SolidColorBrush blueTextColor = new BrushConverter().ConvertFrom("#1F67B3") as SolidColorBrush;


        public MainWindow()
        {

            StartLoadingWindow s = new();
            void DataInit()
            {
                Dispatcher.Invoke(() =>
                {
                    InitializeComponent();
                    GlobalData.MainWindowInstance = this;
                }, DispatcherPriority.Loaded);
                s.progressBar.Width = 120;

                s.LoadinText.Content = "正在初始化信息页面";
                Dispatcher.Invoke(() =>
                {
                    homePage = new();
                    GlobalData.HomePageInstance = homePage;
                }, DispatcherPriority.Loaded);
                s.progressBar.Width = 410;

                s.LoadinText.Content = "正在初始化清理页面";
                Dispatcher.Invoke(() =>
                {
                    cleanPage = new();
                    GlobalData.CleanPageInstance = cleanPage;
                }, DispatcherPriority.Loaded);
                s.progressBar.Width = 460;

                s.LoadinText.Content = "正在初始化测试页面";
                Dispatcher.Invoke(() =>
                {
                    PCTestPage = new();
                    GlobalData.PCTestPageInstance = PCTestPage;
                }, DispatcherPriority.Loaded);
                s.progressBar.Width = 580;

                s.LoadinText.Content = "正在初始化工具页面";
                Dispatcher.Invoke(() =>
                {
                    toolsPage = new();
                    GlobalData.ToolsPageInstance = toolsPage;
                }, DispatcherPriority.Loaded);
                s.progressBar.Width = 610;

                s.LoadinText.Content = "正在初始化设置页面";
                Dispatcher.Invoke(() =>
                {
                    settingPage = new();
                    GlobalData.SettingPageInstance = settingPage;
                }, DispatcherPriority.Loaded);
                s.progressBar.Width = 640;

                s.LoadinText.Content = "正在进行最终设置";
                Dispatcher.Invoke(() =>
                {
                    actionFrame.Navigate(homePage);
                    currentPage = (Page)actionFrame.Content;
                    actionFrame.Navigated += PageChanged;
                    buttons[0] = homePageButton;
                    buttons[1] = cleanPageButton;
                    buttons[2] = PCTestPageButton;
                    buttons[3] = toolsPageButton;
                    buttons[4] = settingPageButton;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    FileManager.WorkDirectory.CreatWorkDirectory();
                    FileManager.WorkDirectory.CreatWorkFile();
                }, DispatcherPriority.Loaded);
                s.progressBar.Width = 720;
            }
            s.Show();
            DataInit();
            s.Close();
            Show();
            CheckFirstUse();
        }
        private static async void CheckFirstUse() 
        {
            string value = IniFileRead("Application.ini", "SETTING", "ISFIRST_USE");
            if (value == "FALSE") return;
            else if (value == "TRUE")
            {
                //await Task.Delay(1000);
                KotoMessageBoxSingle.ShowDialog("看起来您是第一次使用本软件，但我实际上也没什么好说的");
                IniFileWrite("Application.ini", "SETTING", "ISFIRST_USE", "FALSE");
            }
            else if (value == "ERROR")
            {
                await Task.Delay(1000);
                KotoMessageBoxSingle.ShowDialog("检测到文件被修改，已重置");
                IniFileSetDefault("Application.ini");
            }
        }

        private void PageChanged(object sender, NavigationEventArgs e) 
        {
            Animations.ChangeOP(actionFrame, 0, 1, 0.3);
            Animations.FrameMoving(actionFrame, 50);
            currentPage = (Page)actionFrame.Content;

            if (currentPage is toolsPage && toolsPage.secondActionFrame.Visibility == Visibility.Visible) //如果当前页面是工具页面且二级页面处于开启状态
            {
                backButton.Visibility = Visibility.Visible;//将返回按钮显示
            }
            else { backButton.Visibility = Visibility.Collapsed; }
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)//窗口移动事件
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var windowMode = ResizeMode;
                if (ResizeMode != ResizeMode.NoResize)
                {
                    ResizeMode = ResizeMode.NoResize;
                }
                UpdateLayout();

                DragMove();
                e.Handled = true;

                if (ResizeMode != windowMode)
                {
                    ResizeMode = windowMode;
                }
                UpdateLayout();
            }
        }
        private void MinButton_Click(object sender, RoutedEventArgs e)//窗口最小化按钮
        {
            WindowState = WindowState.Minimized;
        }
        private void ShutdownButton_Click(object sender, RoutedEventArgs e)//关闭按钮
        {
            GlobalData.IsRunning = false;
            Close();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e) //返回反扭
        {
            if (currentPage is toolsPage && toolsPage.secondActionFrame.Visibility == Visibility.Visible)//如果当前页面是工具页面且二级页面处于开启状态
            {
                toolsPage.secondActionFrame.Visibility = Visibility.Collapsed;//按钮可以将二级页面关闭
            }
            backButton.Visibility = Visibility.Collapsed;//然后按钮关闭
        }
        private void SetCurrentPageColor()//设置当前页面的颜色                                                                             //根据当前页面设置对应按钮颜色为蓝色
        {

            Dictionary<object, (Button,Image, SolidColorBrush, BitmapImage)> mappings = new()
            {           
                { homePage, (homePageButton,homeImage ,blueTextColor, BitMapImages.homeBlue) },
                { cleanPage, (cleanPageButton,cleanImage, blueTextColor, BitMapImages.cleanBlue) },
                { PCTestPage, (PCTestPageButton,testImage ,blueTextColor, BitMapImages.testBlue) },
                { toolsPage, (toolsPageButton,toolsImage ,blueTextColor, BitMapImages.toolsBlue) },
                { settingPage, (settingPageButton,settingImage ,blueTextColor, BitMapImages.settingBlue) },
            };

            if (mappings.TryGetValue(currentPage, out var mapping))//通过定义的映射设置对应的文字颜色和图片
            {
                mapping.Item1.Foreground = mapping.Item3;
                mapping.Item2.Source = mapping.Item4;
            }
        }

        private void SetAllImageWhite()//将所有DOCK栏的图片设置为白色                                                                                             //把所有图片设白
        {
            homeImage.Source = BitMapImages.homeWhite;
            cleanImage.Source = BitMapImages.cleanWhite;
            testImage.Source = BitMapImages.testWhite;
            toolsImage.Source = BitMapImages.toolsWhite;
            settingImage.Source = BitMapImages.settingWhite;
        }

        private void DockButtonEnter(object sender, MouseEventArgs e)//鼠标悬浮在DOCK按钮事件                                                               //鼠标悬浮时的行为
        {
            Button button = (Button)sender;
            foreach (Button b in buttons)                                                                                           //所有按钮文字设白
            {
                b.Foreground = Brushes.White;//将所有按钮设置为白色
            }   
            SetAllImageWhite();//将所有图片设置为白色                                                                    //所有图片设白
            button.Foreground = blueTextColor;//将悬浮的按钮设置为蓝色                                                                                      //指针悬浮的按钮文字设蓝

            switch (button.Name)//将对应图片设置为蓝色                                                                 //指针悬浮的图片设蓝
            {
                case "homePageButton":
                    homeImage.Source = BitMapImages.homeBlue;
                    break;
                case "cleanPageButton":
                    cleanImage.Source = BitMapImages.cleanBlue;
                    break;
                case "PCTestPageButton":
                    testImage.Source = BitMapImages.testBlue;
                    break;
                case "toolsPageButton":
                    toolsImage.Source = BitMapImages.toolsBlue;
                    break;
                case "settingPageButton":
                    settingImage.Source = BitMapImages.settingBlue;
                    break;
            }
            SetCurrentPageColor(); //把当前页面颜色设置为蓝色                                                                                                 //恢复当前页面对应按钮的颜色
        }

        private void DockButtonLeave(object sender, MouseEventArgs e)                                                               //鼠标离开的行为
        {
            
            foreach (Button b in buttons)
            {
                b.Foreground = Brushes.White;
            }
            SetAllImageWhite();
            SetCurrentPageColor();
        }

        private void DockButtonAction(object sender, RoutedEventArgs e)                                                             //鼠标点击的行为
        {
            Button button = (Button)sender;

            foreach (Button b in buttons)
            {
                b.Foreground = Brushes.White;
                b.IsEnabled = true;
            }

            button.IsEnabled = false;
            SetAllImageWhite();

            button.Foreground = new BrushConverter().ConvertFrom("#1F67B3") as SolidColorBrush;

            switch (button.Name)
            {
                case "homePageButton":
                    Animations.PageSilderMoveing(PageSilder, 50);
                    actionFrame.Navigate(homePage);
                    homeImage.Source = BitMapImages.homeBlue;
                    break;
                case "cleanPageButton":
                    Animations.PageSilderMoveing(PageSilder, 90);
                    actionFrame.Navigate(cleanPage);
                    cleanImage.Source = BitMapImages.cleanBlue;
                    break;
                case "PCTestPageButton":
                    Animations.PageSilderMoveing(PageSilder, 130);
                    actionFrame.Navigate(PCTestPage);
                    testImage.Source = BitMapImages.testBlue;
                    break;
                case "toolsPageButton":
                    Animations.PageSilderMoveing(PageSilder, 170);
                    actionFrame.Navigate(toolsPage);
                    toolsImage.Source = BitMapImages.toolsBlue;
                    break;
                case "settingPageButton":
                    Animations.PageSilderMoveing(PageSilder, 420);
                    actionFrame.Navigate(settingPage);
                    settingImage.Source = BitMapImages.settingBlue;
                    break;

            }
        }

        private void OtherZoneButton_Click(object sender, RoutedEventArgs e)
        {
            TasksListShowZone.Visibility = Visibility.Collapsed;
        }

        private void TaskListButton_Click(object sender, RoutedEventArgs e)
        {
            TasksListShowZone.Visibility = Visibility.Visible;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {

        }
    }
    static class BitMapImages 
    {
        public readonly static BitmapImage homeWhite = new(new Uri("pack://application:,,,/image/icons/Home.png"));
        public readonly static BitmapImage cleanWhite = new(new Uri("pack://application:,,,/image/icons/Clean.png"));
        public readonly static BitmapImage testWhite = new(new Uri("pack://application:,,,/image/icons/PCTest.png"));
        public readonly static BitmapImage toolsWhite = new(new Uri("pack://application:,,,/image/icons/Tools.png"));
        public readonly static BitmapImage settingWhite = new(new Uri("pack://application:,,,/image/icons/Setting.png"));

        public readonly static BitmapImage homeBlue = new(new Uri("pack://application:,,,/image/icons/Home_b.png"));
        public readonly static BitmapImage cleanBlue = new(new Uri("pack://application:,,,/image/icons/Clean_b.png"));
        public readonly static BitmapImage testBlue = new(new Uri("pack://application:,,,/image/icons/PCTest_b.png"));
        public readonly static BitmapImage toolsBlue = new(new Uri("pack://application:,,,/image/icons/Tools_b.png"));
        public readonly static BitmapImage settingBlue = new(new Uri("pack://application:,,,/image/icons/Setting_b.png"));

        public readonly static BitmapImage TestImage = new(new Uri("pack://application:,,,/image/Header/Test2.png"));
    }
}