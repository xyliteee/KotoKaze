
using KotoKaze.Static;
using System.Diagnostics.Eventing.Reader;

namespace KotoKaze.Dynamic
{
    public class BackgroundTaskList<T> : List<T>
    {
        public delegate void ChangeHandler(object sender, EventArgs e);
        public new void Add(T item)
        {
            base.Add(item);
            BackgroundTask.RefreshTaskList();
        }

        public new bool Remove(T item)
        {
            var result = base.Remove(item);
            if (result)
            {
                BackgroundTask.RefreshTaskList();
            }
            return result;
        }
        
    }
    public static class BackgroundTaskList
    {
        public static bool IsTaskRunning(BackgroundTask? backgroundTask)
        {
            if (backgroundTask == null)  return false; 
            if (GlobalData.TasksList.Contains(backgroundTask)) return true;
            return false;
        }
    }
}
