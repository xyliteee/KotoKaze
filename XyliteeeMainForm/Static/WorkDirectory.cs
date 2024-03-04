using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KotoKaze.Static
{
    public static class WorkDirectory
    {
        public static readonly string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string backupDirectory = Path.Combine(rootDirectory, "Backup");

        public static void CreatWorkDirectory() 
        {
            Directory.CreateDirectory(backupDirectory);
        }
    }
}
