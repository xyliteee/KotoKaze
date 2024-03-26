using System.IO;
using System.Text;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace KotoKaze.Static
{
    public class FileManager
    {
        public static class WorkDirectory
        {
            public static readonly string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            public static readonly string backupDirectory = Path.Combine(rootDirectory, "Backup");
            public static readonly string localDataDirectory = Path.Combine(rootDirectory, "LocalData");
            public static readonly string softwareTempDirectory = Path.Combine(CleanRules.APPDATE, "Local\\Temp\\KotoKaze");

            public static void CreatWorkDirectory()
            {
                Directory.CreateDirectory(backupDirectory);
                Directory.CreateDirectory(localDataDirectory);
                Directory.CreateDirectory(softwareTempDirectory);
            }

            public static void CreatWorkFile() 
            {
                if (!Path.Exists(Path.Combine(localDataDirectory, "Performance.ini"))) 
                {
                    IniManager.IniFileSetDefault("Performance.ini");
                }
                if (!Path.Exists(Path.Combine(localDataDirectory, "Application.ini")))
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
                string filePath = Path.Combine(WorkDirectory.localDataDirectory, fileName);
                if (File.Exists(filePath))
                {
                    using StreamReader sr = new(filePath, Encoding.Default);
                    string currentSection = "";
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith("[") && line.EndsWith("]"))
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
                if (!data.ContainsKey(section))
                {
                    data[section] = [];
                }
                data[section][key] = value;

                using StreamWriter sw = new (filePath, false, Encoding.Default);
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
                string filePath = Path.Combine(WorkDirectory.localDataDirectory, fileName);
                section = section.ToUpper();
                key = key.ToUpper();
                Dictionary<string, Dictionary<string, string>> data = [];
                // 如果文件存在，读取数据
                if (File.Exists(filePath))
                {
                    using StreamReader sr = new(filePath, Encoding.Default);
                    string currentSection = "";
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith("[") && line.EndsWith("]"))
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

                // 如果找到了section和key
                if (data.ContainsKey(section) && data[section].ContainsKey(key))
                {
                    string result = data[section][key];
                    return Encryption.Decrypt(result);
                }
                // 如果没有找到，返回空
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
    }
}
