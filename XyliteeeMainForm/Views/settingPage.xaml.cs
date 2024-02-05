using System.Windows.Controls;

namespace XyliteeeMainForm.Views
{
    /// <summary>
    /// settingPage.xaml 的交互逻辑
    /// </summary>
    public partial class settingPage : Page
    {
        private readonly MainWindow mainWindow;
        public settingPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }
    }
}
