using KotoKaze.Static;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SearchOption = System.IO.SearchOption;
using System.Windows.Media.Effects;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using HandyControl;
using System.Management;
using System.Runtime.InteropServices;

#pragma warning disable CS8600
#pragma warning disable CS8602

namespace XyliteeeMainForm.Views
{
    public partial class cleanPage : Page
    {
        private readonly MainWindow mainWindow;
        private bool isScanned = false;
        private readonly List<string> trashTempFiles = [];
        private readonly List<string> trashUpdateFiles = [];
        private readonly List<string> trashRecycleFiles = [];
        private readonly List<string> trashOtherFiles = [];
        private static  readonly double[] sizes = [1024, Math.Pow(1024, 2), Math.Pow(1024, 3), Math.Pow(1024, 4)];
        private static readonly string[] units = ["KB", "MB", "GB", "TB"];
        private readonly List<Button> uninstallerButtons = [];
        private List<SoftWare> softWares = [];
        private readonly BitmapImage defaultIcon = new(new Uri("pack://application:,,,/image/icons/windows11.png"));
        public cleanPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            ScanSoftWare();
        }
        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Button button = (Button)sender;
            Border border;

            if (button.Name == "CleanButton")
            {
                border = CleanButtonBorder;
            }
            else 
            {
                border = RefreshButtonBorder;
            }
            ColorAnimation animation = new()
            {
                To = (Color)ColorConverter.ConvertFromString("#A0C5E0"),
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, border);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Background.Color"));
            storyboard.Begin();
        }
        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Button button = (Button)sender;
            Border border;

            if (button.Name == "CleanButton")
            {
                border = CleanButtonBorder;
            }
            else
            {
                border = RefreshButtonBorder;
            }
            ColorAnimation animation = new()
            {
                To = (Color)ColorConverter.ConvertFromString("#F3F8FE"),
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, border);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Background.Color"));
            storyboard.Begin();
        }

        private void ScanFilesAndUpdateUI(string fileType)
        {
            long trashFilesSize = 0;
            List<string> filePathes;
            switch (fileType)
            {
                case "temp":
                    trashTempFiles.Clear();
                    filePathes = CleanRules.tempFilesRules;
                    foreach (string filePath in filePathes) 
                    {
                        if (!Directory.Exists(filePath)) continue;
                        foreach (var file in Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories))
                        {
                            trashTempFiles.Add(file);
                            FileInfo fi = new(file);
                            trashFilesSize += fi.Length;
                            Dispatcher.Invoke(() => { TempFilesLabel.Content = $"已扫描{BytesToOthers(trashFilesSize)}"; });
                        }
                    }
                    break;

                case "update":
                    trashUpdateFiles.Clear();
                    filePathes = CleanRules.updateFilesRules;
                    foreach (string filePath in filePathes) 
                    {
                        if (!Directory.Exists(filePath)) continue;
                        foreach (var file in Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories))
                        {
                            trashUpdateFiles.Add(file);
                            FileInfo fi = new(file);
                            trashFilesSize += fi.Length;
                            Dispatcher.Invoke(() => { UpdateFilesLabel.Content = $"已扫描{BytesToOthers(trashFilesSize)}"; });
                        }
                    }
                    if (trashFilesSize == 0) { Dispatcher.Invoke(() => { UpdateFilesLabel.Content = $"非常干净"; }); }
                    break;

                case "Recycle":
                    trashRecycleFiles.Clear();
                    try 
                    {
                        foreach (DriveInfo drive in DriveInfo.GetDrives())
                        {
                            if (!drive.IsReady) continue;
                            string filePath = Path.Combine(drive.Name, "$Recycle.Bin");
                            if (!Directory.Exists(filePath)) continue;
                            foreach (string directory in Directory.GetDirectories(filePath))
                            {
                                foreach (var file in FileSystem.GetFiles(directory, Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories, "*"))
                                {
                                    trashRecycleFiles.Add(file);
                                    FileInfo fi = new(file);
                                    trashFilesSize += fi.Length;
                                    Dispatcher.Invoke(() =>{RecycleFilesLabel.Content = $"已扫描{BytesToOthers(trashFilesSize)}";});
                                }
                            }
                        }
                    } 
                    catch{}
                    break;

                case "other":
                    trashOtherFiles.Clear();
                    filePathes = CleanRules.otherFilesRules;
                    foreach (string filePath in filePathes) 
                    {
                        if (!Directory.Exists(filePath)) continue;
                        foreach (var file in Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories))
                        {
                            trashOtherFiles.Add(file);
                            FileInfo fi = new(file);
                            trashFilesSize += fi.Length;
                            Dispatcher.Invoke(() => { OtherFilesLabel.Content = $"已扫描{BytesToOthers(trashFilesSize)}"; });
                        }
                    }
                    if (trashFilesSize == 0) { Dispatcher.Invoke(() => { OtherFilesLabel.Content = $"非常干净"; }); }
                    break;
                    
            }
        }

        private void DeleteFilesAndUpdateUI(string fileType)
        {
            long trashFilesSize = 0;
            long singleFilesSize = 0;
            switch (fileType)
            {
                case "temp":
                    foreach (var file in trashTempFiles)
                    {
                        try 
                        {
                            FileInfo fi = new(file);
                            singleFilesSize = fi.Length;
                            File.Delete(file);
                            trashFilesSize += singleFilesSize;
                        }
                        catch { }
                        Dispatcher.Invoke(() => { TempFilesLabel.Content = $"已清理{BytesToOthers(trashFilesSize)}"; });
                    }
                    break;

                case "update":
                    foreach (var file in trashUpdateFiles)
                    {
                        try
                        {
                            FileInfo fi = new(file);
                            singleFilesSize = fi.Length;
                            File.Delete(file);
                            trashFilesSize += singleFilesSize;
                        }
                        catch{}
                        Dispatcher.Invoke(() =>{UpdateFilesLabel.Content = $"已清理{BytesToOthers(trashFilesSize)}";});
                    }
                    break;

                case "other":
                    foreach (var file in trashOtherFiles)
                    {
                        try
                        {
                            FileInfo fi = new(file);
                            singleFilesSize = fi.Length;
                            File.Delete(file);
                            trashFilesSize += singleFilesSize;
                        }
                        catch { }
                        Dispatcher.Invoke(() => { OtherFilesLabel.Content = $"已清理{BytesToOthers(trashFilesSize)}"; });
                    }
                    break;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) 
        {
            ScanSoftWare();
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            

            if (isScanned)
            {
                CleanButton.IsEnabled = false;
                CleanImage.Source = new BitmapImage(new Uri("pack://application:,,,/image/icons/scaning.png"));
                Animations.ImageTurnRound(CleanImage, true);

                Task.Run(() =>
                {
                    DeleteFilesAndUpdateUI("temp");
                    DeleteFilesAndUpdateUI("update");
                    DeleteFilesAndUpdateUI("other");
                    Dispatcher.Invoke(() =>
                    {
                        RecycleFilesLabel.Content = "请手动清理";
                        isScanned = false;
                        CleanButton.IsEnabled = true;
                        CleanImage.Source = new BitmapImage(new Uri("pack://application:,,,/image/icons/scan.png"));
                        Animations.ImageTurnRound(CleanImage, false);
                    });
                });
            }
            else 
            {
                CleanButton.IsEnabled = false;
                CleanImage.Source = new BitmapImage(new Uri("pack://application:,,,/image/icons/scaning.png"));
                Animations.ImageTurnRound(CleanImage, true);

                Task.Run(() =>
                {
                    ScanFilesAndUpdateUI("temp");
                    ScanFilesAndUpdateUI("update");
                    ScanFilesAndUpdateUI("Recycle");
                    ScanFilesAndUpdateUI("other");
                    Dispatcher.Invoke(() => 
                    {
                        isScanned = true;
                        CleanButton.IsEnabled = true;
                        CleanImage.Source = new BitmapImage(new Uri("pack://application:,,,/image/icons/startclean.png"));
                        Animations.ImageTurnRound(CleanImage, false);
                    });
                });
            }
        }
        private static string BytesToOthers(long inputBytes)
        {
            double output;
            string unit;

            if (inputBytes < 1024)
            {
                output = inputBytes;
                unit = "B";
            }
            else
            {
                int index = 0;
                while (inputBytes >= sizes[index] && index < sizes.Length - 1)
                {
                    index++;
                }
                output = Math.Round(inputBytes / sizes[index - 1], 1);
                unit = units[index - 1];
            }
            return $"{output}{unit}";
        }

        private void CreatSingleCard(int index,SoftWare softWare) 
        {

            Canvas card = new();
            Canvas.SetTop(card, 20+80*index);
            Canvas.SetLeft(card, 20);
            card.Width = 330;
            card.Height = 60;

            Border border = new()
            {
                Width = 330,
                Height = 60,
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(170, 170, 170),
                    Direction = 270,
                    ShadowDepth = 0,
                    Opacity = 0.5,
                    BlurRadius = 5
                }
            };
            card.Children.Add(border);


            BitmapImage softWareIconImage;
            try 
            {
                System.Drawing.Bitmap a = System.Drawing.Icon.ExtractAssociatedIcon(softWare.displayIcon).ToBitmap();
                var m = new MemoryStream();
                a.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                m.Position = 0;
                softWareIconImage = new BitmapImage();
                softWareIconImage.BeginInit();
                softWareIconImage.StreamSource = m;
                softWareIconImage.EndInit();

            }
            catch
            {
                softWareIconImage = defaultIcon; 
            }

            Image image = new()
            {
                Height = 50,
                Width = 50,
                Source = softWareIconImage
            };
            card.Children.Add(image);
            Canvas.SetLeft(image, 5);
            Canvas.SetTop(image, 5);


            Label label1 = new()
            {
                Width = 200,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Content = softWare.displayName,
                FontSize = 12,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            Canvas.SetTop(label1, 5);
            Canvas.SetLeft(label1, 50);
            card.Children.Add(label1);

            Label label2 = new()
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Content = softWare.installDate,
                Width = 200,
                FontSize = 12,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            Canvas.SetBottom(label2, 5);
            Canvas.SetLeft(label2, 50);
            card.Children.Add(label2);


            Button button = new()
            {
                Content = "卸载",
                Width = 60,
                Height = 30
            };
            Canvas.SetTop(button, 15);
            Canvas.SetRight(button, 10);
            button.Style = (Style)FindResource("unInstallerButtonStyle");
            card.Children.Add(button);
            button.Tag = softWare;
            button.Click += DialogButtonClick;
            uninstallerButtons.Add(button);

            ScorllCanvas.Children.Add(card);

        }

        private void DialogButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            SoftWare softWare = (SoftWare)button.Tag;
            string uninstallFileName = string.Empty;
            string uninstallArg = string.Empty;

            if (File.Exists(softWare.uninstallString)) { uninstallFileName = softWare.uninstallString; }

            else if (softWare.uninstallString.Contains('"'))
            {
                var match = Regex.Match(softWare.uninstallString, @"^""([^""]+)""\s*(.*)");
                if (match.Success)
                {
                    uninstallFileName = match.Groups[1].Value;
                    uninstallArg = match.Groups[2].Value;
                }
            }
            else
            {
                var args = softWare.uninstallString.Split([' '], 2);
                if (args.Length > 0)
                {
                    uninstallFileName = args[0];
                    if (args.Length > 1)
                    {
                        uninstallArg = args[1];
                    }
                }
            }
            ProcessStartInfo startInfo = new()
            {
                FileName = uninstallFileName,
                Arguments = uninstallArg,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            try
            {
                Process process = new()
                {
                    StartInfo = startInfo
                };
                process.Start();
                process.BeginErrorReadLine();
            }
            catch(Exception ex)
            {
                HandyControl.Controls.MessageBox.Warning($"执行失败，错误如下{ex}，可能是注册信息缺失或者已经卸载，也可能单纯是开发者的锅");
            }
        }



        
        private void ScanSoftWare()
        {
            uninstallerButtons.Clear();
            softWares.Clear();
            ScorllCanvas.Opacity = 0.4;
            RefreshButton.IsEnabled = false;
            Animations.ImageTurnRound(RefreshImage, true);

            Task.Run(() => 
            {
                RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                RegistryKey uninstallKey = baseKey.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
                foreach (string subKeyName in uninstallKey.GetSubKeyNames())
                {
                    using RegistryKey subKey = uninstallKey.OpenSubKey(subKeyName);
                    if (subKey.GetValue("DisplayName") is not string displayName || displayName == string.Empty)
                    {
                        continue;
                    }
                    SoftWare softWare = new(
                        GetValueOrDefault(subKey, "InstallDate"),
                        GetValueOrDefault(subKey, "InstallLocation"),
                        displayName,
                        GetValueOrDefault(subKey, "DisplayIcon"),
                        GetValueOrDefault(subKey, "UninstallString")
                    );
                    softWares.Add(softWare);
                }
                softWares = [.. softWares.OrderBy(s => s.displayName)];
                Dispatcher.BeginInvoke(() =>
                {
                    ScorllCanvas.Children.Clear();
                    for (int index = 0; index < softWares.Count; index++) 
                    {
                        CreatSingleCard(index, softWares[index]);
                    }
                    ScorllCanvas.Height = 20 + 80 * softWares.Count;
                    ScorllCanvas.Opacity = 1;
                    RefreshButton.IsEnabled = true;
                    Animations.ImageTurnRound(RefreshImage, false);
                });
            });
        }
        private static string GetValueOrDefault(RegistryKey key, string valueName)
        {
            return key.GetValue(valueName) as string ?? "未提供";
        }
    }

    public partial class SoftWare
    {
        public string installDate;
        public string installLocation;
        public string displayName;
        public string displayIcon;
        public string uninstallString;
        public SoftWare(string installDate, string installLocation, string displayName, string displayIcon, string uninstallString)
        {
            if (MyRegex().IsMatch(installDate))
            {
                this.installDate = DateTime.ParseExact(installDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString().Replace(" 0:00:00","");
            }
            else 
            {
                this.installDate = installDate;
            }
            this.installLocation = installLocation;
            this.displayName = displayName;
            this.displayIcon = displayIcon;
            this.uninstallString = uninstallString.Replace("MsiExec.exe", "\"MsiExec.exe\"");
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex MyRegex();

        
    }


}
