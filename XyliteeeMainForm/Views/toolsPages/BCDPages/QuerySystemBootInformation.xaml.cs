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
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using KotoKaze.Windows;
using static KotoKaze.Dynamic.BCDEDIT;
using System.IO;

namespace KotoKaze.Views.toolsPages.BCDPages
{
    /// <summary>
    /// QuerySystemBootInformation.xaml 的交互逻辑
    /// </summary>
    public partial class QuerySystemBootInformation : Page
    {
        private BCDInfo BCDInfo = new();
        private readonly List<Canvas> cardLists = [];
        public QuerySystemBootInformation()
        {
            InitializeComponent();
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
            var r = KotoMessageBox.ShowDialog("这将会删除这个引导，确定吗？");
            if (r.IsYes) 
            {
                Button button = (Button)sender;
                SystemInfo systemInfo = (SystemInfo)button.Tag;
                DeleteBCDInformation(systemInfo);
                ShowBCDInformation();
                KotoMessageBoxSingle.ShowDialog("已删除该引导。");
            }
        }

        private async void BackUP(object sender, RoutedEventArgs e)
        {
            var saveMessageboxRes = KotoMessageBox.ShowDialog("将会在程序根目录/Backup/保存BBF文件，确定？");
            if (!saveMessageboxRes.IsYes) return;

            Button button = (Button)sender;
            SystemInfo systemInfoBack = (SystemInfo)button.Tag;
            BCDBackUPFile BBF = new(systemInfoBack);
            while (true) 
            {
                var saveNameMessageboxRes = KotoMessageBoxInput.ShowDialog("输入要保存的文件名称");
                if (!saveNameMessageboxRes.IsYes) return;
                string filePath = Path.Combine(FileManager.WorkDirectory.backupDirectory, $"{saveNameMessageboxRes.Input}.BBF");

                if (Path.Exists(filePath))
                {
                    var alreadyExitMessageboxRes = KotoMessageBox.ShowDialog("该备份已经存在，是否替换？");
                    if (!alreadyExitMessageboxRes.IsYes) continue;
                }

                await SaveBBF(BBF,saveNameMessageboxRes.Input);
                KotoMessageBoxSingle.ShowDialog($"保存完成，文件为{saveNameMessageboxRes.Input}.BBF");
                return;
            }
        }

        private void AddNewOne(object sender,RoutedEventArgs e) //叫我嵌套仙人
        {
            void UITask(KotoMessageBoxInput.MessageResult rr,BCDBackUPFile bbf) 
            {
                Task.Run(() =>
                {
                    bool isSuccessful = ImportSystemBootInfo(rr.Input, bbf);
                    Dispatcher.Invoke(() =>
                    {
                        string message;
                        if (isSuccessful) { message = "添加完成"; } else { message = "出现错误"; }
                        KotoMessageBoxSingle.ShowDialog(message);
                        ShowBCDInformation();
                    });
                });
            }
            var r = KotoMessageBox.ShowDialog("是否选择导入本地的BBF文件？");
            if (r.IsYes)
            {
                Microsoft.Win32.OpenFileDialog dlg = new()
                {
                    DefaultExt = ".bbf",
                    DefaultDirectory = FileManager.WorkDirectory.backupDirectory,
                    Filter = "BBF Files (*.bbf)|*.bbf"
                };
                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    string selectedFilePath = dlg.FileName;
                    BCDBackUPFile bbf = ReadBBF(selectedFilePath);
                    if (bbf.CheckCode == string.Empty) { KotoMessageBoxSingle.ShowDialog("错误的文件!"); return; }
                    var rr = KotoMessageBoxInput.ShowDialog("输入该引导的描述");
                    if (rr.IsYes)
                    {
                        UITask(rr, bbf);
                    }
                }
            }
            else 
            {
                var rr = KotoMessageBoxInput.ShowDialog("输入该引导的描述");
                BCDBackUPFile bbf = new(new SystemInfo());
                if (rr.IsYes)
                {
                    var rrr = KotoMessageBoxInput.ShowDialog("请输入系统所在盘符，仅输入单个字母即可，例如\"C\"");
                    if (rr.IsYes) 
                    {
                        bbf.SystemInfo.Device = $"partition={rrr.Input}:";
                        bbf.SystemInfo.Path = "\\Windows\\system32\\winload.efi";
                        bbf.SystemInfo.Osdevice = $"partition={rrr.Input}:";
                        bbf.SystemInfo.Systemroot = "\\Windows";
                        UITask(rr, bbf);
                    }
                }
            }
        }

