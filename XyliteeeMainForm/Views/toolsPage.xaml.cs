﻿using KotoKaze.Static;
using KotoKaze.Views.toolsPages;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace XyliteeeMainForm.Views
{
    /// <summary>
    /// toolsPage.xaml 的交互逻辑
    /// </summary>
    public partial class toolsPage : Page
    {
        private readonly MainWindow mainWindow;
        private readonly SolidColorBrush blueColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0C5E0"));
        public toolsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            TipsBox.Content = "这里包含了一些系统功能相关的工具";
        }

        private void SetButtonState(Button button) 
        {
            WindowsButton.IsEnabled = true;
            DismButton.IsEnabled = true;
            BCDButton.IsEnabled = true;
            OtherButton.IsEnabled = true;
            button.IsEnabled = false;
        }


        private void DismButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetButtonState((Button) sender);
            Animations.ButtonSilderMoveing(Silder, 195);
            TipsBox.Content = "这里包含了一些基于DISM的工具，可能具有不可恢复的危险性";
        }

        private void WindowsButton_Click(object sender, System.Windows.RoutedEventArgs e) 
        {
            SetButtonState((Button)sender);
            Animations.ButtonSilderMoveing(Silder,35);
            TipsBox.Content = "这里包含了一些系统功能相关的工具";
        }

        private void BCDButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetButtonState((Button)sender);
            Animations.ButtonSilderMoveing(Silder, 355);
            TipsBox.Content = "这里包含了一些基于BCD的引导工具，可能具有不可恢复的危险性";
            ToolsNegate.Navigate(new BDCPage());
        }

        private void OtherButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetButtonState((Button)sender);
            Animations.ButtonSilderMoveing(Silder, 515);
            TipsBox.Content = "这里包含了一些其他未归类的工具";
        }
    }
}
