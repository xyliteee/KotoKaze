﻿using KotoKaze.Dynamic;
using KotoKaze.Windows;
using Translation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static KotoKaze.Dynamic.ADBINFO;
using static KotoKaze.Dynamic.BackgroundTaskList;
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
        private readonly CMDBackgroundTask ACTIVESCENE = new() { Title = "激活Scene" };
        private readonly CMDBackgroundTask ACTIVESHIZUKU = new() { Title = "激活Shizuku" };
        private readonly SolidColorBrush XiaomiColor = new BrushConverter().ConvertFrom("#FF6600") as SolidColorBrush;
        private readonly SolidColorBrush defaultColor = new BrushConverter().ConvertFrom("#1F67B3") as SolidColorBrush;
        private readonly SolidColorBrush vivotColor = new BrushConverter().ConvertFrom("#415FFF") as SolidColorBrush;
        private bool _isConnected = false;
        private bool IsConnected 
        {
            get => _isConnected;
            set 
            {
                _isConnected = value;
                if (_isConnected) 
                {
                    SetButtonState(true);
                    return;
                }
                SetButtonState(false);
            }
        }
        public ADBPage()
        {
            InitializeComponent();
            GetPhoneInfomation();
            SetButtonState(false);
        }

        private void SetButtonState(bool flag)
        {
            intallButton.IsEnabled = flag;
            SceneButotn.IsEnabled = flag;
            ShizukuButotn.IsEnabled = flag;
        }

        private void GetPhoneInfomation()
        {
            ConnectButotn.IsEnabled = false;
            Mask.Visibility = Visibility.Visible;
            Animations.ImageTurnRound(SettingIcon, true, 2);
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
                        IsConnected = false;
                        installDescripition.Content = "请连接设备";
                    }
                    else
                    {
                        IsConnected = true ;
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
            IsConnected = false;
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
                InstallAPK(selectedFilePath, APKINSTALLTASK);
            }
        }

        private void SceneButotn_Click(object sender, RoutedEventArgs e)
        {
            if (IsTaskRunning(ACTIVESCENE))
            {
                KotoMessageBoxSingle.ShowDialog("该任务已存在,检查任务列表");
                return;
            }
            string activeCmd = $"{adb} shell sh /storage/emulated/0/Android/data/com.omarea.vtools/up.sh";
            ACTIVESCENE.errorThreadAction = new(() => { });//Files Exits会返回错误，但不影响执行，因此把错误监察线程置空
            ACTIVESCENE.Start();
            ACTIVESCENE.CommandWrite([activeCmd]);
            ACTIVESCENE.StreamProcess();
        }

        private void ShizukuButotn_Click(object sender, RoutedEventArgs e)
        {
            string activeCmd = $"{adb} shell sh /storage/emulated/0/Android/data/moe.shizuku.privileged.api/start.sh";
            Active(ACTIVESHIZUKU, activeCmd);
        }
    }
  
}
