﻿using KotoKaze.Static;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XyliteeeMainForm.Views;

namespace XyliteeeMainForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        private readonly homePage homePage;
        private readonly cleanPage cleanPage;
        private readonly PCTestPage PCTestPage;
        private readonly toolsPage toolsPage;
        private readonly settingPage settingPage;
        private readonly Button[] buttons = new Button[5];
        private readonly SolidColorBrush blueTextColor = new BrushConverter().ConvertFrom("#1F67B3") as SolidColorBrush;
        public MainWindow()
        {
            InitializeComponent();
            homePage = new(this);
            cleanPage = new(this);
            PCTestPage = new(this);
            toolsPage = new(this);
            settingPage = new(this);
            actionFrame.Navigate(homePage);
            buttons[0] = homePageButton;
            buttons[1] = cleanPageButton;
            buttons[2] = PCTestPageButton;
            buttons[3] = toolsPageButton;
            buttons[4] = settingPageButton;
            WindowStyle = WindowStyle.SingleBorderWindow;
            
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
            Close();
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

            if (mappings.TryGetValue(actionFrame.Content, out var mapping))
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
                    Animations.PageSilderMoveing(PageSilder, 90);
                    actionFrame.Navigate(cleanPage);
                    cleanImage.Source = BitMapImages.cleanBlue;
                    break;
                case "PCTestPageButton":
                    Animations.PageSilderMoveing(PageSilder, 150);
                    actionFrame.Navigate(PCTestPage);
                    testImage.Source = BitMapImages.testBlue;
                    break;
                case "toolsPageButton":
                    Animations.PageSilderMoveing(PageSilder, 210);
                    actionFrame.Navigate(toolsPage);
                    toolsImage.Source = BitMapImages.toolsBlue;
                    break;
                case "settingPageButton":
                    Animations.PageSilderMoveing(PageSilder, 270);
                    actionFrame.Navigate(settingPage);
                    settingImage.Source = BitMapImages.settingBlue;
                    break;

            }
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
    }
}