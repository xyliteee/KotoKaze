using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using KotoKaze.Static;
using System.Windows.Threading;
using System.Diagnostics;
using KotoKaze.Windows;

namespace KotoKaze.Dynamic
{
    public class BackgroundTask
    {
        public string Title = string.Empty;
        public Process taskProcess = new();
        private string _description = string.Empty;

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

        public void SetFinished() 
        {
            GlobalData.TasksList.Remove(this);
            RefreshTaskList();
        }

        public void SetError(string message) 
        {
            GlobalData.TasksList.Remove(this);
            RefreshTaskList();
            GlobalData.MainWindowInstance.Dispatcher.Invoke(() => 
            {
                KotoMessageBoxSingle.ShowDialog($"{Title}执行失败：{message}");
            });
        }

        private static void RefreshTaskList()
        {
            static void ButtonCLick(object sender, RoutedEventArgs e) 
            {
                Button button = (Button)sender;
                BackgroundTask backgroundTask = (BackgroundTask)button.Tag;
                ShutdownTask(backgroundTask);
            }
            static void ShutdownTask(BackgroundTask backgroundTask) 
            {
                backgroundTask.Description = "正在取消......";
                backgroundTask.taskProcess.Close();
            }

            static void CreatSingleCard(BackgroundTask backgroundTask, int index)
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
            });
            
        }
    }

}
