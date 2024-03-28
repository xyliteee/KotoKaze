using KotoKaze.Dynamic;
using KotoKaze.Windows;
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
using static KotoKaze.Dynamic.ADBINFO;

namespace KotoKaze.Views.toolsPages.otherPages
{
    /// <summary>
    /// adb.xaml 的交互逻辑
    /// </summary>
    public partial class ADBPage : Page
    {
        private PhoneInfo phoneInfo = new();
        private readonly SolidColorBrush XiaomiColor = new BrushConverter().ConvertFrom("#FF6600") as SolidColorBrush;
        private readonly SolidColorBrush defaultColor = new BrushConverter().ConvertFrom("#1F67B3") as SolidColorBrush;

        public ADBPage()
        {
            InitializeComponent();
            GetPhoneInfomation();
        }

        private void GetPhoneInfomation() 
        {
            ConnectButotn.IsEnabled = false;
            deviceName.Content = "设备名称：索引中";
            deviceBrand.Content = "索引中";
            deviceResolutionRatio.Content = "设备分辨率：索引中";
            deviceDPI.Content = "设备DPI：索引中";
            deviceAndroidID.Content = "设备安卓ID：索引中";
            deviceAndroidVersion.Content = "设备安卓版本：索引中";
            deviceMemTotal.Content = "设备总内存：索引中";
            BrandBorder.Background = defaultColor;
            Task.Run(() => 
            {
                phoneInfo = ADBINFO.GetPhoneInfomation();
                Dispatcher.Invoke(() =>
                {
                    if (!phoneInfo.isConnected)
                    {
                        KotoMessageBoxSingle.ShowDialog("连接设备出错，未能读取到设备信息。请记得打开手机的USB调试模式");
                        deviceName.Content = "";
                        deviceBrand.Content = "连接中断";
                        deviceResolutionRatio.Content = "";
                        deviceDPI.Content = "";
                        deviceAndroidID.Content = "";
                        deviceAndroidVersion.Content = "";
                        deviceMemTotal.Content = "";
                    }
                    else 
                    {
                        deviceName.Content = $"设备名称：{phoneInfo.name}";
                        deviceBrand.Content = phoneInfo.brand + " " + phoneInfo.model;
                        deviceResolutionRatio.Content = $"设备分辨率：{phoneInfo.resolutionRatio}";
                        deviceDPI.Content = $"设备DPI：{phoneInfo.DPI}";
                        deviceAndroidID.Content = $"设备安卓ID：{phoneInfo.androidID}";
                        deviceAndroidVersion.Content = $"设备安卓版本：{phoneInfo.androidVersion}";
                        deviceMemTotal.Content = $"设备总内存：{phoneInfo.memTotal} KB || {Math.Round(double.Parse(phoneInfo.memTotal) / 1024.0 / 1024.0,2)} GB";
                        if (phoneInfo.brand.Equals("Xiaomi")) 
                        {
                            BrandBorder.Background = XiaomiColor;
                        }
                    }
                    ConnectButotn.IsEnabled = true;
                });
            });
            
        }

        private void ConnectButotn_Click(object sender, RoutedEventArgs e)
        {
            GetPhoneInfomation();
        }
    }
}
