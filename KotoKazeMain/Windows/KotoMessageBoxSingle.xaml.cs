﻿using KotoKaze.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KotoKaze.Windows
{
    /// <summary>
    /// KotoMessageBoxSingle.xaml 的交互逻辑
    /// </summary>
    public partial class KotoMessageBoxSingle : Window
    {
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
        public KotoMessageBoxSingle()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            InitializeComponent();
        }
        public static void Show(string context, EventHandler<MessageBoxEventArgs> result)
        {
            KotoMessageBoxSingle mb = new()
            {
                Context = context
            };
            mb.Result += result;
            mb.Show();
        }
        public static MessageResult ShowDialog(string context)
        {
            MessageResult r = new();
            GlobalData.MainWindowInstance.Dispatcher.Invoke(() => 
            {
                KotoMessageBoxSingle kotoMessageBox = new()
                {
                    Context = context,
                    Owner = GlobalData.MainWindowInstance
                };
                KotoMessageBox.RunShow(kotoMessageBox);

                kotoMessageBox.Result += (s, e) => {
                    r = e.Result;
                };
                kotoMessageBox.ShowDialog();
            });
            return r;
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
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            GlobalData.messageBoxList.Remove(this);
            if (GlobalData.messageBoxList.Count == 0)
            {
                Animations.ChangeOP(GlobalData.MainWindowInstance.messageMask, null, 0, 0.05);
            }
            base.OnClosing(e);
        }
    }
}
