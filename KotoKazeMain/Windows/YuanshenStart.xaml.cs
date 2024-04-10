using KotoKaze.Static;
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
using System.Windows.Shapes;

namespace KotoKaze.Windows
{
    /// <summary>
    /// YuanshenStart.xaml 的交互逻辑
    /// </summary>
    public partial class YuanshenStart : Window
    {
        public YuanshenStart()
        {
            InitializeComponent();
            ChangeOP();
        }
        private async void ChangeOP()
        {
            Animations.ChangeOP(YuanshenImage, 0, 1, 3);
            await Task.Delay(3000);
            Animations.ChangeOP(YuanshenImage, 1, 0, 3);
            await Task.Delay(3000);
            Close();
        }
    }
}
