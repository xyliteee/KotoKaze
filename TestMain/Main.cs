using System.Runtime.InteropServices;
using FileControl;

namespace TestApplication 
{
    public static partial class RAM
    {
        [LibraryImport("TestModule.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int RamWriteSpeed();

        [LibraryImport("TestModule.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial int RamReadSpeed();
    }

    public class MainProgress 
    {
        public static void Main(string[]args)
        {
            if (args.Length == 0) 
            {
                return;
            }
            string arg = args[0];
            switch (arg) 
            {
                case "RAM_WRITE":
                    int RamWriteSpeed = RAM.RamWriteSpeed();
                    int RamWriteScore = (int)(RamWriteSpeed * 0.5 / 8.4);
                    FileManager.IniManager.IniFileWrite("Performance.ini", "VALUE", "RAM_WRITE_SCORE", RamWriteScore.ToString());
                    break;
                case "RAM_READ":
                    int RamReadSpeed = RAM.RamReadSpeed();
                    int RamReadScore = (int)(RamReadSpeed * 0.5 / 8.4);
                    FileManager.IniManager.IniFileWrite("Performance.ini", "VALUE", "RAM_READ_SCORE", RamReadScore.ToString());
                    break;
            }
            return;
        }
    }
}
