
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
        private readonly SystemInfo systemInfo = new();
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

            GetCurrentRam();
        }
        private void GetCurrentRam()
        {
            Thread thread = new(() =>
            {
                double memoryAvailable;
                double memoryUsed;
                double diskTotal;
                double diskAvailable;
                double diskUsed;
                int ramUseRate = 0;
                int diskUseRate = 0;
                double cpuUsage = 0;
                PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");
                PerformanceCounter ramCounter = new("Memory", "Available MBytes");
                try
                {
                    while (true)
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

                        Dispatcher.Invoke(() =>
                        {
                            cpuCircleBar.Value = cpuUsage;
                            cpuLabel.Content = $"CPU占用{(int)cpuUsage}%";
                            ramBar.Value = ramUseRate;
                            ramLabel.Content = $"内存使用情况：{Math.Round(memoryUsed/1024,1)}GB / {Math.Round(systemInfo.RamNumber / 1024, 1)}GB";
                            DiskBar.Value = diskUseRate;
                            diskLabel.Content = $"系统分区使用情况：{(int)(diskUsed/1024/1024/1024)}GB / {(int)(diskTotal/1024/1024/1024)}GB";
                        });
                    }
                }
                catch (ThreadAbortException) { }
                catch (TaskCanceledException) { }
            });
            thread.Start();
        }
    }



    public class SystemInfo
    {
        public string SystemName = string.Empty;
        public string SystemVersion = string.Empty;
        public string ModelOfCPU = string.Empty;
        public string Ram = string.Empty;
        public double RamNumber = 0;
        public string UserName = string.Empty;

        public SystemInfo()
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
