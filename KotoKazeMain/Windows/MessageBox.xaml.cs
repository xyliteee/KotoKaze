using KotoKaze.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KotoKaze.Windows
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class KotoMessageBox : Window
    {
        private static double AnimationTimeline = 0.1;
        public class MessageResult
        {
            public bool IsYes { get; set; } = true;
            public bool IsClose { get; set; } = false;
        }
        public class MessageBoxEventArgs : EventArgs
        {
            public MessageResult Result { get; set; } = new();
        }
        public event EventHandler<MessageBoxEventArgs> Result;

        public string Context
        {
            get { return TB_Context.Text; }
            set { TB_Context.Text = value; }
        }
        bool _isLegal = false;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public KotoMessageBox()
        {
            InitializeComponent();
        }
        public static void Show(string context, EventHandler<MessageBoxEventArgs> result)
        {
            KotoMessageBox mb = new()
            {
                Context = context
            };
            mb.Result += result;
            mb.Show();
        }

        
        public static MessageResult ShowDialog(string context)
        {
            MessageResult r = new();
            ProcessControl.UpdateUI(() =>
            {
                KotoMessageBox kotoMessageBox = new()
                {
                    Context = context,
                    Owner = GlobalData.MainWindowInstance
                };
                RunShow(kotoMessageBox);
                kotoMessageBox.Result += (s, e) =>
                {
                    r = e.Result;
                };
                kotoMessageBox.ShowDialog();
            });
            return r;
        }

        private void No_Button_Click(object sender, RoutedEventArgs e)
        {
            _isLegal = true;
            Close();
            Result?.Invoke(this, new MessageBoxEventArgs() { Result = new MessageResult() { IsYes = false } });
        }
        private void Yes_Button_Click(object sender, RoutedEventArgs e)
        {
            _isLegal = true;
            Close();
            Result?.Invoke(this, new MessageBoxEventArgs() { Result = new MessageResult() { IsYes = true } });
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_isLegal;
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            _isLegal = true;
            Close();
            Result?.Invoke(this, new MessageBoxEventArgs()
            {
                Result = new MessageResult()
                {
                    IsClose = true,
                }
            });
        }
        public static void RunClose(FrameworkElement frameworkElement) 
        {
            GlobalData.MessageBoxList.Remove(frameworkElement);
            if (GlobalData.MessageBoxList.Count == 0)
            {
                Animations.ChangeOP(GlobalData.MainWindowInstance.messageMask, null, 0, 0);
                Animations.ChangeBlur(GlobalData.MainWindowInstance.mainBase, 0, null, 0);
            }
        }
        public static void RunShow(FrameworkElement kotoMessageBox)
        {
            FrameworkElement window = (FrameworkElement)kotoMessageBox.FindName("window")!;
            GlobalData.MessageBoxList.Add(window);
            Animations.ChangeOP(GlobalData.MainWindowInstance.messageMask, null, 0.3, AnimationTimeline);
            Animations.ChangeSize(window, AnimationTimeline, 0.95, 1);
            Animations.ChangeBlur(GlobalData.MainWindowInstance.mainBase, AnimationTimeline, null, 5);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            RunClose(this);
            base.OnClosing(e);
        }
    }
}
