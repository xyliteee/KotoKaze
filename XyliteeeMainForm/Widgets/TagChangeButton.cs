using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KotoKaze.Widgets
{
    public class TagChangeButton : Button
    {
        private object _tag;

        public new object Tag
        {
            get { return _tag; }
            set
            {
                if (_tag != value)
                {
                    _tag = value;
                    OnTagChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler TagChanged;

        protected virtual void OnTagChanged(EventArgs e)
        {
            TagChanged?.Invoke(this, e);
        }
    }
}
