﻿using KotoKaze.Dynamic;
using KotoKaze.Windows;
using Translation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static KotoKaze.Dynamic.ADBINFO;
using FileControl;
using KotoKaze.Static;

namespace KotoKaze.Views.toolsPages.otherPages
{
    /// <summary>
    /// adb.xaml 的交互逻辑
    /// </summary>
    public partial class ADBPage : Page
    {
        private PhoneInfo phoneInfo = new();
        private readonly CMDBackgroundTask APKINSTALLTASK = new();
        private readonly SolidColorBrush XiaomiColor = new BrushConverter().ConvertFrom("#FF6600") as SolidColorBrush;
        private readonly SolidColorBrush defaultColor = new BrushConverter().ConvertFrom("#1F67B3") as SolidColorBrush;
        private readonly SolidColorBrush vivotColor = new BrushConverter().ConvertFrom("#415FFF") as SolidColorBrush;
        public ADBPage()
        {
            InitializeComponent();
            GetPhoneInfomation();
        }

        private void SetButtonState(bool flag) 
        {
            intallButton.IsEnabled = flag;
        } 

        private void GetPhoneInfomation() 
        {
            ConnectButotn.IsEnabled = false;
            Mask.Visibility = Visibility.Visible;
            Animations.ImageTurnRound(SettingIcon,true,2);
            BrandBorder.Background = defaultColor;
            Task.Run(() => 
            {
                phoneInfo = ADBINFO.GetPhoneInfomation();
                Dispatcher.Invoke(() =>
                {
                    if (!phoneInfo.isConnected)
                    {
                        KotoMessageBoxSingle.ShowDialog("连接设备出错，未能读取到设备信息。请记得打开手机的USB调试模式");
                        Mask.Visibility = Visibility.Visible;
                        Animations.ImageTurnRound(SettingIcon, false);
                        SetButtonState(false);
                        installDescripition.Content = "请连接设备";
                    }
                    else
                    {
                        SetButtonState(true);
                        Mask.Visibility = Visibility.Collapsed;
                        Animations.ImageTurnRound(SettingIcon, false);
                        if (TranslationRules.phoneModel.TryGetValue(phoneInfo.name.ToUpper(), out string? modelName))
                        {
                            deviceBrand.Content = modelName;
                        }
                        else
                        {
                            deviceBrand.Content = phoneInfo.name;
                        }
                        deviceName.Content = "工厂代号：" + phoneInfo.brand + " " + phoneInfo.model;
                        deviceResolutionRatio.Content = $"设备分辨率：{phoneInfo.resolutionRatio}";
                        deviceDPI.Content = $"设备DPI：{phoneInfo.DPI}";
                        deviceAndroidID.Content = $"设备安卓ID：{phoneInfo.androidID}";
                        deviceAndroidVersion.Content = $"设备安卓版本：{phoneInfo.androidVersion}";
                        deviceMemTotal.Content = $"设备总内存：{phoneInfo.memTotal} KB || {Math.Round(double.Parse(phoneInfo.memTotal) / 1024.0 / 1024.0, 2)} GB";

                        if (phoneInfo.brand.ToUpper().Equals("XIAOMI"))
                        {
                            BrandBorder.Background = XiaomiColor;
                        }
                        else if (phoneInfo.brand.ToUpper().Equals("VIVO")) 
                        {
                            BrandBorder.Background = vivotColor;
                        }
                        installDescripition.Content = "点击以选择APK文件进行安装";
                    }
                    ConnectButotn.IsEnabled = true;
                });
            });
            
        }
        private void ConnectButotn_Click(object sender, RoutedEventArgs e)
        {
            GetPhoneInfomation();
        }
        private void UpdataFiles(object sender, RoutedEventArgs e) 
        {
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                DefaultExt = ".apk",
                DefaultDirectory = FileManager.WorkDirectory.backupDirectory,
                Filter = "APK Files (*.apk)|*.apk"
            };
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string selectedFilePath = dlg.FileName;
                InstallAPK(selectedFilePath,APKINSTALLTASK);
            }
        }
    }
}
