using KotoKaze.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static KotoKaze.Dynamic.BCDEDIT;

namespace KotoKaze.Dynamic
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
            public string Flag { get; set; } = string.Empty;
            public string Device { get; set; } = string.Empty;
            public string Flightsigning { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Locale { get; set; } = string.Empty;
            public string Inherit { get; set; } = string.Empty;
            public string Recoverysequence { get; set; } = string.Empty;
            public string Displaymessageoverride { get; set; } = string.Empty;
            public string Recoveryenabled { get; set; } = string.Empty;
            public string Isolatedcontext { get; set; } = string.Empty;
            public string Allowedinmemorysettings { get; set; } = string.Empty;
            public string Osdevice { get; set; } = string.Empty;
            public string Systemroot { get; set; } = string.Empty;
            public string Resumeobject { get; set; } = string.Empty;
            public string Nx { get; set; } = string.Empty;
            public string Bootmenupolicy { get; set; } = string.Empty;
            public string Hypervisorlaunchtype { get; set; } = string.Empty;
            public string Winpe { get; set; } = string.Empty;
            public string OriginalInfomation { get; set; } = string.Empty;
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

            Process process = new() { StartInfo = startInfo };
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
                    SystemInfo systemInfo = new()
                    {
                        OriginalInfomation = section
                    };
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
                            systemInfo.Flag = value;
                        else if (key == "device")
                            systemInfo.Device = value;
                        else if (key == "flightsigning")
                            systemInfo.Flightsigning = value;
                        else if (key == "path")
                            systemInfo.Path = value;
                        else if (key == "description")
                            systemInfo.Description = value;
                        else if (key == "locale")
                            systemInfo.Locale = value;
                        else if (key == "inherit")
                            systemInfo.Inherit = value;
                        else if (key == "recoverysequence")
                            systemInfo.Recoverysequence = value;
                        else if (key == "displaymessageoverride")
                            systemInfo.Displaymessageoverride = value;
                        else if (key == "recoveryenabled")
                            systemInfo.Recoveryenabled = value;
                        else if (key == "isolatedcontext")
                            systemInfo.Isolatedcontext = value;
                        else if (key == "allowedinmemorysettings")
                            systemInfo.Allowedinmemorysettings = value;
                        else if (key == "osdevice")
                            systemInfo.Osdevice = value;
                        else if (key == "systemroot")
                            systemInfo.Systemroot = value;
                        else if (key == "resumeobject")
                            systemInfo.Resumeobject = value;
                        else if (key == "nx")
                            systemInfo.Nx = value;
                        else if (key == "bootmenupolicy")
                            systemInfo.Bootmenupolicy = value;
                        else if (key == "hypervisorlaunchtype")
                            systemInfo.Hypervisorlaunchtype = value;
                        else if (key == "winpe")
                            systemInfo.Winpe = value;
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
                streamWriter.WriteLine($"bcdedit /delete {systemInfo.Flag}");
                streamWriter.WriteLine("exit");
            }

        }
        public class BCDBackUPFile
        {
            public string CheckCode { get; set; }
            public SystemInfo SystemInfo { get; set; }
            public BCDBackUPFile(SystemInfo systemInfo)
            {
                CheckCode = systemInfo.Flag.Replace("{", "").Replace("}", "");
                SystemInfo = systemInfo;
            }
        }

        public static async Task SaveBBF(BCDBackUPFile BBF,string fileName)
        {
            string jsonString = JsonSerializer.Serialize(BBF);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            string base64String = Convert.ToBase64String(jsonBytes);
            string filePath = Path.Combine(FileManager.WorkDirectory.backupDirectory, $"{fileName}.BBF");
            await File.WriteAllTextAsync(filePath, base64String);
        }

        public static async Task SaveOriginal(string originalString, string fileName)
        {
            string filePath = Path.Combine(FileManager.WorkDirectory.backupDirectory, $"{fileName}.txt");
            await File.WriteAllTextAsync(filePath, originalString);
        }

        public static BCDBackUPFile ReadBBF(string fullFilePath)
        {
            BCDBackUPFile BBFFile = new(new SystemInfo());
            if (Path.Exists(fullFilePath))
            {
                try
                {
                    string base64String = File.ReadAllText(fullFilePath);
                    byte[] jsonBytes = Convert.FromBase64String(base64String);
                    string jsonString = Encoding.UTF8.GetString(jsonBytes);
                    BBFFile = JsonSerializer.Deserialize<BCDBackUPFile>(jsonString)!;
                }
                catch
                {
                    //被修改
                }
            }
            return BBFFile;
        }

        public static bool ImportSystemBootInfo(string description,BCDBackUPFile BBF) 
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
                    streamWriter.WriteLine($"bcdedit /create /d \"{description}\" /application osloader");
                    streamWriter.WriteLine("exit");
                }
            }
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (!output.Contains("成功创建")) { return false; }

            Match match = Regex.Match(output, @"\{(.+?)\}");
            string id = match.Groups[1].Value;
            string GUID = "{" + id + "}";

            process.Start();
            using (StreamWriter streamWriter = process.StandardInput)
            {
                if (streamWriter.BaseStream.CanWrite)
                {
                    void WriteLineIfNotEmpty(string parameter, string value)
                    {
                        if (!string.IsNullOrEmpty(value)){streamWriter.WriteLine($"bcdedit /set {GUID} {parameter} {value}");}
                    }

                    WriteLineIfNotEmpty("device", BBF.SystemInfo.Device);
                    WriteLineIfNotEmpty("path", BBF.SystemInfo.Path);
                    WriteLineIfNotEmpty("locale", BBF.SystemInfo.Locale);
                    WriteLineIfNotEmpty("inherit", BBF.SystemInfo.Inherit);
                    WriteLineIfNotEmpty("recoverysequence", GUID);
                    WriteLineIfNotEmpty("displaymessageoverride", BBF.SystemInfo.Displaymessageoverride);
                    WriteLineIfNotEmpty("recoveryenabled", BBF.SystemInfo.Recoveryenabled);
                    WriteLineIfNotEmpty("isolatedcontext", BBF.SystemInfo.Isolatedcontext);
                    WriteLineIfNotEmpty("flightsigning", BBF.SystemInfo.Flightsigning);
                    WriteLineIfNotEmpty("allowedinmemorysettings", BBF.SystemInfo.Allowedinmemorysettings);
                    WriteLineIfNotEmpty("osdevice", BBF.SystemInfo.Osdevice);
                    WriteLineIfNotEmpty("systemroot", BBF.SystemInfo.Systemroot);
                    WriteLineIfNotEmpty("resumeobject", GUID);
                    WriteLineIfNotEmpty("nx", BBF.SystemInfo.Nx);
                    WriteLineIfNotEmpty("bootmenupolicy", BBF.SystemInfo.Bootmenupolicy);
                    WriteLineIfNotEmpty("hypervisorlaunchtype", BBF.SystemInfo.Hypervisorlaunchtype);
                    streamWriter.WriteLine($"bcdedit /displayorder {GUID} /addlast");
                    streamWriter.WriteLine("exit");
                }
            }
            process.WaitForExit();
            return true;
        }

        public static void SetDefault(SystemInfo systemInfo) 
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
                streamWriter.WriteLine($"bcdedit /default {systemInfo.Flag}");
                streamWriter.WriteLine("exit");
            }
        }
    }
}
