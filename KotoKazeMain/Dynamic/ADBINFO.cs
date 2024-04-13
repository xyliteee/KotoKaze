using KotoKaze.Static;
using KotoKaze.Windows;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace KotoKaze.Dynamic
{
    public class ADBINFO
    {
        private static readonly string adb = Path.Combine(FileManager.WorkDirectory.BinDirectory, "platform-tools/adb.exe");
        
        public class PhoneInfo
        {
            public bool isConnected = true;
            public string model = string.Empty;
            public string brand  = string.Empty;
            public string name = string.Empty;
            public string resolutionRatio = string.Empty;
            public string DPI = string.Empty;
            public string androidID = string.Empty;
            public string androidVersion = string.Empty;
            public string memTotal = string.Empty;
        }

        private static List<string> GetLineBack(string cmd)
        {
            List<string> results = [];
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
                    streamWriter.WriteLine(cmd);
                }
            }
            while (GlobalData.IsRunning)
            {
                string? temp = process.StandardOutput.ReadLine();
                if (temp == null) { break; }
                else if (temp == string.Empty || temp.Contains("Microsoft") || temp.Contains(":\\")) { continue; }
                else { results.Add(temp); }
            }
            Debug.WriteLine(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
            return results;
        }
        private static string GetNumber(string input) 
        {
            string output = string.Empty;
            string pattern = @"\d+"; // 正则表达式匹配连续的数字

            MatchCollection matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                output = match.Value;
                return output;
            }
            return output;
        }
        private static string SearchInList(List<string> sections,string section) 
        {
            string output = string.Empty;
            foreach (string temp in sections) 
            {
                if (temp.Contains(section)) 
                {
                    output = temp;
                }
            }
            return output;
        }
        public static PhoneInfo GetPhoneInfomation()
        {
            PhoneInfo phoneInfo = new();
            List<string> tempValues;
            tempValues = GetLineBack($"{adb} shell getprop ro.product.name");
            if (tempValues.Count==0)
            {
                phoneInfo.isConnected = false;
                return phoneInfo;
            }
            phoneInfo.name = tempValues[0];

            tempValues = GetLineBack($"{adb} shell getprop ro.product.brand");
            if (tempValues.Count == 0 )
            {
                phoneInfo.isConnected = false;
                return phoneInfo;
            }
            phoneInfo.brand = tempValues[0];

            tempValues = GetLineBack($"{adb} shell getprop ro.product.model");
            if (tempValues.Count == 0)
            {
                phoneInfo.isConnected = false;
                return phoneInfo;
            }
            phoneInfo.model = tempValues[0];

            tempValues = GetLineBack($"{adb} shell wm size");
            if (tempValues.Count == 0)
            {
                phoneInfo.isConnected = false;
                return phoneInfo;
            }
            phoneInfo.resolutionRatio = tempValues[0].Replace("Physical size: ", "");

            tempValues = GetLineBack($"{adb} shell wm density");
            if (tempValues.Count == 0)
            {
                phoneInfo.isConnected = false;
                return phoneInfo;
            }
            phoneInfo.DPI = tempValues[0].Replace("Physical density: ", "");

            tempValues = GetLineBack($"{adb} shell settings get secure android_id");
            if (tempValues.Count == 0)
            {
                phoneInfo.isConnected = false;
                return phoneInfo;
            }
            phoneInfo.androidID = tempValues[0];

            tempValues = GetLineBack($"{adb} shell getprop ro.build.version.release");
            if (tempValues.Count == 0)
            {
                phoneInfo.isConnected = false;
                return phoneInfo;
            }
            phoneInfo.androidVersion = tempValues[0];

            phoneInfo.memTotal = GetNumber(SearchInList(GetLineBack($"{adb} shell cat /proc/meminfo"), "MemTotal"));

            return phoneInfo;
        }

        public static void InstallAPK(string filePath,CMDBackgroundTask APKINSTALLTASK) 
        {
            if (GlobalData.TasksList.Contains(APKINSTALLTASK))
            {
                KotoMessageBoxSingle.ShowDialog("该任务已存在,检查任务列表");
                return;
            }
            APKINSTALLTASK = new() { Title = "安装APK应用程序" };
            GlobalData.TasksList.Add(APKINSTALLTASK);
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
                APKINSTALLTASK.taskProcess = process;

                process.Start();
                using (StreamWriter streamWriter = process.StandardInput)
                {
                    if (streamWriter.BaseStream.CanWrite)
                    {
                        streamWriter.WriteLine($"{adb} install {filePath}");
                    }
                }
                APKINSTALLTASK.outputThread = new(() =>
                {
                    APKINSTALLTASK.Description = "Performing Streamed Install....";
                });
                APKINSTALLTASK.StreamProcess();
            });
        }
    }
}
