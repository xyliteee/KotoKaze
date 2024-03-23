﻿using KotoKaze.Dynamic;
using KotoKaze.Static;
using KotoKaze.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static HandyControl.Tools.Interop.InteropValues;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KotoKaze.Views.toolsPages
{
    /// <summary>
    /// SystemToolsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SystemToolsPage : Page
    {
        private readonly BackgroundTask GPEDITTASK = new() {title = "组策略添加" };
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

                    // 读取标准输出
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

        [GeneratedRegex(@"\d+\.\d+")]
        private static partial Regex MyRegex();
    }
}
