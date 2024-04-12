namespace CleanContent
{
    public class CleanRules
    {
        public readonly static string APPDATE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "");
        public readonly static string SystemDisk = APPDATE[0].ToString();
        public readonly static List<string> tempFilesRules;
        public readonly static List<string> updateFilesRules;
        public readonly static List<string> otherFilesRules;
        static CleanRules() 
        {
            tempFilesRules = SetTempFilesRules();
            updateFilesRules = SetUpdateFilesRules();
            otherFilesRules = SetOtherFilesRules();
        }

        private static string[] GetDirectoriesContains(string rootPath,string targetContains)
        {
            string[] directories = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);
            string[] result = directories.Where(path => Path.GetFileName(path).Contains(targetContains)).ToArray();
            return result;
        }

        private static List<string> SetTempFilesRules() 
        {
            List<string> result =
                [
                Path.Combine(APPDATE, "Local\\Temp"),
                Path.Combine(APPDATE, "LocalLow\\Temp"),
                Path.Combine(APPDATE, "Roaming\\Temp"),
                Path.Combine(APPDATE, "Local\\Downloaded Installations"),
                Path.Combine(APPDATE, "Local\\Microsoft\\Windows\\Explorer"),
                ];
            return result;
                
        }

        private static List<string> SetUpdateFilesRules() 
        {
            List<string> result =
                [
                "C:\\Windows\\SoftwareDistribution\\download",
                ];
            return result;
        }

        private static List<string> SetOtherFilesRules() 
        {
            List<string> result = 
                [
                Path.Combine(APPDATE, "Local\\NVIDIA\\GLCache"),
                Path.Combine(APPDATE, "Local\\NVIDIA\\DXCache"),
                Path.Combine(APPDATE, "LocalLow\\NVIDIA\\PerDriverVersion\\DXCache"),
                Path.Combine(APPDATE, "Roaming\\NVIDIA\\ComputeCache"),
                $"{SystemDisk}:\\ProgramData\\Microsoft\\Windows Defender\\Definition Updates\\Backup",
                ];
            result.AddRange(GetDirectoriesContains($"{SystemDisk}:\\Windows\\assembly", "NativeImages"));
            return result;
        }

    }
}
