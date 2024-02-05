using System.Windows.Controls;

namespace XyliteeeMainForm.Views
{
    /// <summary>
    /// toolsPage.xaml 的交互逻辑
    /// </summary>
    public partial class toolsPage : Page
    {
        private readonly MainWindow mainWindow;
        public toolsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }
    }
}
