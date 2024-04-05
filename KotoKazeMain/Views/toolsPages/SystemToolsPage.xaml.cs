using KotoKaze.Dynamic;
using KotoKaze.Static;
using Translation;
using KotoKaze.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace KotoKaze.Views.toolsPages
{
    /// <summary>
    /// SystemToolsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SystemToolsPage : Page
    {
        private BackgroundTask GPEDITTASK = new();
        private BackgroundTask SFCSCANNOW = new();
        private BackgroundTask GETBATTERYREPORT = new();
        public SystemToolsPage()
        {
            InitializeComponent();
        }

        private void EnableGPEDIT_Click(object sender, RoutedEventArgs e)
        {

            if (GlobalData.TasksList.Contains(GPEDITTASK)) 
            {
                KotoMessageBoxSingle.ShowDialog("该任务已存在,检查任务列表");
                return;
            }
            GPEDITTASK = new() { Title = "组策略添加" };
            var r = KotoMessageBox.ShowDialog("这将会为系统添加组策略(gpedit.msc)管理，确定？");
            if (r.IsClose) return;
            if (r.IsYes)
            {
                GlobalData.TasksList.Add(GPEDITTASK);
                Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new()
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };

                    Process process = new() { StartInfo = startInfo };
                    GPEDITTASK.taskProcess = process;

                    process.Start();

                    using (StreamWriter streamWriter = process.StandardInput)
                    {
                        if (streamWriter.BaseStream.CanWrite)
                        {
                            streamWriter.WriteLine("FOR %F IN (\"%SystemRoot%\\servicing\\Packages\\Microsoft-Windows-GroupPolicy-ClientTools-Package~*.mum\") DO (DISM /Online /NoRestart /Add-Package:\"%F\")");
                            streamWriter.WriteLine("FOR %F IN (\"%SystemRoot%\\servicing\\Packages\\Microsoft-Windows-GroupPolicy-ClientExtensions-Package~*.mum\") DO (DISM /Online /NoRestart /Add-Package:\"%F\")");
                        }
                    }

                    GPEDITTASK.StreamProcess();
                });
            }

        }
        private void SFCSCNOW_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalData.TasksList.Contains(SFCSCANNOW))
            {
                KotoMessageBoxSingle.ShowDialog("该任务已存在,检查任务列表");
                return;
            }
            SFCSCANNOW = new() { Title = "SFC系统修复" };
            var r = KotoMessageBox.ShowDialog("这将会使用系统自带的修复命令，确定？");
            if (r.IsClose) return;
            if (r.IsYes)
            {
                GlobalData.TasksList.Add(SFCSCANNOW);
                Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new()
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    Process process = new() { StartInfo = startInfo };
                    SFCSCANNOW.taskProcess = process;
                    process.Start();

                    using (StreamWriter streamWriter = process.StandardInput)
                    {
                        if (streamWriter.BaseStream.CanWrite)
                        {
                            streamWriter.WriteLine("SFC /SCANNOW");
                        }
                    }

                    SFCSCANNOW.StreamProcess();
                });
            }
        }
        private void BATTERYINFO_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalData.TasksList.Contains(GETBATTERYREPORT))
            {
                KotoMessageBoxSingle.ShowDialog("该任务已存在,检查任务列表");
                return;
            }
            GETBATTERYREPORT = new() { Title = "获取电池报告" };
            Task.Run(() =>
            {
                GlobalData.TasksList.Add(GETBATTERYREPORT);
                ProcessStartInfo startInfo = new()
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process process = new() { StartInfo = startInfo };
                GETBATTERYREPORT.taskProcess = process;
                process.Start();

                string reportFilePath = Path.Combine(FileManager.WorkDirectory.localDataDirectory, "BatteryReport.html");
                string reportFilePathTemp = Path.Combine(FileManager.WorkDirectory.softwareTempDirectory, "BatteryReport.html");
                File.Delete(reportFilePathTemp);
                using (StreamWriter streamWriter = process.StandardInput)
                {
                    if (streamWriter.BaseStream.CanWrite)
                    {
                        streamWriter.WriteLine($"powercfg /batteryreport /output \"{reportFilePathTemp}\"");
                    }
                };
                GETBATTERYREPORT.Description = "正在生成报告";
                process.WaitForExit();
                string reportText = File.ReadAllText(reportFilePathTemp);

                GETBATTERYREPORT.Description = "正在格式化报告";
                string reportTextToChinese = TranslationRules.Translate(reportText, TranslationRules.batteryReport);

                GETBATTERYREPORT.SetFinal();
                File.Delete(reportFilePath);
                File.WriteAllText(reportFilePath, reportTextToChinese);

                Dispatcher.Invoke(() =>
                {
                    var r = KotoMessageBox.ShowDialog("报告文件已保存在程序目录的/LocalData文件夹下,是否打开？");
                    if (r.IsClose) return;
                    if (r.IsYes)
                    {
                        Process.Start(new ProcessStartInfo("cmd", $"/c start \"\" \"{reportFilePath}\"") { CreateNoWindow = true });
                    }
                });
            });
        }
    }
}