        private void Default(object sender, RoutedEventArgs e) 
        {
            var r = KotoMessageBox.ShowDialog("将这个引导设置为默认，确定吗？");
            if (r.IsYes)
            {
                Button button = (Button)sender;
                SystemInfo systemInfo = (SystemInfo)button.Tag;
                SetDefault(systemInfo);
                ShowBCDInformation();
                KotoMessageBoxSingle.ShowDialog("已设置为默认。");
            }
        }

        private async void ExportOriginal(object sender, RoutedEventArgs e) 
        {
            var saveMessageboxRes = KotoMessageBox.ShowDialog("将会在程序根目录/Backup/保存txt文件，确定？");
            if (!saveMessageboxRes.IsYes) return;

            Button button = (Button)sender;
            SystemInfo systemInfoBack = (SystemInfo)button.Tag;
            BCDBackUPFile BBF = new(systemInfoBack);
            while (true)
            {
                var saveNameMessageboxRes = KotoMessageBoxInput.ShowDialog("输入要保存的文件名称");
                if (!saveNameMessageboxRes.IsYes) return;
                string filePath = Path.Combine(FileManager.WorkDirectory.backupDirectory, $"{saveNameMessageboxRes.Input}.txt");

                if (Path.Exists(filePath))
                {
                    var alreadyExitMessageboxRes = KotoMessageBox.ShowDialog("该文件已经存在，是否替换？");
                    if (!alreadyExitMessageboxRes.IsYes) continue;
                }

                await SaveOriginal(BBF.SystemInfo.OriginalInfomation, saveNameMessageboxRes.Input);
                KotoMessageBoxSingle.ShowDialog($"保存完成，文件为{saveNameMessageboxRes.Input}.txt");
                return;
            }
        }

        private void ShowDetail(object sender, RoutedEventArgs e) 
        {
            Animations.ImageTurnRound(SettingIcon, false);
            foreach (Canvas canvas in cardLists) 
            {
                canvas.Opacity = 0.6;
            }
            Button button = (Button)sender;
            Canvas thisCansvas = (Canvas)button.Parent;thisCansvas.Opacity = 1;

            SystemInfo systemInfo = (SystemInfo)button.Tag;
            SysteDescription.Content = systemInfo.Description;
            SystemFlag.Content = $"{systemInfo.Flag}";
            SystemPath.Content = $"文件目录：{systemInfo.Path}";
            SystemDevice.Content = $"文件设备：{systemInfo.Device}";
            SystemLocal.Content = $"语言环境：{systemInfo.Locale}";
            SystemInherit.Content = $"继承对象：{systemInfo.Inherit}";
            SystemDisplaymessageoverride.Content = $"显示消息覆盖：{systemInfo.Displaymessageoverride}";
            SystemRecoveryenabled.Content = $"是否启用恢复：{systemInfo.Recoveryenabled}";
            SystemIsolatedcontext.Content = $"是否隔离上下文：{systemInfo.Isolatedcontext}";
            SystemAllowedinmemorysettings.Content = $"允许内存设置：{systemInfo.Allowedinmemorysettings}";
            SystemOSdevice.Content = $"系统目录：{systemInfo.Osdevice}";
            SystemSystemroot.Content = $"系统根目录：{systemInfo.Systemroot}";
            SystemResumeobject.Content = $"{systemInfo.Resumeobject}";
            SystemNX.Content = $"数据执行防止：{systemInfo.Nx}";
            SystemBootmenupolicy.Content = $"引导菜单类型：{systemInfo.Bootmenupolicy}";
            SystemHypervisorlaunchtype.Content = $"是否加载HyperV：{systemInfo.Hypervisorlaunchtype}";
            DetailContent.Visibility = Visibility.Visible;
            if (systemInfo.Flag == "{current}") { DeleteButton.IsEnabled = false; }
            else { 
                DeleteButton.IsEnabled = true;
                DeleteButton.Tag = systemInfo;
            }
            BackUpButton.Tag = systemInfo;
            DefaultButton.Tag = systemInfo;
            OriginalButton.Tag = systemInfo;
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
            canvas.Opacity = 0.6;
            cardLists.Add(canvas);

            Border border = new()
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
                Source = new BitmapImage(new Uri("/image/icons/windows11.png", UriKind.Relative)),
                Width = 50,
                Height = 50
            };
            Canvas.SetTop(image, 5);
            Canvas.SetLeft(image, 5);

            string description = BCDInfo.SystemInfos[index].Description;

            if (BCDInfo.SystemInfos[index].Flag == "{current}")
                description += "【当前】";
            if (BCDInfo.SystemInfos[index].Flag == "{default}")
                description += "【默认】";
            if (BCDInfo.SystemInfos[index].Winpe == "Yes")
                description += "【PE】";

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
            button.Click += AddNewOne;
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
