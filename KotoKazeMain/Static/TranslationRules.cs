using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotoKaze.Static
{
    public static class TranslationRules
    {
        public static string Translate(string input, Dictionary<string, string> translationRules) 
        {
            foreach (KeyValuePair<string, string> translation in translationRules)
            {
                input = input.Replace(translation.Key, translation.Value);
            }
            return input;
        }

        public readonly static Dictionary<string, string> batteryReport = new()
                {
                    { "Battery report", "电池报告" },
                    { "COMPUTER NAME", "电脑名称" },
                    { "SYSTEM PRODUCT NAME", "系统产品名称" },
                    { "OS BUILD", "系统构建版本" },
                    { "BIOS", "BIOS" },
                    { "PLATFORM ROLE", "设备类型" },
                    { "CONNECTED STANDBY", "连接待机状态" },
                    { "REPORT TIME", "报告时间" },
                    { "Installed batteries","已安装的电池"},
                    { "Information about each currently installed battery","关于当前每个已安装电池的信息"},
                    { "                  BATTERY","电池"},
                    { ">NAME<",">名称<"},
                    { "MANUFACTURER","制造商"},
                    { "SERIAL NUMBER","序列号"},
                    { "CHEMISTRY","化学类型"},
                    { "DESIGN CAPACITY","设计容量"},
                    { "FULL CHARGE CAPACITY","实际充满容量"},
                    { "CYCLE COUNT","循环次数"},
                    { "Recent usage","最近使用情况"},
                    { "Power states over the last 3 days","过去三日的电源状态"},
                    { "START TIME","开始时间"},
                    { "            STATE","状态"},
                    { "            SOURCE","来源"},
                    { "CAPACITY REMAINING","剩余容量"},
                    { "Battery usage","电池使用情况"},
                    { "Battery drains over the last 3 days","过去三日的电池使用情况"},
                    { "DURATION","使用时长"},
                    { "ENERGY DRAINED","消耗的电量"},
                    { "Usage history","使用历史"},
                    { "History of system usage on AC and battery","系统使用电源或电池的历史情况"},
                    { "BATTERY DURATION","电池使用时长"},
                    { "AC DURATION","电源使用时长"},
                    { "PERIOD","日期"},
                    { "ACTIVE","活跃"},
                    { "Battery capacity history","电池容量历史"},
                    { "Charge capacity history of the system's batteries","系统实际充满容量历史情况"},
                };
    }
}
