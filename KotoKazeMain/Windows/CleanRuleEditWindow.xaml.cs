using KotoKaze.Static;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using CleanContent;
using XyliteeeMainForm.Views;
using Newtonsoft.Json;
using System.IO;
using System;


namespace KotoKaze.Windows
{
    /// <summary>
    /// CleanRuleEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CleanRuleEditWindow : Window
    {
        private readonly List<CleanStruct> cleanStructs = [];
        private bool isChanged = false;
        public CleanRuleEditWindow(List<CleanStruct> cleanStructs)
        {
            InitializeComponent();
            this.cleanStructs = cleanStructs;
            AnalyzeRules();
        }
        public static void ShowSettingPage()
        {
            List<CleanStruct>? cleanStructs = cleanPage.GetCleanStructs();
            if (cleanStructs == null) { return; }
            CleanRuleEditWindow cleanRuleEditWindow = new(cleanStructs)
            {
                Owner = GlobalData.MainWindowInstance
            };
            KotoMessageBox.RunShow(cleanRuleEditWindow);
            cleanRuleEditWindow.ShowDialog();
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(cleanStructs, Formatting.Indented);
            if (!isChanged) 
            {
                Close();
                return;
            }
            var rr = KotoMessageBox.ShowDialog("是否保存修改？");
            if (rr.IsYes)
            {
                File.WriteAllText(Path.Combine(FileControl.FileManager.WorkDirectory.BinDirectory, "clean.json"), json);
            }
            Close();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            KotoMessageBox.RunClose(this);
            base.OnClosing(e);
        }

        private void TypeEditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int index = (int)button.Tag;
            isChanged = true;
            if (button.Content.Equals("类型：Path"))
            {
                button.Content = "类型：Target";
                cleanStructs[index].Type = "Target";
            }
            else if (button.Content.Equals("类型：Target"))
            {
                button.Content = "类型：Type";
                cleanStructs[index].Type = "Type";
            }
            else if (button.Content.Equals("类型：Type"))
            {
                button.Content = "类型：Contain";
                cleanStructs[index].Type = "Contain";
            }
            else if (button.Content.Equals("类型：Contain"))
            {
                button.Content = "类型：Path";
                cleanStructs[index].Type = "Path";
            }
        }

        private void PathEditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int index = (int)button.Tag;
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult result = dialog.ShowDialog();
            if (result == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                string? path = dialog.FileName;
                if (path != null)
                {
                    button.Content = path;
                    cleanStructs[index].Path = path;
                    isChanged = true;
                }
            }
        }



        private void KeyEditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int index = (int)button.Tag;
            var rr = KotoMessageBoxInput.ShowDialog("请输入Key值");
            if (rr.IsClose) return;
            if (rr.IsYes)
            {
                button.Content = rr.Input;
                cleanStructs[index].Key = rr.Input;
                isChanged = true;
            }
        }

        private void AnalyzeRules()
        {
            scrollCanvas.Children.Clear();
            for (int index = 0; index < cleanStructs.Count; index++)
            {
                CreatSingleCard(index, cleanStructs[index]);
            }
            AddNewOne();
            scrollCanvas.Height = 50 * (cleanStructs.Count+1);
        }

        private void CreatSingleCard(int index, CleanStruct cs)
        {
            Canvas canvas = new()
            {
                Width = 530,
                Height = 40
            };
            Canvas.SetLeft(canvas, 10);
            Canvas.SetTop(canvas, 50 * index);
            Border border = new()
            {
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush(Color.FromRgb(243, 248, 254)),
                Height = 40,
                Width = 530,
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(170, 170, 170),
                    Direction = 270,
                    ShadowDepth = 0,
                    Opacity = 0.2,
                    BlurRadius = 5
                }
            };
            canvas.Children.Add(border);

            Button typeEditButton = new()
            {
                Height = 30,
                Width = 100,
                Content = $"类型：{cs.Type}",
                Style = (Style)FindResource("NomalButtonStyle"),
                Tag = index

            };
            Canvas.SetTop(typeEditButton, 5);
            Canvas.SetLeft(typeEditButton, 5);
            typeEditButton.Click += TypeEditButton_Click;
            canvas.Children.Add(typeEditButton);

            Button pathEditButton = new()
            {
                Height = 30,
                Width = 300,
                Content = cs.Path,
                Style = (Style)FindResource("NomalButtonStyle"),
                Tag = index
            };
            Canvas.SetTop(pathEditButton, 5);
            Canvas.SetLeft(pathEditButton, 110);
            pathEditButton.Click += PathEditButton_Click;
            canvas.Children.Add(pathEditButton);

            Button keyEditButton = new()
            {
                Height = 30,
                Width = 110,
                Content = cs.Key,
                Style = (Style)FindResource("NomalButtonStyle"),
                Tag = index
            };
            Canvas.SetTop(keyEditButton, 5);
            Canvas.SetLeft(keyEditButton, 415);
            keyEditButton.Click += KeyEditButton_Click;
            canvas.Children.Add(keyEditButton);
            scrollCanvas.Children.Add(canvas);
        }

        private void AddNewOne()
        {
            void AddNew(object sender, RoutedEventArgs e) 
            {
                cleanStructs.Add(new("Path","Null","Null"));
                AnalyzeRules();
                isChanged = true;
            }
            Canvas canvas = new()
            {
                Width = 530,
                Height = 40
            };
            Canvas.SetLeft(canvas, 10);
            Canvas.SetTop(canvas, 50 * cleanStructs.Count);
            Border border = new()
            {
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush(Color.FromRgb(243, 248, 254)),
                Height = 40,
                Width = 530,
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(170, 170, 170),
                    Direction = 270,
                    ShadowDepth = 0,
                    Opacity = 0.2,
                    BlurRadius = 5
                }
            };
            canvas.Children.Add(border);

            Button addNewButton = new()
            {
                Height = 30,
                Width = 400,
                Content = "新增加一个清理条目",
                Style = (Style)FindResource("NomalButtonStyle"),
            };
            addNewButton.Click += AddNew;
            canvas.Children.Add(addNewButton);
            Canvas.SetTop(addNewButton, 5);
            Canvas.SetLeft(addNewButton, 65);
            scrollCanvas.Children.Add(canvas);
        }
    }
}
