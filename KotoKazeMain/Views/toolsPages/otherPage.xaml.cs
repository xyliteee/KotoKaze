using KotoKaze.Dynamic;
using KotoKaze.Static;
using KotoKaze.Views.toolsPages.BCDPages;
using KotoKaze.Views.toolsPages.otherPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace KotoKaze.Views.toolsPages
{
    /// <summary>
    /// otherPage.xaml 的交互逻辑
    /// </summary>
    public partial class otherPage : Page
    {
        public otherPage()
        {
            InitializeComponent();
        }

        private void ADBbutton_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.ToolsPageInstance.secondActionFrame.Navigate(new ADBPage());
            GlobalData.ToolsPageInstance.ShowSecondPage(true);
            GlobalData.MainWindowInstance.backButton.Visibility = Visibility.Visible;
        }
    }
}
