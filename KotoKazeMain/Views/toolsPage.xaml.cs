using KotoKaze.Static;
using KotoKaze.Views.toolsPages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XyliteeeMainForm.Views
{
    /// <summary>
    /// toolsPage.xaml 的交互逻辑
    /// </summary>
    public partial class toolsPage : Page
    {
        private readonly SolidColorBrush blueColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0C5E0"));
        public readonly SystemToolsPage systemToolsPage = new();
        public readonly DISMPage DismPage = new();
        public readonly BCDPage BcdPage = new();
        public readonly otherPage otherPage = new();
        public toolsPage()
        {
            InitializeComponent();
            TipsBox.Content = "这里包含了一些系统功能相关的工具";
            ToolsNegate.Navigate(systemToolsPage);
            WindowsButton.IsEnabled = false;
        }

        private void SetButtonState(Button button) 
        {
            WindowsButton.IsEnabled = true;
            DismButton.IsEnabled = true;
            BCDButton.IsEnabled = true;
            OtherButton.IsEnabled = true;
            button.IsEnabled = false;
        }


        private void DismButton_Click(object sender, RoutedEventArgs e)
        {
            SetButtonState((Button) sender);
            Animations.ButtonSilderMoveing(Silder, 195);
            TipsBox.Content = "这里包含了一些基于DISM的工具，可能具有不可恢复的危险性";
            ToolsNegate.Navigate(DismPage);
        }

        private void WindowsButton_Click(object sender, RoutedEventArgs e) 
        {
            SetButtonState((Button)sender);
            Animations.ButtonSilderMoveing(Silder,35);
            TipsBox.Content = "这里包含了一些系统功能相关的工具";
            ToolsNegate.Navigate(systemToolsPage);
        }

        private void BCDButton_Click(object sender, RoutedEventArgs e)
        {
            SetButtonState((Button)sender);
            Animations.ButtonSilderMoveing(Silder, 355);
            TipsBox.Content = "这里包含了一些基于BCD的引导工具，可能具有不可恢复的危险性";
            ToolsNegate.Navigate(BcdPage);
        }

        private void OtherButton_Click(object sender, RoutedEventArgs e)
        {
            SetButtonState((Button)sender);
            Animations.ButtonSilderMoveing(Silder, 515);
            TipsBox.Content = "这里包含了一些其他未归类的工具";
            ToolsNegate.Navigate(otherPage);
        }

        public void ShowSecondPage(bool show) 
        {
            if (show)
            {
                secondActionFrame.Visibility = Visibility.Visible;
            }
            else 
            {
                secondActionFrame.Visibility = Visibility.Collapsed;
            }
        }
    }
}
