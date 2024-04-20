using CleanContent;
using KotoKaze.Static;
using SevenZip.Compression.LZ;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OpenHardwareMonitor.Hardware;
using FileControl;
using System.Drawing.Printing;
using Newtonsoft.Json;
using KotoKaze.Windows;
using KotoKaze.Views;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Text;

#pragma warning disable CS8602
namespace XyliteeeMainForm.Views
{
    public partial class homePage : Page
    {
        private readonly SystemInfos systemInfo;
        private readonly Computer myComputer = new();
        private readonly UpdateVisitor updateVisitor = new();
        private static bool IsRecord = false;
        private double  recordTime = 0;

        private Dictionary<string, List<double>> DataRecord = [];
        

        public homePage()
        {
            InitializeComponent();
            myComputer.Open();
            myComputer.MainboardEnabled = true;
            myComputer.CPUEnabled = true;
            myComputer.RAMEnabled = true;
            myComputer.GPUEnabled = true;
            myComputer.HDDEnabled = true;

            systemInfo = new(myComputer);
            systemName.Content = $"系统名称：{systemInfo.SystemName}";
            systemVersion.Content = $"系统版本：{systemInfo.SystemVersion}";
            modelOfCPU.Content = $"CPU型号：{systemInfo.ModelOfCPU}";
            sizeOfRAM.Content = $"主板型号：{systemInfo.Mainbord}";
            userName.Content = $"账户名称：{systemInfo.UserName}";

            if (systemInfo.SystemName.Contains("11"))
            {
                systemIcon.Source = new BitmapImage(new Uri("pack://application:,,,/image/icons/windows11.png"));
            }
            else
            {
                systemIcon.Source = new BitmapImage(new Uri("pack://application:,,,/image/icons/windows10.png"));

            }
            GetCurrentData();
        }
        private async void GetCurrentData()
        {
            double CPUUsage=0;
            double ramUseRate = 0;
            double ramUsage = 0;
            double diskUseRate = 0;
            double CPUpower = 0;
            double CPUTemp = 0;
            double SocGPUPower = 0;
            string key = string.Empty;
            try
            {
                while (GlobalData.IsRunning)
                {
                    myComputer.Accept(updateVisitor);
                    foreach (var hardwareItem in myComputer.Hardware)
                    {
                        if (hardwareItem.HardwareType == HardwareType.CPU)
                        {
                            foreach (var sensor in hardwareItem.Sensors)
                            {
                                key = sensor.Name + "_" + sensor.SensorType;
                                if (key == "CPU Total_Load") CPUUsage = (int)sensor.Value!;
                                else if(key == "CPU Package_Power") CPUpower = Math.Round((double)sensor.Value!,2);
                                else if(key == "CPU Package_Temperature") CPUTemp = Math.Round((double)sensor.Value!,2);
                                else if(key == "CPU Graphics_Power") SocGPUPower = Math.Round((double)sensor.Value!,2);
                            }
                        }
                        else if (hardwareItem.HardwareType == HardwareType.RAM)
                        {
                            foreach (var sensor in hardwareItem.Sensors)
                            {
                                key = sensor.Name + "_" + sensor.SensorType;
                                if(key == "Used Memory_Data") 
                                {
                                    ramUsage = Math.Round((double)sensor.Value!,2);
                                    break;
                                }
                            }
                            ramUseRate = Math.Round(ramUsage / systemInfo.RamNumber, 2);
                        }
                        else if (hardwareItem.HardwareType == HardwareType.HDD)
                        {
                            foreach (var sensor in hardwareItem.Sensors)
                            {
                                key = sensor.Name + "_" + sensor.SensorType;
                                if (key == "Used Space_Load") 
                                {
                                    diskUseRate = (int)sensor.Value!;
                                    break;
                                }
                            }
                        }
                    }
                    if (IsRecord) 
                    {
                        if (recordTime > 1800)
                        {
                            OutofTime();
                            continue;
                        }
                        DataRecord["CPU_Load"].Add(CPUUsage);
                        DataRecord["CPU_Power"].Add(CPUpower);
                        DataRecord["CPU_Temp"].Add(CPUTemp);
                        DataRecord["CoreGPU_Power"].Add(SocGPUPower);
                        DataRecord["RAM_Load"].Add(ramUseRate*100);
                        int minutes = (int)(recordTime / 60);
                        double second = recordTime - 60 * minutes;
                        Dispatcher.Invoke(()=>
                        {
                            recordTimeText.Text = $"{minutes}分\n{second}秒";
                        });
                        recordTime+= GlobalData.RefreshTime;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        cpuCircleBar.Value = CPUUsage;
                        cpuUsedLabel.Content = $"CPU占用{CPUUsage}%";
                        cpuPowerLabel.Content = $"封装功耗:{CPUpower}W";
                        gpuPowerLabel.Content = $"核显功耗:{SocGPUPower}W";
                        cpuTempLabel.Content = $"封装温度:{CPUTemp}°C";
                        ramBar.Value = ramUseRate * 100;
                        ramLabel.Content = $"内存使用情况：{ramUsage}GB/{systemInfo.RamNumber}GB";
                        DiskBar.Value = diskUseRate;
                        diskLabel.Content = $"硬盘使用情况：{diskUseRate * systemInfo.diskTotal / 100}GB/{systemInfo.diskTotal}GB";
                    });
                    
                    await Task.Delay((int)(GlobalData.RefreshTime*1000));
                }
            }
            catch (ThreadAbortException) { }
            catch (TaskCanceledException) { }
            catch(Exception ex) 
            {
                await FileManager.LogManager.LogWriteAsync("Get DeviceInformation Error",ex.ToString());
            }
        }
        private void OutofTime() 
        {
            Dispatcher.Invoke(async() => 
            {
                RecordButton.IsEnabled = false;
                recordTimeText.Text = "...";
                IsRecord = false;
                RecordButtonImage.Source = BitmapImages.startImage;
                string json = JsonConvert.SerializeObject(DataRecord, Formatting.Indented);
                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                await File.WriteAllTextAsync(Path.Combine(FileManager.WorkDirectory.outPutDirectory, $"RecordData/{time}.json"), json);
                KotoMessageBoxSingle.ShowDialog("录制超时，已自动保存");
                RecordButton.IsEnabled = true;
                var rr = KotoMessageBox.ShowDialog("是否绘制表格？");
                if (rr.IsYes)
                {
                    ReadRecord(DataRecord);
                }
            });
        } 
        private async void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsRecord)
            {
                RecordButton.IsEnabled = false;
                recordTimeText.Text = "...";
                IsRecord = false;
                RecordButtonImage.Source = BitmapImages.startImage;
                string json = JsonConvert.SerializeObject(DataRecord, Formatting.Indented);
                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                await File.WriteAllTextAsync(Path.Combine(FileManager.WorkDirectory.outPutDirectory, $"RecordData/{time}.json"), json);
                KotoMessageBoxSingle.ShowDialog("已记录数据");
                RecordButton.IsEnabled = true;
                SettingButton.IsEnabled  = true;
                var rr = KotoMessageBox.ShowDialog("是否绘制表格？");
                if (rr.IsYes)
                {
                    ReadRecord(DataRecord);
                }
            }
            else 
            {
                var r = KotoMessageBox.ShowDialog("是否录制资源占用信息？");
                if (r.IsYes)
                {
                    SettingButton.IsEnabled = false;
                    recordTime = 0;
                    DataRecord = new()
                    {
                    { "Refresh_Time",[GlobalData.RefreshTime]},
                    { "CPU_Load" , [] },
                    { "CPU_Power", [] },
                    { "CPU_Temp" , [] },
                    { "CoreGPU_Power", [] },
                    { "RAM_Load" , [] },
                    };
                    IsRecord = true;
                    RecordButtonImage.Source = BitmapImages.StopImage;
                }
            }
        }
        private void ReadRecord(Dictionary<string, List<double>> data) 
        {
            secondActionFrame.Navigate(new RecordPage(data));
            secondActionFrame.Visibility = Visibility.Visible;
            GlobalData.MainWindowInstance.backButton.Visibility = Visibility.Visible;
        }

        private void ShowChartButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "JSON files (*.json)|*.json",
                InitialDirectory = Path.Combine(FileManager.WorkDirectory.outPutDirectory, "RecordData")
            };
            if (openFileDialog.ShowDialog()==true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                Dictionary<string, List<double>>? data = JsonConvert.DeserializeObject<Dictionary<string, List<double>>>(json);
                if (data == null) 
                {
                    KotoMessageBoxSingle.ShowDialog("不是有效的文件");
                    return;
                }
                ReadRecord(data);
            }
            
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            RecordSettingWindow.ShowSettingPage();
        }
    }

    public class BitmapImages
    {
        public readonly static BitmapImage startImage = new(new Uri("pack://application:,,,/image/icons/startButton.png"));
        public readonly static BitmapImage StopImage = new(new Uri("pack://application:,,,/image/icons/stopButton.png"));
    }
    public class SystemInfos
    {
        public string SystemName = string.Empty;
        public string SystemVersion = string.Empty;
        public string ModelOfCPU = string.Empty;
        public double RamNumber = 0;
        public string UserName = string.Empty;
        public string Mainbord = string.Empty;
        public double diskTotal = 0;


        public SystemInfos(Computer myComputer)
        {
            ManagementObjectSearcher osSearcher = new("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject osInfo in osSearcher.Get().Cast<ManagementObject>())
            {
                SystemName = osInfo["Caption"].ToString().Replace("Microsoft", "");
                SystemVersion = osInfo["Version"].ToString();
            }
            foreach (var hardwareItem in myComputer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.CPU)
                {
                    ModelOfCPU = hardwareItem.Name;
                }
                if (hardwareItem.HardwareType == HardwareType.Mainboard) 
                {
                    Mainbord = hardwareItem.Name;
                }
                if (ModelOfCPU != string.Empty && Mainbord != string.Empty) break;
            }
            ManagementObjectSearcher noNameSearcher = new("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject noNameInfo in noNameSearcher.Get().Cast<ManagementObject>())
            {
                RamNumber = Math.Round(Convert.ToDouble(noNameInfo["TotalPhysicalMemory"]) / 1024 / 1024 / 1024, 2);
                UserName = noNameInfo[nameof(UserName)].ToString();
            }
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    diskTotal += d.TotalSize / 1024.0 / 1024.0 / 1024.0;
                }
            }
            diskTotal = Math.Round(diskTotal);
        }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware)
                subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
        //CPU Core #1_Load
        //CPU Core #2_Load
        //CPU Total_Load
        //CPU Core #1_Temperature
        //CPU Core #2_Temperature
        //CPU Package_Temperature
        //CPU Core #1_Clock
        //CPU Core #2_Clock
        //CPU Package_Power
        //CPU Cores_Power
        //CPU Graphics_Power
        //CPU DRAM_Power
        //Bus Speed_Clock
        //Memory_Load
        //Used Memory_Data
        //Available Memory_Data
        //以上是所有目前需要且能获取的数据
    }
}
