using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

namespace KotoKaze.Static
{
    public static class ProcessControl
    {
        public async static Task<(bool,string)> ProcessShutdownAsync(string name) 
        {
            try
            {
                foreach (Process process in Process.GetProcessesByName(name))
                {
                    await Task.Run(process.Kill);
                }
                return (true,string.Empty);
            }
            catch (Exception e) 
            {
                return (false,e.ToString());
            }
        }

        public static (bool,string)  ProcessShutdown(string name) 
        {
            try
            {
                foreach (Process process in Process.GetProcessesByName(name))
                {
                    process.Kill();
                }
                return (true, string.Empty);
            }
            catch (Exception e) 
            {
                return (false, e.ToString());
            }
            
        }

        public static void UpdateUI(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            try
            {
               Application.Current.Dispatcher.Invoke(action, priority);
            }
            catch (Exception){}
        }
    }
}
