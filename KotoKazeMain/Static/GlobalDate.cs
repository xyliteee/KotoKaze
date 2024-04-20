using KotoKaze.Dynamic;
using static FileControl.FileManager.IniManager;
using System.Windows;
using TestContent;
using XyliteeeMainForm;
using XyliteeeMainForm.Views;
using FileControl;

namespace KotoKaze.Static
{
    public static class GlobalData
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public static MainWindow MainWindowInstance { get; set; }
        public static toolsPage ToolsPageInstance { get; set; }
        public static homePage HomePageInstance { get; set; }
        public static cleanPage CleanPageInstance { get; set; }
        public static PCTestPage PCTestPageInstance { get; set; }
        public static settingPage SettingPageInstance { get; set; }
        public static BackgroundTaskList<BackgroundTask> TasksList { get; set; } = [];
        public static List<FrameworkElement> MessageBoxList { get; set; } = [];
        public static bool IsRunning { get; set; } = true;
        public static double RefreshTime = double.Parse(IniFileRead("Application.ini", "SETTING", "REFRESH_TIME"));//单位为秒
        public static int AnimationLevel = int.Parse(IniFileRead("Application.ini", "SETTING", "ANIMATION_LEVEL"));

    }
}
