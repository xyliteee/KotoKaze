﻿using KotoKaze.Static;
using static KotoKaze.Static.FileManager.IniManager;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using XyliteeeMainForm.Views;
using KotoKaze.Windows;
using HandyControl.Interactivity;
using KotoKaze.Dynamic;
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
            async Task LoadingUI() 
            {
                InitializeComponent();
                Hide();

                s.LoadinText.Content = "正在初始化信息页面";
                s.progressBar.Width = 120;
                await Task.Delay(100);
                homePage = new();

                s.LoadinText.Content = "正在初始化清理页面";
                s.progressBar.Width = 410;
                await Task.Delay(100);
                cleanPage = new();

                s.LoadinText.Content = "正在初始化测试页面";
                s.progressBar.Width = 460;
                await Task.Delay(100);
                PCTestPage = new();

                s.LoadinText.Content = "正在初始化工具页面";
                s.progressBar.Width = 580;
                await Task.Delay(100);
                toolsPage = new();

                s.LoadinText.Content = "正在初始化设置页面";
                s.progressBar.Width = 610;
                await Task.Delay(100);
                settingPage = new();

                s.LoadinText.Content = "正在进行最终设置";
                s.progressBar.Width = 680;
                await Task.Delay(100);
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
                GlobalData.MainWindowInstance = this;
                CheckFirstUse();
                CheckTasksList();
            }
            async void Start() 
            {
                s.Show();
                await LoadingUI();
                s.progressBar.Width = 720;  
                s.Close();
                Show();
            }
            Start();
        }

        private async void CheckTasksList()
        {
            void CreatSingleCard(BackgroundTask backgroundTask,int index) 
            {
                // 创建一个Canvas
                Canvas myCanvas = new()
                {
                    Width = 390,
                    Height = 40
                };
                Canvas.SetTop(myCanvas,index * 40);
                // 创建第一个Label
                Label title = new()
                {
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Width = 390,
                    Height = 30,
                    Content = backgroundTask.title,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };

                myCanvas.Children.Add(title);
                Label description = new()
                {
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Width = 390,
                    Height = 20,
                    Content = backgroundTask.description,
                    Padding = new Thickness(10, 0, 10, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Foreground = Brushes.Gray
                };
                Canvas.SetTop(description, 20);
                myCanvas.Children.Add(description);
                ScorllCanvas.Children.Add(myCanvas);
            }
            List<BackgroundTask> lastTasksList = [];

            while (GlobalData.IsRunning)
            {
                if (GlobalData.TasksList != lastTasksList)
                {
                    ScorllCanvas.Children.Clear();
                    if (GlobalData.TasksList.Count == 0)
                    {
                        TaskListMessage.Content = "无任务";
                        Label label = new()
                        {
                            Content = "没有任何任务存在",
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                            Width = 390,
                            Height = 30
                        };
                        ScorllCanvas.Children.Add(label);
                        Canvas.SetTop(label, 50);
                    }
                    else
                    {
                        TaskListMessage.Content = $"{GlobalData.TasksList.Count}个任务正在执行";
                        for (int index = 0; index < GlobalData.TasksList.Count; index++)
                        {
                            CreatSingleCard(GlobalData.TasksList[index], index);
                        }
                        ScorllCanvas.Height = GlobalData.TasksList.Count * 40;
                    }
                    lastTasksList = [.. GlobalData.TasksList];
                }
                await Task.Delay(200);
            }
        }


        private async void CheckFirstUse() 
        {
            string value = IniFileRead("Application.ini", "SETTING", "ISFIRST_USE");
            if (value == "FALSE") return;
            else if (value == "TRUE")
            {
                await Task.Delay(1000);
                KotoMessageBoxSingle.ShowDialog("看起来您是第一次使用本软件，但我实际上也没什么好说的");
                IniFileWrite("Application.ini", "SETTING", "ISFIRST_USE", "FALSE");
            }
            else if (value == "ERROR")
            {
                await Task.Delay(1000);
                KotoMessageBoxSingle.ShowDialog("BYD别改文件");
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

        private void DragWindow(object sender, MouseButtonEventArgs e)
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

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.IsRunning = false;
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) 
        {
            if (currentPage is toolsPage && toolsPage.secondActionFrame.Visibility == Visibility.Visible)//如果当前页面是工具页面且二级页面处于开启状态
            {
                toolsPage.secondActionFrame.Visibility = Visibility.Collapsed;//按钮可以将二级页面关闭
            }
            backButton.Visibility = Visibility.Collapsed;//然后按钮关闭
        }

        private void SetCurrentPageColor()                                                                                          //根据当前页面设置对应按钮颜色为蓝色
        {

            Dictionary<object, (Button,Image, SolidColorBrush, BitmapImage)> mappings = new()
            {           
                { homePage, (homePageButton,homeImage ,blueTextColor, BitMapImages.homeBlue) },
                { cleanPage, (cleanPageButton,cleanImage, blueTextColor, BitMapImages.cleanBlue) },
                { PCTestPage, (PCTestPageButton,testImage ,blueTextColor, BitMapImages.testBlue) },
                { toolsPage, (toolsPageButton,toolsImage ,blueTextColor, BitMapImages.toolsBlue) },
                { settingPage, (settingPageButton,settingImage ,blueTextColor, BitMapImages.settingBlue) },
            };

            if (mappings.TryGetValue(currentPage, out var mapping))
            {
                mapping.Item1.Foreground = mapping.Item3;
                mapping.Item2.Source = mapping.Item4;
            }
        }

        private void SetAllImageWhite()                                                                                             //把所有图片设白
        {
            homeImage.Source = BitMapImages.homeWhite;
            cleanImage.Source = BitMapImages.cleanWhite;
            testImage.Source = BitMapImages.testWhite;
            toolsImage.Source = BitMapImages.toolsWhite;
            settingImage.Source = BitMapImages.settingWhite;
        }

        private void DockButtonEnter(object sender, MouseEventArgs e)                                                               //鼠标悬浮时的行为
        {
            Button button = (Button)sender;
            foreach (Button b in buttons)                                                                                           //所有按钮文字设白
            {
                b.Foreground = Brushes.White;
            }   
            SetAllImageWhite();                                                                                                     //所有图片设白
            button.Foreground = blueTextColor;                                                                                      //指针悬浮的按钮文字设蓝

            switch (button.Name)                                                                                                    //指针悬浮的图片设蓝
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
            SetCurrentPageColor();                                                                                                  //恢复当前页面对应按钮的颜色
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
                    Animations.PageSilderMoveing(PageSilder, 30);
                    actionFrame.Navigate(homePage);
                    homeImage.Source = BitMapImages.homeBlue;
                    break;
                case "cleanPageButton":
                    Animations.PageSilderMoveing(PageSilder, 70);
                    actionFrame.Navigate(cleanPage);
                    cleanImage.Source = BitMapImages.cleanBlue;
                    break;
                case "PCTestPageButton":
                    Animations.PageSilderMoveing(PageSilder, 110);
                    actionFrame.Navigate(PCTestPage);
                    testImage.Source = BitMapImages.testBlue;
                    break;
                case "toolsPageButton":
                    Animations.PageSilderMoveing(PageSilder, 150);
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
    }
    public class BitMapImages 
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