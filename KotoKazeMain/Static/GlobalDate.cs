using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XyliteeeMainForm;
using XyliteeeMainForm.Views;

namespace KotoKaze.Static
{
    public static class GlobalData
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public static MainWindow MainWindowInstance { get; set; }
        public static toolsPage ToolsPageInstance { get; set; }
    }
}
