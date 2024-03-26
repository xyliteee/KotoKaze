using System.IO;


namespace KotoKaze.Static
{
    internal class CleanRules
    {
        public readonly static string APPDATE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "");

        public readonly static List<string> tempFilesRules = 
            [
                Path.Combine(APPDATE, "Local\\Temp"),
                Path.Combine(APPDATE, "LocalLow\\Temp"),
                Path.Combine(APPDATE, "Roaming\\Temp"),
                Path.Combine(APPDATE, "Local\\Downloaded Installations"),
                Path.Combine(APPDATE, "Local\\Microsoft\\Windows\\Explorer"),
            ];

        public readonly static List<string> updateFilesRules =
            [
                "C:\\Windows\\SoftwareDistribution\\download",
            ];

        public readonly static List<string> otherFilesRules =
            [
                Path.Combine(APPDATE, "Local\\NVIDIA\\GLCache"),
                Path.Combine(APPDATE, "Local\\NVIDIA\\DXCache"),
                Path.Combine(APPDATE, "LocalLow\\NVIDIA\\PerDriverVersion\\DXCache"),
                Path.Combine(APPDATE, "Roaming\\NVIDIA\\ComputeCache")
            ];
    }
}
