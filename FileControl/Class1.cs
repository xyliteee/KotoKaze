using System.IO.Compression;
using System.IO;
using System.Text.Json;
using System.Text;
using CleanContent;

namespace FileControl
{
    public class FileManager
    {
        public static class WorkDirectory
        {
            public static readonly string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            public static readonly string backupDirectory = Path.Combine(rootDirectory, "Backup");
            public static readonly string BinDirectory = Path.Combine(rootDirectory, "Bin");
            public static readonly string logfileDirectory = Path.Combine(rootDirectory, "logs");
            public static readonly string outPutDirectory = Path.Combine(rootDirectory, "Output");
            public static readonly string softwareTempDirectory = Path.Combine(CleanRules.APPDATE, "Local\\Temp\\KotoKaze");

            public static void CreatWorkDirectory()
            {
                Directory.CreateDirectory(backupDirectory);
                Directory.CreateDirectory(BinDirectory);
                Directory.CreateDirectory(Path.Combine(outPutDirectory, "BatteryReport"));
                Directory.CreateDirectory(Path.Combine(outPutDirectory, "RecordData"));
                Directory.CreateDirectory(softwareTempDirectory);
                Directory.CreateDirectory(logfileDirectory);
                Directory.CreateDirectory(outPutDirectory);
            }


            public static void CreatWorkFile()
            {
                if (!Path.Exists(Path.Combine(BinDirectory, "Performance.ini")))
                {
                    IniManager.IniFileSetDefault("Performance.ini");
                }
                if (!Path.Exists(Path.Combine(BinDirectory, "Application.ini")))
                {
                    IniManager.IniFileSetDefault("Application.ini");
                }
            }
        }
        public static class IniManager
        {
            public static void IniFileSetDefault(string fileName)
            {
                switch (fileName)
                {
                    case "Performance.ini":
                        IniFileWrite("Performance.ini", "VALUE", "CPU_MULTI_SCORE", "");
                        IniFileWrite("Performance.ini", "VALUE", "CPU_SINGLE_SCORE", "");
                        IniFileWrite("Performance.ini", "VALUE", "GPU_FPS", "");
                        IniFileWrite("Performance.ini", "VALUE", "RAM_READ_SCORE", "");
                        IniFileWrite("Performance.ini", "VALUE", "RAM_WRITE_SCORE", "");
                        IniFileWrite("Performance.ini", "VALUE", "DISK_READ_SCORE", "");
                        IniFileWrite("Performance.ini", "VALUE", "DISK_WRITE_SCORE", "");
                        break;
                    case "Application.ini":
                        IniFileWrite("Application.ini", "SETTING", "ISFIRST_USE", "TRUE");
                        IniFileWrite("Application.ini", "SETTING", "REFRESH_TIME", "1");
                        IniFileWrite("Application.ini", "SETTING", "ANIMATION_LEVEL", "0");
                        IniFileWrite("Application.ini", "VALUE", "VERSION", "114.514");
                        break;
                }
            }

            public static void IniFileWrite(string fileName, string section, string key, string value)
            {
                section = section.ToUpper();
                key = key.ToUpper();
                value = Encryption.Encrypt(value.ToUpper());
                Dictionary<string, Dictionary<string, string>> data = [];
                string filePath = Path.Combine(WorkDirectory.BinDirectory, fileName);
                if (File.Exists(filePath))
                {
                    using StreamReader sr = new(filePath, Encoding.Default);
                    string currentSection = "";
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith('[') && line.EndsWith(']'))
                        {
                            currentSection = line[1..^1];
                            data[currentSection] = [];
                        }
                        else if (currentSection != "")
                        {
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                data[currentSection][parts[0].Trim()] = parts[1].Trim();
                            }
                        }
                    }
                }
                if (!data.ContainsKey(section))
                {
                    data[section] = [];
                }
                data[section][key] = value;

                using StreamWriter sw = new(filePath, false, Encoding.Default);
                foreach (var sectionPair in data)
                {
                    sw.WriteLine("[" + sectionPair.Key + "]");
                    foreach (var keyPair in sectionPair.Value)
                    {
                        sw.WriteLine(keyPair.Key + "=" + keyPair.Value);
                    }
                }
            }

            public static string IniFileRead(string fileName, string section, string key)
            {
                string filePath = Path.Combine(WorkDirectory.BinDirectory, fileName);
                section = section.ToUpper();
                key = key.ToUpper();
                Dictionary<string, Dictionary<string, string>> data = [];
                if (File.Exists(filePath))
                {
                    using StreamReader sr = new(filePath, Encoding.Default);
                    string currentSection = "";
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith('[') && line.EndsWith(']'))
                        {
                            currentSection = line.Substring(1, line.Length - 2);
                            data[currentSection] = [];
                        }
                        else if (currentSection != "")
                        {
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                data[currentSection][parts[0].Trim()] = parts[1].Trim();
                            }
                        }
                    }
                }

                if (data.ContainsKey(section) && data[section].ContainsKey(key))
                {
                    string result = data[section][key];
                    return Encryption.Decrypt(result);
                }
                return string.Empty;
            }
        }
        public static class Encryption
        {
            private static readonly string Key = "TestKey";

            public static string Encrypt(string originalString)
            {
                var result = new StringBuilder();
                for (int i = 0; i < originalString.Length; i++)
                {
                    char character = originalString[i];
                    char keyChar = Key[i % Key.Length];
                    char encryptedChar = (char)(character ^ keyChar);
                    result.Append(((int)encryptedChar).ToString("X2"));
                }

                return result.ToString();
            }

            public static string Decrypt(string encryptedString)
            {
                try
                {
                    var result = new StringBuilder();
                    for (int i = 0; i < encryptedString.Length; i += 2)
                    {
                        string hexNumber = encryptedString.Substring(i, 2);
                        int encryptedChar = Convert.ToInt32(hexNumber, 16);
                        char keyChar = Key[(i / 2) % Key.Length];
                        char decryptedChar = (char)(encryptedChar ^ keyChar);
                        result.Append(decryptedChar);
                    }
                    return result.ToString();
                }
                catch
                {
                    if (encryptedString != string.Empty)
                    {
                        encryptedString = "ERROR";
                    }
                    return encryptedString;
                }
            }
        }
        public static class LogManager
        {
            private readonly static JsonSerializerOptions jsonSerializerOptions = new()
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            public async static Task LogWriteAsync(string title, string message, string suggestion = "None")
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string fileName = time + ".log";
                var log = new { Title = title, Message = message, Time = time, Suggestion = suggestion };
                string json = JsonSerializer.Serialize(log, jsonSerializerOptions);
                string filePath = Path.Combine(WorkDirectory.logfileDirectory, fileName);
                await File.WriteAllTextAsync(filePath, json);
            }
            public static void LogWrite(string title, string message, string suggestion = "None")
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string fileName = time + ".log";
                var log = new { Title = title, Message = message, Time = time, Suggestion = suggestion };
                string json = JsonSerializer.Serialize(log, jsonSerializerOptions);
                string filePath = Path.Combine(WorkDirectory.logfileDirectory, fileName);
                File.WriteAllText(filePath, json);
            }
        }

        public async static Task<bool> UnzipAsync(string filePath, string targetPath, string title = "UnZip")
        {
            try
            {
                Directory.CreateDirectory(targetPath);
                ZipFile.ExtractToDirectory(filePath, targetPath);
                return true;
            }
            catch (Exception e)
            {
                await LogManager.LogWriteAsync(title, e.ToString());
                return false;
            }
        }
    }

}
