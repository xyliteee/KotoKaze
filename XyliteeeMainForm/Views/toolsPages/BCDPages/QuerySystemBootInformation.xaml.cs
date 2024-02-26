using KotoKaze.Static;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static KotoKaze.Static.BCDEDIT;

namespace KotoKaze.Views.toolsPages.BCDPages
{
    /// <summary>
    /// QuerySystemBootInformation.xaml 的交互逻辑
    /// </summary>
    public partial class QuerySystemBootInformation : Page
    {
        private BCDInfo BCDInfo = new();
        public QuerySystemBootInformation()
        {
            InitializeComponent();
            Animations.ImageTurnRound(SettingIcon, true,4);
            ShowBCDInformation();
        }

        private void GetBCDInfo() 
        {
            BCDInfo = GetBCDInformation();
        }

        private void UpDateBootInfo() 
        {
            BootDescription.Content = BCDInfo.BootInfos.First().description;
            BootPath.Content = $"文件目录：{BCDInfo.BootInfos.First().path}";
            BootDevice.Content = $"文件设备：{BCDInfo.BootInfos.First().device}";
            BootLocal.Content = $"语言环境：{BCDInfo.BootInfos.First().locale}";
            BootTimeOut.Content = $"引导超时：{BCDInfo.BootInfos.First().timeout}秒";
        }

        private void Delete(object sender,RoutedEventArgs e) 
        {
            Button button = (Button)sender;
            SystemInfo systemInfo = (SystemInfo)button.Tag;
            DeleteBCDInformation(systemInfo);
            Animations.ImageTurnRound(SettingIcon, true, 4);
            ShowBCDInformation();
        }

        private void BackUP(object sender, RoutedEventArgs e) 
        {
            Button button = (Button)sender;
            SystemInfo systemInfoBack = (SystemInfo)button.Tag;
            BCDBackUPFile BBF = new(systemInfoBack);
            SaveBBF(BBF);
        }

        private void ShowDetail(object sender, RoutedEventArgs e) 
        {
            Animations.ImageTurnRound(SettingIcon, false);
            Button button = (Button)sender;
            SystemInfo systemInfo = (SystemInfo)button.Tag;
            SysteDescription.Content = systemInfo.description;
            SystemFlag.Content = $"{systemInfo.flag}";
            SystemPath.Content = $"文件目录：{systemInfo.path}";
            SystemDevice.Content = $"文件设备：{systemInfo.device}";
            SystemLocal.Content = $"语言环境：{systemInfo.locale}";
            SystemInherit.Content = $"继承对象：{systemInfo.inherit}";
            SystemRecoverysequence.Content = $"{systemInfo.recoverysequence}";
            SystemDisplaymessageoverride.Content = $"显示消息覆盖：{systemInfo.displaymessageoverride}";
            SystemRecoveryenabled.Content = $"是否启用恢复：{systemInfo.recoveryenabled}";
            SystemIsolatedcontext.Content = $"是否隔离上下文：{systemInfo.isolatedcontext}";
            SystemAllowedinmemorysettings.Content = $"允许内存设置：{systemInfo.allowedinmemorysettings}";
            SystemOSdevice.Content = $"系统目录：{systemInfo.osdevice}";
            SystemSystemroot.Content = $"系统根目录：{systemInfo.systemroot}";
            SystemResumeobject.Content = $"{systemInfo.resumeobject}";
            SystemNX.Content = $"数据执行防止：{systemInfo.nx}";
            SystemBootmenupolicy.Content = $"引导菜单类型：{systemInfo.bootmenupolicy}";
            SystemHypervisorlaunchtype.Content = $"是否加载HyperV：{systemInfo.hypervisorlaunchtype}";
            DetailContent.Visibility = Visibility.Visible;
            if (systemInfo.flag == "{current}") { DeleteButton.IsEnabled = false; }
            else { 
                DeleteButton.IsEnabled = true;
                DeleteButton.Click += Delete;
                DeleteButton.Tag = systemInfo;
            }
            BackUpButton.Tag = systemInfo;
            BackUpButton.Click += BackUP;
        }

        private void UpDateSystemInfo() 
        {
            ScrollCanvas.Children.Clear();
            for (int index = 0; index < BCDInfo.SystemInfos.Count; index++) 
            {
                CreatSingaleCard(index);
            }
            CreatADDCard();
            ScrollCanvas.Height = 20 + 80 * ScrollCanvas.Children.Count+1;
        }

        private void CreatSingaleCard(int index) 
        {

            Canvas canvas = new();
            Canvas.SetTop(canvas, 20 + 80 * index);
            Canvas.SetLeft(canvas, 20);
            canvas.Width = 265;
            canvas.Height = 60;

            Border border = new Border
            {
                Height = 60,
                Width = 265,
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(170, 170, 170),
                    Direction = 270,
                    ShadowDepth = 0,
                    Opacity = 0.5,
                    BlurRadius = 5
                }
            };
            Image image = new Image
            {
                Source = new BitmapImage(new Uri("/image/icons/windows11.png", UriKind.Relative)),
                Width = 50,
                Height = 50
            };
            Canvas.SetTop(image, 5);
            Canvas.SetLeft(image, 5);

            string description;
            if (BCDInfo.SystemInfos[index].flag == "{current}")
                description = BCDInfo.SystemInfos[index].description+"【当前】";
            else description = BCDInfo.SystemInfos[index].description;

            Label label = new()
            {
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Height = 60,
                Width = 205,
                Content = description
            };
            Canvas.SetLeft(label, 60);
            Button button = new()
            {
                Width = 265,
                Height = 60,
                Opacity = 0,
                Tag = BCDInfo.SystemInfos[index]
            };
            button.Click += ShowDetail;
            canvas.Children.Add(border);
            canvas.Children.Add(image);
            canvas.Children.Add(label);
            canvas.Children.Add(button);
            ScrollCanvas.Children.Add(canvas);
        }

        private void CreatADDCard()
        {

            Canvas canvas = new();
            Canvas.SetTop(canvas, 20 + 80 * BCDInfo.SystemInfos.Count);
            Canvas.SetLeft(canvas, 20);
            canvas.Width = 265;
            canvas.Height = 60;

            Border border = new Border
            {
                Height = 60,
                Width = 265,
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(170, 170, 170),
                    Direction = 270,
                    ShadowDepth = 0,
                    Opacity = 0.5,
                    BlurRadius = 5
                }
            };
            Image image = new()
            {
                Source = new BitmapImage(new Uri("/image/icons/cha.png", UriKind.Relative)),
                Width = 32,
                Height = 32
            };
            RotateTransform rotateTransform = new(45, image.Width / 2, image.Height / 2);
            image.RenderTransform = rotateTransform;
            Canvas.SetTop(image, 14);
            Canvas.SetLeft(image, 60);

            Label label = new()
            {
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Height = 60,
                Width = 205,
                Content = "添加引导项"
            };
            Canvas.SetLeft(label, 50);
            Button button = new()
            {
                Width = 265,
                Height = 60,
                Opacity = 0,
            };
            canvas.Children.Add(border);
            canvas.Children.Add(image);
            canvas.Children.Add(label);
            canvas.Children.Add(button);
            ScrollCanvas.Children.Add(canvas);
        }

        private void ShowBCDInformation() 
        {
            DetailContent.Visibility = Visibility.Hidden;
            Task.Run(() => 
            {
                GetBCDInfo();
                Dispatcher.BeginInvoke(() => 
                {
                    UpDateBootInfo();
                    UpDateSystemInfo();
                });
            });
        }
    }
}
