﻿using KotoKaze.Dynamic;
using KotoKaze.Static;
using Translation;
using KotoKaze.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static KotoKaze.Dynamic.BackgroundTaskList;
using FileControl;
using System.Text.RegularExpressions;


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
                GPEDITTASK.outputThreadAction = new(() => 
                {
                    try
                    {

                        Task<string?> readLineTask = GPEDITTASK.taskProcess.StandardOutput.ReadLineAsync();
                        int index = 1;
                        double percentage;
                        while (GlobalData.IsRunning)
                        {
                            if (GPEDITTASK.isCancle || GPEDITTASK.isError || GPEDITTASK.isFinished) break;
                            if (!readLineTask.IsCompleted)
                            {
                                Thread.Sleep(16);
                                continue;
                            }
                            if (string.IsNullOrEmpty(readLineTask.Result) || !readLineTask.Result.StartsWith('['))
                            {
                                readLineTask = GPEDITTASK.taskProcess.StandardOutput.ReadLineAsync();
                                continue;
                            }
                            var match = MyRegex().Match(readLineTask.Result);
                            if (!match.Success)
                            {
                                readLineTask = GPEDITTASK.taskProcess.StandardOutput.ReadLineAsync();
                                continue;
                            }
                            percentage = double.Parse(match.Value);
                            GPEDITTASK.Description = $"正在安装第{index}/4个包：{percentage}% [{new string('*', (int)(percentage * 0.2))}]";
                            if (percentage == 100) index++;
                            readLineTask = GPEDITTASK.taskProcess.StandardOutput.ReadLineAsync();
                        }

                    }
                    catch (InvalidOperationException) { }
                });
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
                KotoMessageBoxSingle.ShowDialog("该任务耗时非常长，进程不由本程序接管\n注意检查弹出的窗口");
                SFCSCANNOW = new() { Title = "SFC系统修复" };
                SFCSCANNOW.taskProcess.StartInfo = CMDBackgroundTask.startInfoWithWindow;
                SFCSCANNOW.errorThreadAction = new(() => { });
                SFCSCANNOW.outputThreadAction = new(() => { });
                SFCSCANNOW.Start();
                SFCSCANNOW.CommandWrite(["sfc /scannow"]);
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
                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                GETBATTERYREPORT.Start();
                string reportFilePath = Path.Combine(FileManager.WorkDirectory.outPutDirectory, $"BatteryReport/{time}.html");
                string reportFilePathTemp = Path.Combine(FileManager.WorkDirectory.softwareTempDirectory, "BatteryReport.html");
                File.Delete(reportFilePathTemp);
                GETBATTERYREPORT.CommandWrite([$"powercfg /batteryreport /output \"{reportFilePathTemp}\""]);
                GETBATTERYREPORT.Description = "正在生成报告";
                GETBATTERYREPORT.taskProcess.WaitForExit();
                string reportText = File.ReadAllText(reportFilePathTemp);

                GETBATTERYREPORT.Description = "正在格式化报告";
                string reportTextToChinese = TranslationRules.Translate(reportText, TranslationRules.batteryReport);

                GETBATTERYREPORT.SetFinished();
                File.WriteAllText(reportFilePath, reportTextToChinese);

                Dispatcher.Invoke(() =>
                {
                    var r = KotoMessageBox.ShowDialog("报告文件已保存在程序目录的/Output文件夹下,是否打开？");
                    if (r.IsClose) return;
                    if (r.IsYes)
                    {
                        Process.Start(new ProcessStartInfo("cmd", $"/c start \"\" \"{reportFilePath}\"") { CreateNoWindow = true });
                    }
                });
            });
        }

        [GeneratedRegex(@"\d+")]
        private static partial Regex MyRegex();

        private void ReturnToBios_Click(object sender, RoutedEventArgs e)
        {
            Process process = new()
            {
                StartInfo = CMDBackgroundTask.startInfo
            };
            process.Start();
            using (StreamWriter streamWriter = process.StandardInput)
            {
                string cmd = "shutdown /r /fw /t 1";
                if (streamWriter.BaseStream.CanWrite)
                {
                    streamWriter.WriteLine(cmd);
                }
            }
            process.WaitForExit();
            process.Close();
        }
    }
}
