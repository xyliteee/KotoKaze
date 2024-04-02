using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using KotoKaze.Static;
using System.Windows.Threading;
using System.Diagnostics;
using KotoKaze.Windows;
using System;

namespace KotoKaze.Dynamic
{
    public class BackgroundTask
    {
        public string Title = string.Empty;
        public Process taskProcess = new();
        private string _description = string.Empty;
        public bool isCancle = false;
        public bool isError = false;

        public event Action OnChanged;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnChanged.Invoke();
                }
            }
        }
        public BackgroundTask()
        {
            OnChanged += RefreshTaskList;
        }
        public void SetFinal(Action? action=null) 
        {
            GlobalData.TasksList.Remove(this);
            RefreshTaskList();
            action?.Invoke();
        }
        private Thread DefaultOutputProcess() 
        {
            Thread thread = new(() =>
            {
                try
                {
                    string? res;
                    while ((res = taskProcess.StandardOutput.ReadLine()) != null)
                    {
                        Description = res;
                    }
                }
                catch (InvalidOperationException) { }
            });
            return thread;
        }
        private Thread DefaultErrorProcess() 
        {
            Thread thread = new(() =>
            {
                try
                {
                    string? errorMessage;
                    if ((errorMessage = taskProcess.StandardError.ReadLine()) != null)
                    {
                        isError = true;
                        KotoMessageBoxSingle.ShowDialog($"{Title} 发生错误：\n{errorMessage}");
                        SetFinal();
                    }
                }
                catch (InvalidOperationException) { }
            });
            return thread;
        }
        public void StreamProcess(Thread? outputThread = null, Thread? errorThread = null) 
        {

            outputThread ??= DefaultOutputProcess();
            errorThread ??= DefaultErrorProcess();
            outputThread.Start();
            errorThread.Start();
            try 
            {
                taskProcess.WaitForExit();
                if (!isError && !isCancle)
                {
                    SetFinal();
                    KotoMessageBoxSingle.ShowDialog($"{Title} 执行完成");
                }
            } 
            catch(InvalidOperationException) 
            {}
            
        }
        private static void ButtonCLick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            BackgroundTask backgroundTask = (BackgroundTask)button.Tag;
            ShutdownTask(backgroundTask);
        }
        private static void ShutdownTask(BackgroundTask backgroundTask)
        {
            backgroundTask.isCancle = true;
            backgroundTask.Description = "正在取消......";
            backgroundTask.taskProcess.Close();
            GlobalData.TasksList.Remove(backgroundTask);
            RefreshTaskList();
            KotoMessageBoxSingle.ShowDialog($"{backgroundTask.Title}被用户取消");
        }
        private static void CreatSingleCard(BackgroundTask backgroundTask, int index)
        {
            Canvas myCanvas = new()
            {
                Width = 390,
                Height = 40
            };
            Canvas.SetTop(myCanvas, index * 40);
            Label title = new()
            {
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Width = 390,
                Height = 30,
                Content = backgroundTask.Title,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };

            myCanvas.Children.Add(title);
            Label description = new()
            {
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Width = 320,
                Height = 20,
                Content = backgroundTask.Description,
                Padding = new Thickness(10, 0, 10, 0),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Foreground = Brushes.Gray
            };
            Canvas.SetTop(description, 20);
            myCanvas.Children.Add(description);

            Button button = new()
            {
                Style = (Style)Application.Current.FindResource("CancleButtonStyle"),
                Width = 30,
                Height = 30,
                Content = "×",
                Tag = backgroundTask,
            };
            button.Click += ButtonCLick;
            Canvas.SetRight(button, 20);
            Canvas.SetTop(button, 7);
            myCanvas.Children.Add(button);

            GlobalData.MainWindowInstance.ScorllCanvas.Children.Add(myCanvas);
        }
        private static void RefreshTaskList()
        {
            GlobalData.MainWindowInstance.Dispatcher.Invoke(() => 
            {
                GlobalData.MainWindowInstance.ScorllCanvas.Children.Clear();
                if (GlobalData.TasksList.Count == 0)
                {
                    GlobalData.MainWindowInstance.TaskListMessage.Content = "无任务";
                    Label label = new()
                    {
                        Content = "没有后台任务正在进行",
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Width = 390,
                        Height = 30
                    };
                    GlobalData.MainWindowInstance.ScorllCanvas.Children.Add(label);
                    Canvas.SetTop(label, 50);
                }
                else
                {
                    GlobalData.MainWindowInstance.TaskListMessage.Content = $"{GlobalData.TasksList.Count}个任务正在执行";
                    for (int index = 0; index < GlobalData.TasksList.Count; index++)
                    {
                        CreatSingleCard(GlobalData.TasksList[index], index);
                    }
                    GlobalData.MainWindowInstance.ScorllCanvas.Height = GlobalData.TasksList.Count * 40;
                }
            },DispatcherPriority.Background);
        }
    }

}
