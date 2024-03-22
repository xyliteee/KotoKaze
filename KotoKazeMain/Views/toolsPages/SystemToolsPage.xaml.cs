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
        
        public SystemToolsPage()
        {
            InitializeComponent();
        }

        private void EnableGPEDIT_Click(object sender, RoutedEventArgs e)
        {
            var r = KotoMessageBox.ShowDialog("这将会为系统添加组策略(gpedit.msc)管理，确定？");
            if (r.IsClose) return;
            if (r.IsYes)
            {
                KotoMessageBoxSingle.ShowDialog("已开启任务，将会在后台自动执行");
                GlobalData.TasksList.Add("组策略添加");
                Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new()
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
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
                    while (true) 
                    {
                        if (process.HasExited)
                        {
                            Dispatcher.Invoke(() => { KotoMessageBoxSingle.ShowDialog("已成功添加组策略"); });
                            GlobalData.TasksList.Remove("组策略添加");
                            break;
                        }
                    }
                });
            }

        }


        private void SFCSCNOW_Click(object sender, RoutedEventArgs e)
        {
            var r = KotoMessageBox.ShowDialog("这将会使用系统自带的修复命令，确定？");
            if (r.IsClose) return;
            if (r.IsYes)
            {
                Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new()
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = false,
                        UseShellExecute = false
                    };

                    Process process = new Process { StartInfo = startInfo };
                    process.Start();

                    using (StreamWriter streamWriter = process.StandardInput)
                    {
                        if (streamWriter.BaseStream.CanWrite)
                        {
                            streamWriter.WriteLine("sfc /scannow");
                        }
                    }

                    // 读取标准输出
                    using StreamReader reader = process.StandardOutput;
                    string result;
                    while ((result = reader.ReadLine()) != null)
                    {
                        Debug.WriteLine(result);
                    }
                });
            }
        }

        [GeneratedRegex(@"\d+\.\d+")]
        private static partial Regex MyRegex();
    }
}
