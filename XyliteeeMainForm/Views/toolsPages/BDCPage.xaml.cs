using KotoKaze.Static;
using KotoKaze.Views.toolsPages.BCDPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XyliteeeMainForm;

namespace KotoKaze.Views.toolsPages
{
    /// <summary>
    /// BDCPage.xaml 的交互逻辑
    /// </summary>
    public partial class BDCPage : Page
    {
        public BDCPage()
        {
            InitializeComponent();
        }

        private void QuerySystemBootInformationButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.MainWindowInstance.actionFrame.Navigate(new QuerySystemBootInformation());
        }
    }
}
