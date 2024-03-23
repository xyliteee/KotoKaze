
using KotoKaze.Static;
using SevenZip.Compression.LZ;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

#pragma warning disable CS8602
namespace XyliteeeMainForm.Views
{
    public partial class homePage : Page
    {
        private readonly SystemInfos systemInfo = new();
        public homePage()
        {
            InitializeComponent();
            systemName.Content = $"系统名称：{systemInfo.SystemName}";
            systemVersion.Content = $"系统版本：{systemInfo.SystemVersion}";
            modelOfCPU.Content = $"CPU型号：{systemInfo.ModelOfCPU}";
            sizeOfRAM.Content = $"内存容量：{systemInfo.Ram}";
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
        private void GetCurrentData()
        {
            Task.Run(() => 
            {
                double memoryAvailable;
                double memoryUsed;
                double diskTotal;
                double diskAvailable;
                double diskUsed;
                int ramUseRate;
                int diskUseRate;
                double cpuUsage;
                PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");
                PerformanceCounter ramCounter = new("Memory", "Available MBytes");
                while (GlobalData.IsRunning)
                {
                    memoryAvailable = ramCounter.NextValue();
                    memoryUsed = Convert.ToDouble(systemInfo.RamNumber) - memoryAvailable;
                    ramUseRate = (int)(memoryUsed / systemInfo.RamNumber * 100);

                    DriveInfo systemDrive = new("C:\\");
                    diskTotal = systemDrive.TotalSize;
                    diskAvailable = systemDrive.TotalFreeSpace;
                    diskUsed = diskTotal - diskAvailable;
                    diskUseRate = (int)(diskUsed / diskTotal * 100);
                    cpuCounter.NextValue();
                    Thread.Sleep(1000);
                    cpuUsage = cpuCounter.NextValue();
                    double memoryUsedToGB = Math.Round(memoryUsed / 1024, 1);
                    double memoryTotleToGB = Math.Round(systemInfo.RamNumber / 1024, 1);
                    int diskUsedToGB = (int)(diskUsed / 1024 / 1024 / 1024);
                    int diskTotletToGB = (int)(diskTotal / 1024 / 1024 / 1024);

                    Dispatcher.Invoke(() => 
                    {
                        cpuCircleBar.Value = cpuUsage;
                        cpuLabel.Content = $"CPU占用{(int)cpuUsage}%";
                        ramBar.Value = ramUseRate;
                        ramLabel.Content = $"内存使用情况：{memoryUsedToGB}GB / {memoryTotleToGB}GB";
                        DiskBar.Value = diskUseRate;
                        diskLabel.Content = $"系统分区使用情况：{diskUsedToGB}GB / {diskTotletToGB}GB";
                    });
                }
            });
            
        }
    }



    public class SystemInfos
    {
        public string SystemName = string.Empty;
        public string SystemVersion = string.Empty;
        public string ModelOfCPU = string.Empty;
        public string Ram = string.Empty;
        public double RamNumber = 0;
        public string UserName = string.Empty;

        public SystemInfos()
        {
            ManagementObjectSearcher osSearcher = new("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject osInfo in osSearcher.Get().Cast<ManagementObject>())
            {
                SystemName = osInfo["Caption"].ToString().Replace("Microsoft", "");
                SystemVersion = osInfo["Version"].ToString();
            }
            ManagementObjectSearcher cpuSearcher = new("SELECT * FROM Win32_Processor");
            foreach (ManagementObject cpuInfo in cpuSearcher.Get().Cast<ManagementObject>())
            {
                ModelOfCPU = cpuInfo["Name"].ToString();
            }
            ManagementObjectSearcher noNameSearcher = new("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject noNameInfo in noNameSearcher.Get().Cast<ManagementObject>())
            {
                Ram = Math.Round(Convert.ToDouble(noNameInfo["TotalPhysicalMemory"]) / 1024 / 1024 / 1024, 2).ToString() + " GB";
                RamNumber = Math.Round(Convert.ToDouble(noNameInfo["TotalPhysicalMemory"]) / 1024 / 1024);
                UserName = noNameInfo[nameof(UserName)].ToString();
            }
        }
    }
}
