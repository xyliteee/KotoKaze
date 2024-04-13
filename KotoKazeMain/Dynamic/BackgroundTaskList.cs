using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnChange.Invoke();
        }
    }

}
