
using KotoKaze.Static;
using System.Diagnostics.Eventing.Reader;

namespace KotoKaze.Dynamic
{
    public class BackgroundTaskList<T> : List<T>
    {
        public delegate void ChangeHandler(object sender, EventArgs e);
        public Action OnChange;
        public BackgroundTaskList() => OnChange = new Action(BackgroundTask.RefreshTaskList);
        public new void Add(T item)
        {
            base.Add(item);
            OnChange.Invoke();
        }

        public new bool Remove(T item)
        {
            var result = base.Remove(item);
            if (result)
            {
                OnChange.Invoke();
            }
            return result;
        }
        
    }
    public static class BackgroundTaskList
    {
        public static bool IsTaskRunning(BackgroundTask backgroundTask)
        {
            if (GlobalData.TasksList.Contains(backgroundTask)) return true;
            return false;
        }
    }
}
