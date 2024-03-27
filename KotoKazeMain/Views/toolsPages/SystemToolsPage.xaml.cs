using KotoKaze.Dynamic;
using KotoKaze.Static;
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
        private readonly BackgroundTask GPEDITTASK = new() { title = "组策略添加" };
        private readonly BackgroundTask SFCSCANNOW = new() { title = "SFC系统修复" };
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
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };

                    Process process = new() { StartInfo = startInfo };
                    process.Start();

                    using (StreamWriter streamWriter = process.StandardInput)
                    {
                        if (streamWriter.BaseStream.CanWrite)
                        {
                            streamWriter.WriteLine("FOR %F IN (\"%SystemRoot%\\servicing\\Packages\\Microsoft-Windows-GroupPolicy-ClientTools-Package~*.mum\") DO (DISM /Online /NoRestart /Add-Package:\"%F\")");
                            streamWriter.WriteLine("FOR %F IN (\"%SystemRoot%\\servicing\\Packages\\Microsoft-Windows-GroupPolicy-ClientExtensions-Package~*.mum\") DO (DISM /Online /NoRestart /Add-Package:\"%F\")");
                        }
                    }

                    using StreamReader reader = process.StandardOutput;
                    string result;

                    while (GlobalData.IsRunning && ((result = reader.ReadLine())!=null)) 
                    {
                        GPEDITTASK.description = result;
                        if (process.HasExited)
                        {
                            GlobalData.TasksList.Remove(GPEDITTASK);
                            Dispatcher.Invoke(() => { KotoMessageBoxSingle.ShowDialog("组策略添加完成"); });
                            break;
                        }
                    }
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
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    Process process = new() { StartInfo = startInfo };
                    process.Start();

                    using (StreamWriter streamWriter = process.StandardInput)
                    {
                        if (streamWriter.BaseStream.CanWrite)
                        {
                            streamWriter.WriteLine("SFC /SCANNOW");
                        }
                    }

                    using StreamReader reader = process.StandardOutput;
                    string result;
                    while (GlobalData.IsRunning && ((result = reader.ReadLine()) != null))
                    {
                        SFCSCANNOW.description = result;
                        if (process.HasExited)
                        {
                            GlobalData.TasksList.Remove(SFCSCANNOW);
                            Dispatcher.Invoke(() => { KotoMessageBoxSingle.ShowDialog("已执行修复命令"); });
                            break;
                        }
                    }
                });
            }
        }
        private void BATTERYINFO_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process process = new() { StartInfo = startInfo };
                process.Start();

                string reportFilePath = System.IO.Path.Combine(FileManager.WorkDirectory.localDataDirectory, "BatteryReport.html");
                string reportFilePathTemp = System.IO.Path.Combine(FileManager.WorkDirectory.softwareTempDirectory, "BatteryReport.html");
                File.Delete(reportFilePathTemp);
                using StreamWriter streamWriter = process.StandardInput;
                if (streamWriter.BaseStream.CanWrite)
                {
                    streamWriter.WriteLine($"powercfg /batteryreport /output \"{reportFilePathTemp}\"");
                    streamWriter.WriteLine("exit");
                }

                process.WaitForExit();
                string reportText = File.ReadAllText(reportFilePathTemp);

                string reportTextToChinese = TranslationRules.Translate(reportText, TranslationRules.batteryReport);

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
