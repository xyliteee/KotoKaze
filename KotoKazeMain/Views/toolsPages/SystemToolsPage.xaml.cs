using KotoKaze.Dynamic;
using KotoKaze.Static;
using Translation;
using KotoKaze.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static KotoKaze.Dynamic.BackgroundTaskList;


namespace KotoKaze.Views.toolsPages
{
    /// <summary>
    /// SystemToolsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SystemToolsPage : Page
    {
        private CMDBackgroundTask? GPEDITTASK;
        private CMDBackgroundTask? SFCSCANNOW;
        private CMDBackgroundTask? GETBATTERYREPORT;
        public SystemToolsPage()
        {
            InitializeComponent();
        }

        private void EnableGPEDIT_Click(object sender, RoutedEventArgs e)
        {
            if (IsTaskRunning(GPEDITTASK)) 
            {
                KotoMessageBoxSingle.ShowDialog("该任务已存在,检查任务列表");
                return;
            }
            var r = KotoMessageBox.ShowDialog("这将会为系统添加组策略(gpedit.msc)管理，确定？");
            if (r.IsClose) return;
            if (r.IsYes)
            {
                GPEDITTASK = new() { Title = "组策略添加" };
                GPEDITTASK.Start();
                string[] commands = 
                [
                    "FOR %F IN (\"%SystemRoot%\\servicing\\Packages\\Microsoft-Windows-GroupPolicy-ClientTools-Package~*.mum\") DO (DISM /Online /NoRestart /Add-Package:\"%F\")",
                    "FOR %F IN (\"%SystemRoot%\\servicing\\Packages\\Microsoft-Windows-GroupPolicy-ClientExtensions-Package~*.mum\") DO (DISM /Online /NoRestart /Add-Package:\"%F\")"
                ];
                GPEDITTASK.CommandWrite(commands);
                GPEDITTASK.StreamProcess();
            }
        }
        private void SFCSCNOW_Click(object sender, RoutedEventArgs e)
        {
            if (IsTaskRunning(SFCSCANNOW))
            {
                KotoMessageBoxSingle.ShowDialog("该任务已存在,检查任务列表");
                return;
            }
            var r = KotoMessageBox.ShowDialog("这将会使用系统自带的修复命令，确定？");
            if (r.IsClose) return;
            if (r.IsYes)
            {
                SFCSCANNOW = new() { Title = "SFC系统修复" };
                SFCSCANNOW.Start();
                SFCSCANNOW.CommandWrite(["SFC /SCANNOW"]);
                SFCSCANNOW.StreamProcess();
            }
        }
        private void BATTERYINFO_Click(object sender, RoutedEventArgs e)
        {
            if (IsTaskRunning(GETBATTERYREPORT))
            {
                KotoMessageBoxSingle.ShowDialog("该任务已存在,检查任务列表");
                return;
            }
            GETBATTERYREPORT = new() { Title = "获取电池报告" };
            Task.Run(() => 
            { 
                GETBATTERYREPORT.Start();
                string reportFilePath = Path.Combine(FileManager.WorkDirectory.BinDirectory, "BatteryReport.html");
                string reportFilePathTemp = Path.Combine(FileManager.WorkDirectory.softwareTempDirectory, "BatteryReport.html");
                File.Delete(reportFilePathTemp);
                GETBATTERYREPORT.CommandWrite([$"powercfg /batteryreport /output \"{reportFilePathTemp}\""]);
                GETBATTERYREPORT.Description = "正在生成报告";
                GETBATTERYREPORT.taskProcess.WaitForExit();
                string reportText = File.ReadAllText(reportFilePathTemp);

                GETBATTERYREPORT.Description = "正在格式化报告";
                string reportTextToChinese = TranslationRules.Translate(reportText, TranslationRules.batteryReport);

                GETBATTERYREPORT.SetFinished();
                File.Delete(reportFilePath);
                File.WriteAllText(reportFilePath, reportTextToChinese);

                Dispatcher.Invoke(() =>
                {
                    var r = KotoMessageBox.ShowDialog("报告文件已保存在程序目录的/Bin文件夹下,是否打开？");
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
