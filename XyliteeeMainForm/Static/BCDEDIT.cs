using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static KotoKaze.Static.BCDEDIT;

namespace KotoKaze.Static
{
    
    internal class BCDEDIT
    {
        public class BCDInfo 
        {
            public List<BootInfo> BootInfos = [];
            public List<SystemInfo> SystemInfos = [];
        }
        public class BootInfo
        {
            public string device = string.Empty;
            public string partition = string.Empty;
            public string path = string.Empty;
            public string description = string.Empty;
            public string locale = string.Empty;
            public string inherit = string.Empty;
            public string flightsigning = string.Empty;
            public string defaultSetting = string.Empty;
            public string resumeobject = string.Empty;
            public string displayorder = string.Empty;
            public string toolsdisplayorder = string.Empty;
            public string timeout = string.Empty;
        }
        public class SystemInfo
        {
            public string flag = string.Empty;
            public string device = string.Empty;
            public string partition = string.Empty;
            public string path = string.Empty;
            public string description = string.Empty;
            public string locale = string.Empty;
            public string inherit = string.Empty;
            public string recoverysequence = string.Empty;
            public string displaymessageoverride = string.Empty;
            public string recoveryenabled = string.Empty;
            public string isolatedcontext = string.Empty;
            public string allowedinmemorysettings = string.Empty;
            public string osdevice = string.Empty;
            public string systemroot = string.Empty;
            public string resumeobject = string.Empty;
            public string nx = string.Empty;
            public string bootmenupolicy = string.Empty;
            public string hypervisorlaunchtype = string.Empty;
        }

        internal static readonly string[] separator = ["-------------------"];
        internal static readonly string[] separatorArray = ["\r\n", "\n"];

        static public BCDInfo GetBCDInformation() 
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();

            using (StreamWriter streamWriter = process.StandardInput)
            {
                if (streamWriter.BaseStream.CanWrite)
                {
                    streamWriter.WriteLine("bcdedit /enum");
                    streamWriter.WriteLine("exit");
                }
            }

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] sections = output.Split(separator, StringSplitOptions.None);

            List<string> desiredSections = [];

            for (int i = 1; i < sections.Length; i++)
            {
                desiredSections.Add(sections[i]);
            }

            List<BootInfo> bootInfos = [];
            List<SystemInfo> systemInfos = [];

            foreach (string section in desiredSections)
            {
                if (string.IsNullOrWhiteSpace(section))
                    continue;

                string[] lines = section.Split(separatorArray, StringSplitOptions.None);
                if (lines.Any(line => line.StartsWith("timeout")))
                {
                    BootInfo bootInfo = new();
                    foreach (string line in lines)
                    {
                        string key = "缺省";
                        string value = "缺省";
                        int index = line.IndexOf(' ');
                        if (index > 0)
                        {
                            key = line.Substring(0, index).Trim();
                            value = line.Substring(index).Trim();
                        }

                        if (key == "device")
                            bootInfo.device = value;
                        else if (key == "partition")
                            bootInfo.partition = value;
                        else if (key == "path")
                            bootInfo.path = value;
                        else if (key == "description")
                            bootInfo.description = value;
                        else if (key == "locale")
                            bootInfo.locale = value;
                        else if (key == "inherit")
                            bootInfo.inherit = value;
                        else if (key == "flightsigning")
                            bootInfo.flightsigning = value;
                        else if (key == "default")
                            bootInfo.defaultSetting = value;
                        else if (key == "resumeobject")
                            bootInfo.resumeobject = value;
                        else if (key == "displayorder")
                            bootInfo.displayorder = value;
                        else if (key == "toolsdisplayorder")
                            bootInfo.toolsdisplayorder = value;
                        else if (key == "timeout")
                            bootInfo.timeout = value;
                    }
                    bootInfos.Add(bootInfo);
                }
                else
                {
                    SystemInfo systemInfo = new();
                    foreach (string line in lines)
                    {
                        string key = "缺省";
                        string value = "缺省";
                        int index = line.IndexOf(' ');
                        if (index > 0)
                        {
                            key = line.Substring(0, index).Trim();
                            value = line.Substring(index).Trim();
                        }

                        if (key == "标识符")
                            systemInfo.flag = value;
                        else if (key == "device")
                            systemInfo.device = value;
                        else if (key == "partition")
                            systemInfo.partition = value;
                        else if (key == "path")
                            systemInfo.path = value;
                        else if (key == "description")
                            systemInfo.description = value;
                        else if (key == "locale")
                            systemInfo.locale = value;
                        else if (key == "inherit")
                            systemInfo.inherit = value;
                        else if (key == "recoverysequence")
                            systemInfo.recoverysequence = value;
                        else if (key == "displaymessageoverride")
                            systemInfo.displaymessageoverride = value;
                        else if (key == "recoveryenabled")
                            systemInfo.recoveryenabled = value;
                        else if (key == "isolatedcontext")
                            systemInfo.isolatedcontext = value;
                        else if (key == "allowedinmemorysettings")
                            systemInfo.allowedinmemorysettings = value;
                        else if (key == "osdevice")
                            systemInfo.osdevice = value;
                        else if (key == "systemroot")
                            systemInfo.systemroot = value;
                        else if (key == "resumeobject")
                            systemInfo.resumeobject = value;
                        else if (key == "nx")
                            systemInfo.nx = value;
                        else if (key == "bootmenupolicy")
                            systemInfo.bootmenupolicy = value;
                        else if (key == "hypervisorlaunchtype")
                            systemInfo.hypervisorlaunchtype = value;
                    }
                    systemInfos.Add(systemInfo);
                }
            }
            BCDInfo bCDInfo = new() { BootInfos = bootInfos, SystemInfos = systemInfos };
            return bCDInfo;
        }

        static public void DeleteBCDInformation(SystemInfo systemInfo) 
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

            using StreamWriter streamWriter = process.StandardInput;
            if (streamWriter.BaseStream.CanWrite)
            {
                streamWriter.WriteLine($"bcdedit /delete {systemInfo.flag}");
                streamWriter.WriteLine("exit");
            }

        }

        [Serializable]
        public class BCDBackUPFile
        {
            public string CheckCode { get; set; }
            public SystemInfo SystemInfo { get; set; }
            public BCDBackUPFile(SystemInfo systemInfo)
            {
                CheckCode = systemInfo.flag.Replace("{","").Replace("}","");
                SystemInfo = systemInfo;
            }
        }

        public static void SaveBBF(BCDBackUPFile BBF)
        {
            string jsonString = JsonSerializer.Serialize(BBF);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            string base64String = Convert.ToBase64String(jsonBytes);
            File.WriteAllText($"/Backup/{BBF.CheckCode}.BBF", base64String);//修改路径和文件名字
        }

        public static BCDBackUPFile ReadBBF(string fullFilePath)
        {
            BCDBackUPFile cacheFile = new(new SystemInfo());
            if (Path.Exists(fullFilePath))
            {
                try
                {
                    string base64String = File.ReadAllText(fullFilePath);
                    byte[] jsonBytes = Convert.FromBase64String(base64String);
                    string jsonString = Encoding.UTF8.GetString(jsonBytes);
                    cacheFile = JsonSerializer.Deserialize<BCDBackUPFile>(jsonString)!;
                }
                catch
                {
                    //被修改
                }
            }
            return cacheFile;
        }
    }
}
