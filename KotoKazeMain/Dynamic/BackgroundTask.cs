using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using KotoKaze.Static;
using System.Windows.Threading;
using System.Diagnostics;
using KotoKaze.Windows;
using FileControl;
using System.IO;
using HelixToolkit.Logger;

namespace KotoKaze.Dynamic
{
    public class BackgroundTask 
    {
        public string Title = string.Empty;
        private string _description = string.Empty;
        private Label DescriptionLable = new();
        public bool isCancle = false;
        public bool isError = false;
        public bool isFinished = false;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                ProcessControl.UpdateUI(() =>
                {
                    DescriptionLable.Content = value;
                });
            }
        }
        public void SetFinished(Action? action = null)
        {
            GlobalData.TasksList.Remove(this);
            action?.Invoke();
        }
        virtual public void Start() 
        {
            ProcessControl.UpdateUI(() =>
            {
                Description = "正在启动......";
            });
            GlobalData.TasksList.Add(this);
        }
        private static void ButtonCLick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            BackgroundTask backgroundTask = (BackgroundTask)button.Tag;
            backgroundTask.Shutdown();
        }
        virtual public void Shutdown(bool showMessage = true)
        {
            isCancle = true;
            Description = "正在取消......";
            GlobalData.TasksList.Remove(this);
            if (!showMessage) return;
            KotoMessageBoxSingle.ShowDialog($"{Title}被用户取消");
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
            backgroundTask.DescriptionLable = description;
            Canvas.SetTop(description, 20);
            myCanvas.Children.Add(description);

            Button button = new()
            {
                Style = (Style)Application.Current.FindResource("ButtonDanger"),
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
        public static void RefreshTaskList()
        {
            ProcessControl.UpdateUI(() =>
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
            }, DispatcherPriority.Background);
        }
    }
    public class CMDBackgroundTask:BackgroundTask
    {
        public Action outputThreadAction;
        private Thread outputThread;
        public Action errorThreadAction;
        private Thread errorThread;
        public  static readonly ProcessStartInfo startInfoWithWindow = new()
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            CreateNoWindow = false,
            UseShellExecute = false
        };
        private static readonly ProcessStartInfo startInfo = new()
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            UseShellExecute = false
        };
        public readonly Process taskProcess = new() { StartInfo = startInfo };

        public CMDBackgroundTask()
        {
            outputThreadAction = DefaultOutputProcess();
            errorThreadAction = DefaultErrorProcess();
            outputThread = new(() => { });
            errorThread = new(() =>{ });
        }
        override public void Start()
        {
            base.Start();
            taskProcess.Start();
        }
        private Action DefaultOutputProcess()
        {
            Action action = new(() => 
            {
                try
                {
                    Task<string?> readLineTask = taskProcess.StandardOutput.ReadLineAsync();
                    while (GlobalData.IsRunning)
                    {
                        if (isCancle || isError || isFinished) break;
                        if (!readLineTask.IsCompleted)
                        {
                            Thread.Sleep(16);
                            continue;
                        }
                        if (string.IsNullOrEmpty(readLineTask.Result))
                        {
                            readLineTask = taskProcess.StandardOutput.ReadLineAsync();
                            continue;
                        }
                        Description = readLineTask.Result;
                        readLineTask = taskProcess.StandardOutput.ReadLineAsync();
                    }
                }
                catch (InvalidOperationException) { }
            });
            return action;
        }
        private Action DefaultErrorProcess()
        {
            Action action = new(() => 
            {
                try
                {
                    Task<string?> readLineTask = taskProcess.StandardError.ReadLineAsync();
                    while (!(isCancle || isError || isFinished) && GlobalData.IsRunning)
                    {
                        if (readLineTask.IsCompleted)
                        {
                            string? errorMessage = readLineTask.Result;
                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                isError = true;
                                Description = "error";
                                FileManager.LogManager.LogWrite(Title + " Error", errorMessage);
                                SetFinished(() => { KotoMessageBoxSingle.ShowDialog($"{Title} 发生错误,已保存日志"); });
                                break;
                            }
                            readLineTask = taskProcess.StandardError.ReadLineAsync();
                        }
                        else
                        {
                            Thread.Sleep(16);
                        }
                    }
                }
                catch (InvalidOperationException) { }
            });
            return action;
        }
        public void CommandWrite(string[] commands ) 
        {
            using StreamWriter streamWriter = taskProcess.StandardInput;
            if (streamWriter.BaseStream.CanWrite)
            {
                foreach (string commad in commands)
                {
                    streamWriter.WriteLine(commad);
                }
            }
        }
        public void StreamProcess() 
        {
            outputThread = new(() => { outputThreadAction(); });
            errorThread = new(() => { errorThreadAction(); });
            outputThread.Start();
            errorThread.Start();
            try 
            {
                Task.Run(() => 
                {
                    taskProcess.WaitForExit();//等待进程完成
                    isFinished = true;//进程完成
                    errorThread.Join(1000);//等待错误流线程完成
                    outputThread.Join(1000);//等待输出流线程完成
                    if (!(isError || isCancle))//如果没有错误也不是打断
                    {
                        SetFinished(() => { KotoMessageBoxSingle.ShowDialog($"{Title} 执行完成"); });
                    }
                });
            } 
            catch(InvalidOperationException){}
        }
        private static void ButtonCLick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CMDBackgroundTask backgroundTask = (CMDBackgroundTask)button.Tag;
            backgroundTask.Shutdown();
        }
        override public void Shutdown(bool showMessage = true)
        {
            void SetShutdown() 
            {
                isCancle = true;//设置取消标识
                outputThread.Join(1000);//等待输出线程,最多等待1000秒
                errorThread.Join(1000);//等待错误线程
                SetFinished(() =>
                {
                    if (showMessage) KotoMessageBoxSingle.ShowDialog($"{Title}被用户取消");
                });
            }
            Description = "正在取消......";
            taskProcess.Kill();//手动关闭进程
            Task.Run(() => 
            {
                try
                {
                    SetShutdown();
                }

                catch (Exception e)
                {
                    KotoMessageBoxSingle.ShowDialog($"{Title}取消时发生了未知的错误，已生成日志文件");
                    Task.Run(async () =>
                    {
                        await FileManager.LogManager.LogWriteAsync($"{Title} Interupt Error", e.ToString());
                    });
                }
            });
        }
    }
    public class NetworkBackgroundTask : BackgroundTask 
    {
        public Network.Downloader downloader;
        public NetworkBackgroundTask(Network.Downloader downloader) 
        {
            this.downloader = downloader;
            this.downloader.action = new Action(() => 
            {
                double percentage = downloader.FileDateHaveAlreadyDownloaded * 100 / downloader.fileSize;
                Description = $"已下载：{percentage}% [{new string('*', (int)(percentage*0.35))}]";
            });
        }
        private static void ButtonCLick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            NetworkBackgroundTask backgroundTask = (NetworkBackgroundTask)button.Tag;
            backgroundTask.Shutdown();
        }
        override public void Shutdown(bool showMessage = true)
        {
            downloader.isDownloading = false;
            base.Shutdown();
        }
    }
}
