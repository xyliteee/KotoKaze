using System.Windows.Controls;
using static FileControl.FileManager.IniManager;
using FileControl;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows;
using KotoKaze.Static;
using System.Windows.Threading;
using System.IO;
using KotoKaze.Windows;
using TestContent;

namespace XyliteeeMainForm.Views
{
    /// <summary>
    /// PCTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class PCTestPage : Page
    {
        private readonly WorkLoadTest.CPU CPUTest;
        private readonly WorkLoadTest.RAM RAMTest;
        private readonly WorkLoadTest.Disk DiskTest;
        private static bool isFirstView = true;
        private GPUTestWindow GPUWindow;
        private int CPUScore = 0;
        private int GPUScore = 0;
        private int RamScore = 0;
        private int DiskScore = 0;
#pragma warning disable CS8618
        public PCTestPage()
        {
            InitializeComponent();
            CPUTest = new(this);
            RAMTest = new(this);
            DiskTest = new(this);
            CheckScore();
        }

        private void CheckScore() 
        {
            string CPUSINGLESCORE = IniFileRead("Performance.ini", "VALUE", "CPU_SINGLE_SCORE");
            string CPUMULTISCORE = IniFileRead("Performance.ini", "VALUE", "CPU_MULTI_SCORE");
            string GPUFPS = IniFileRead("Performance.ini", "VALUE", "GPU_FPS");
            string RAMREADSCORE = IniFileRead("Performance.ini", "VALUE", "RAM_READ_SCORE");
            string RAMWRITESCORE = IniFileRead("Performance.ini", "VALUE", "RAM_WRITE_SCORE");
            string DISKREADSCORE = IniFileRead("Performance.ini", "VALUE", "DISK_READ_SCORE");
            string DISKWRITESCORE = IniFileRead("Performance.ini", "VALUE", "DISK_WRITE_SCORE");

            if (CPUSINGLESCORE != string.Empty && CPUMULTISCORE != string.Empty) 
            {
                if (double.TryParse(CPUSINGLESCORE, out double cpuSingleScore) && double.TryParse(CPUMULTISCORE, out double cpuMultiScore))
                {
                    CPUScore = (int)(cpuSingleScore * 0.4 + cpuMultiScore * 0.6);
                    CPUScoreLabel.Content = CPUScore;
                    CPUDialogScoreLabel.Content = $"{cpuSingleScore}S-{cpuMultiScore}M";
                }
                else 
                {
                    //文件被修改，不需要再往下读取了
                    IniFileSetDefault("Performance.ini");
                    return;
                }
            }

            if (GPUFPS != string.Empty)
            {
                if (double.TryParse(GPUFPS, out double gpuFPS))
                {
                    GPUScore = (int)(gpuFPS * 280.5);
                    GPUScoreLabel.Content = GPUScore;
                    GPUDialogScoreLabel.Content = $"平均帧率-{gpuFPS}";
                }
                else 
                {
                    IniFileSetDefault("Performance.ini");
                    return;
                }
            }

            if(RAMWRITESCORE != string.Empty && RAMREADSCORE != string.Empty) 
            {
                if (double.TryParse(RAMWRITESCORE, out double ramWriteScore) && double.TryParse(RAMREADSCORE, out double ramReadScore))
                {
                    RamScore = (int)(ramReadScore * 0.5 + ramWriteScore * 0.5);
                    RAMScoreLabel.Content = RamScore;
                    RAMDialogScoreLabel.Content = $"{ramWriteScore}W-{ramReadScore}R";
                }
                else 
                {
                    IniFileSetDefault("Performance.ini");
                    return;
                }
            }

            if (DISKREADSCORE != string.Empty && DISKWRITESCORE != string.Empty) 
            {
                if (double.TryParse(DISKREADSCORE, out double diskReadScore) && double.TryParse(DISKWRITESCORE, out double diskWriteScore))
                {
                    DiskScore = (int)(diskReadScore * 0.5 + diskWriteScore * 0.5);
                    DiskScoreLabel.Content = DiskScore;
                    DiskDialogScoreLabel.Content = $"{diskWriteScore}W-{diskReadScore}R";
                }
                else 
                {
                    IniFileSetDefault("Performance.ini");
                }
            }
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ColorAnimation animation = new()
            {
                To = (Color)ColorConverter.ConvertFromString("#A0C5E0"),
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, TestButtonBorder);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Background.Color"));
            storyboard.Begin();
        }
        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ColorAnimation animation = new()
            {
                To = (Color)ColorConverter.ConvertFromString("#F3F8FE"),
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, TestButtonBorder);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Background.Color"));
            storyboard.Begin();
        }

        private void RunCPUTest()
        {
            Dispatcher.Invoke(() => { CPUScoreLabel.Content = "正在测试"; });
            CPUTest.RunTest();                                                                                          //执行CPU测试
            Dispatcher.Invoke(() =>
            {
                CPUScore = (int)(CPUTest.CPUSingleCoreScore * 0.4 + CPUTest.CPUMultiCoreScore * 0.6);                   //获取测试结果，单核和多核权重为0.4与0.6
                CPUScoreLabel.Content = CPUScore;
                CPUDialogScoreLabel.Content = $"{CPUTest.CPUSingleCoreScore}S-{CPUTest.CPUMultiCoreScore}M";
            });
            IniFileWrite("Performance.ini", "VALUE", "CPU_SINGLE_SCORE", CPUTest.CPUSingleCoreScore.ToString());
            IniFileWrite("Performance.ini", "VALUE", "CPU_MULTI_SCORE", CPUTest.CPUMultiCoreScore.ToString());
            GC.Collect();
            Thread.Sleep(1000);
        }
        private void RunGPUTest()
        {
            Dispatcher.Invoke(() =>
            {
                GPUWindow = new();
                GPUScoreLabel.Content = "正在测试";
                GlobalData.MainWindowInstance.Hide();
                GPUWindow.Show();
                GPUWindow.Test();
            });
            Thread.Sleep(5000);
            double FPS = Math.Round(1000 / GPUWindow.frameTimes.Average(), 2);
            Dispatcher.Invoke(() =>
            {
                GPUWindow.Close();
                GlobalData.MainWindowInstance.Show();
                GPUScore = (int)(FPS * 280.5);
                GPUScoreLabel.Content = GPUScore;
                GPUDialogScoreLabel.Content = $"平均帧率-{FPS}";
            });
            IniFileWrite("Performance.ini", "VALUE", "GPU_FPS",FPS.ToString());
        }
        private async void RunRamTest()
        {
            if (!Path.Exists(Path.Combine(FileManager.WorkDirectory.rootDirectory,"TestModule.dll"))) 
            {
                KotoMessageBoxSingle.ShowDialog("测试模块TestModule.dll丢失!");
                await FileManager.LogManager.LogWriteAsync("RAM TEST", "TestMoudle.dll is missing");
                return;
            }
            Dispatcher.Invoke(() => { RAMScoreLabel.Content = "正在测试"; });
            RAMTest.RunTest();
            Dispatcher.Invoke(() =>
            {
                RamScore = (int)(RAMTest.readScore * 0.5 + RAMTest.writeScore * 0.5);
                RAMScoreLabel.Content = RamScore;
                RAMDialogScoreLabel.Content = $"{RAMTest.writeScore}W-{RAMTest.readScore}R";
            });
            IniFileWrite("Performance.ini", "VALUE", "RAM_WRITE_SCORE",RAMTest.writeScore.ToString());
            IniFileWrite("Performance.ini", "VALUE", "RAM_READ_SCORE", RAMTest.readScore.ToString());
            GC.Collect();
            Thread.Sleep(1000);
        }
        private async void RunDiskTest()
        {
            if (!Path.Exists(Path.Combine(FileManager.WorkDirectory.rootDirectory, "TestModule.dll")))
            {
                KotoMessageBoxSingle.ShowDialog("测试模块TestModule.dll丢失!");
                await FileManager.LogManager.LogWriteAsync("DISK TEST", "TestMoudle.dll is missing");
                return;
            }
            Dispatcher.Invoke(() => { DiskScoreLabel.Content = "正在测试"; });
            DiskTest.RunTest();
            Dispatcher.Invoke(() =>
            {
                DiskScore = (int)(DiskTest.readScore * 0.5 + DiskTest.writeScore * 0.5);
                DiskScoreLabel.Content = DiskScore;
                DiskDialogScoreLabel.Content = $"{DiskTest.writeScore}W-{DiskTest.readScore}R";
            });
            IniFileWrite("Performance.ini", "VALUE", "DISK_WRITE_SCORE", DiskTest.writeScore.ToString());
            IniFileWrite("Performance.ini", "VALUE", "DISK_READ_SCORE", DiskTest.readScore.ToString());
            GC.Collect();
        }

        private void GetScore() 
        {
            Dispatcher.Invoke(() =>                                                                                 //当所有测试完成时
            {
                ScoreLabel.Content = CPUScore + GPUScore + RamScore + DiskScore;
                Animations.ImageTurnRound(TestImage, false);                                                        //别转了
                SetAllButtonState(true);                                                                        //恢复按钮
            });
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Animations.ImageTurnRound(TestImage, true);                                                                 //按下按钮，图标开始转
            SetAllButtonState(false);                                                                               //此时禁用按钮
            ScoreLabel.Content = "正在测试";
            Task.Run(() => 
            {
                try
                {
                    RunCPUTest();
                    RunGPUTest();
                    RunRamTest();
                    RunDiskTest();
                    GetScore();
                }
                catch (ThreadAbortException) { }
                catch (TaskCanceledException) { }
            });
        }

        private void SetAllButtonState(bool flag) 
        {
            TestButton.IsEnabled = flag;
            CPUTestButton.IsEnabled = flag;
            GPUTestButton.IsEnabled = flag;
            RAMTestButton.IsEnabled = flag;
            DiskTestButton.IsEnabled = flag;
        }
        private void CPUTestButton_Click(object sender, RoutedEventArgs e)
        {
            Animations.ImageTurnRound(TestImage, true);
            SetAllButtonState(false);
            ScoreLabel.Content = "正在测试";
            Thread thread = new(() =>
            {
                try
                {
                    RunCPUTest();
                    GetScore();
                }
                catch (ThreadAbortException) { }
                catch (TaskCanceledException) { }
            })
            {
                Priority = ThreadPriority.Highest
            };
        }

        private void GPUTestButton_Click(object sender, RoutedEventArgs e)
        {
            Animations.ImageTurnRound(TestImage, true);
            SetAllButtonState(false);
            ScoreLabel.Content = "正在测试";
            Task.Run(() =>
            {
                try
                {
                    RunGPUTest();
                    GetScore();
                }
                catch (ThreadAbortException) { }
                catch (TaskCanceledException) { }
            });
        }

        private void RAMTestButton_Click(object sender, RoutedEventArgs e)
        {
            Animations.ImageTurnRound(TestImage, true);
            SetAllButtonState(false);
            ScoreLabel.Content = "正在测试";

            Thread thread = new(() =>
            {
                try
                {
                    RunRamTest();
                    GetScore();
                }
                catch (ThreadAbortException) { }
                catch (TaskCanceledException) { }
            })
            {
                Priority = ThreadPriority.Highest
            };
            thread.Start();
        }


        private void DiskTestButton_Click(object sender, RoutedEventArgs e)
        {
            Animations.ImageTurnRound(TestImage, true);
            SetAllButtonState(false);
            ScoreLabel.Content = "正在测试";
            Task.Run(() =>
            {
                try
                {
                    RunDiskTest();
                    GetScore();
                }
                catch (ThreadAbortException) { }
                catch (TaskCanceledException) { }
            });
        }
    }

    public class WorkLoadTest
    {
        public class CPU
        {
            private readonly PCTestPage ParentClass;                                                                    //声明父类，用于更新UI
            private readonly Process process = Process.GetCurrentProcess();                                             //获取当前进程（是进程）
            readonly nint originalAffinity;                                                                             //声明原始的CPU亲和性
            private readonly int coreCount = Environment.ProcessorCount;                                                //获取CPU核心数（应该放在基础页面的，不过无所谓了）
            public int CPUSingleCoreScore = new();                                                                      //声明初始化分数(int是这么用的？)
            public int CPUMultiCoreScore = new();
            private readonly int[] aesCounts = [0,0];                                                                   //各项测试的计数
            private readonly int[] navigationCounts = [0,0];
            private readonly int[] textCompressCounts = [0,0];
            private readonly int[] textDeCompressCounts = [0, 0];
            private readonly int[] imageCompressCounts = [0, 0];
            private readonly int[] sqlSearchCounts = [0, 0];
            private readonly int[] markDownCounts = [0, 0];
            private readonly int[] solarSystemCounts = [0, 0];
            private readonly int[] gaussianCounts = [0, 0];
            private readonly int[] pskCounts = [0, 0];
            public CPU(PCTestPage ParentClass)                                                                          
            {
                this.ParentClass = ParentClass;                                                                         //父类赋值
                originalAffinity = process.ProcessorAffinity;                                                           //获取原始CPU亲和性
            }
            private void RunSingleTest()                                                                                //单核测试函数
            {
                process.ProcessorAffinity = 0x0001;                                                                     //绑定核心
                Thread thread = new(() =>                                                                               
                {
                    try
                    {
                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核加密算法"; });
                        aesCounts[0] = WorkLoad.CPU.Crytography.AES();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核路径算法"; });
                        navigationCounts[0] = WorkLoad.CPU.Integer.Navigation();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核压缩算法"; });
                        textCompressCounts[0] = WorkLoad.CPU.Integer.TextCompress();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核解压算法"; });
                        textDeCompressCounts[0] = WorkLoad.CPU.Integer.TextDeCompress();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核图片压缩"; });
                        imageCompressCounts[0] = WorkLoad.CPU.Integer.ImageCompress();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核SQL查询"; });
                        sqlSearchCounts[0] = WorkLoad.CPU.Integer.SQLSearch();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核MD渲染"; });
                        markDownCounts[0] = WorkLoad.CPU.Integer.MarkDown();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核行星轨迹运算"; });
                        solarSystemCounts[0] = WorkLoad.CPU.Floating.SolarSystem();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核高斯模糊运算"; });
                        gaussianCounts[0] = WorkLoad.CPU.Floating.GaussianBlur();

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核PSK处理"; });
                        pskCounts[0] = WorkLoad.CPU.Floating.PSK();
                    }
                    catch (ThreadAbortException) { }
                    catch (TaskCanceledException) { }
                });
                thread.Start();                                                                                         
                thread.Join();                                                                                          //需要等待测试完成，否则直接跳到多核测试了
            }

            private void RunMultiTest()                                                                                 //多核测试函数
            {
                process.ProcessorAffinity = originalAffinity;                                                           //释放核心绑定
                int theadNumber = (int)(coreCount/2.0);                                                                 //多核测试的线程数，
                try
                {
                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核加密算法"; });
                    aesCounts[1] = RunTestsInParallel(WorkLoad.CPU.Crytography.AES, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核路径算法"; });
                    navigationCounts[1] = RunTestsInParallel(WorkLoad.CPU.Integer.Navigation, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核压缩算法"; });
                    textCompressCounts[1] = RunTestsInParallel(WorkLoad.CPU.Integer.TextCompress, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核解压算法"; });
                    textDeCompressCounts[1] = RunTestsInParallel(WorkLoad.CPU.Integer.TextDeCompress, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核图片压缩"; });
                    imageCompressCounts[1] = RunTestsInParallel(WorkLoad.CPU.Integer.ImageCompress, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核SQL查询"; });
                    sqlSearchCounts[1] = RunTestsInParallel(WorkLoad.CPU.Integer.SQLSearch, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核MD渲染"; });
                    markDownCounts[1] = RunTestsInParallel(WorkLoad.CPU.Integer.MarkDown, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核行星轨迹运算"; });
                    solarSystemCounts[1] = RunTestsInParallel(WorkLoad.CPU.Floating.SolarSystem, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核高斯模糊运算"; });
                    gaussianCounts[1] = RunTestsInParallel(WorkLoad.CPU.Floating.GaussianBlur, theadNumber).Sum();

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核PSK处理"; });
                    pskCounts[1] = RunTestsInParallel(WorkLoad.CPU.Floating.PSK, theadNumber).Sum();
                }
                catch (ThreadAbortException) { }
                catch (TaskCanceledException) { }
            }

            public void RunTest()                                                                                    //开始测试
            {
                Thread.Sleep(1000);
                RunSingleTest();
                Thread.Sleep(1000);
                RunMultiTest();
                double rate = 1;
                double[] scoreWeight =
                    [
                        0.102 * rate, 
                        0.0000129 * rate,
                        0.143 * rate,
                        0.0000912 * rate,
                        2.567 * rate,
                        0.1826 * rate,
                        0.0586 * rate,
                        0.0932 * rate,
                        14.97 * rate,
                        0.0323 * rate,
                    ];


                CPUSingleCoreScore = (int)
                    (
                        aesCounts[0] * scoreWeight[0] + 
                        navigationCounts[0] * scoreWeight[1] + 
                        textCompressCounts[0] * scoreWeight[2]+
                        textDeCompressCounts[0] * scoreWeight[3]+
                        imageCompressCounts[0] * scoreWeight[4]+
                        sqlSearchCounts[0] * scoreWeight[5]+
                        markDownCounts[0] * scoreWeight[6]+
                        solarSystemCounts[0] * scoreWeight[7]+
                        gaussianCounts[0] * scoreWeight[8]+
                        pskCounts[0] * scoreWeight[9]
                    );

                CPUMultiCoreScore = (int)
                    (
                        aesCounts[1] * scoreWeight[0] + 
                        navigationCounts[1] * scoreWeight[1] + 
                        textCompressCounts[1] * scoreWeight[2]+
                        textDeCompressCounts[1] * scoreWeight[3]+
                        imageCompressCounts[1] * scoreWeight[4]+
                        sqlSearchCounts[1] * scoreWeight[5]+
                        markDownCounts[1] * scoreWeight[6]+
                        solarSystemCounts[1] * scoreWeight[7]+
                        gaussianCounts[1] * scoreWeight[8]+
                        pskCounts[1] * scoreWeight[9]
                    );
                //void WriteSocre() 
                //{
                //    string path = @"test.txt";
                //    using StreamWriter sw = File.AppendText(path);
                //    sw.WriteLine(aesCounts[0] * 0.4 + aesCounts[1] * 0.6);
                //    sw.WriteLine(navigationCounts[0] * 0.4 + navigationCounts[1] * 0.6);
                //    sw.WriteLine(textCompressCounts[0] * 0.4 + textCompressCounts[1] * 0.6);
                //    sw.WriteLine(textDeCompressCounts[0] * 0.4 + textDeCompressCounts[1] * 0.6);
                //    sw.WriteLine(imageCompressCounts[0] * 0.4 + imageCompressCounts[1] * 0.6);
                //    sw.WriteLine(sqlSearchCounts[0] * 0.4 + sqlSearchCounts[1] * 0.6);
                //    sw.WriteLine(markDownCounts[0] * 0.4 + markDownCounts[1] * 0.6);
                //    sw.WriteLine(solarSystemCounts[0] * 0.4 + solarSystemCounts[1] * 0.6);
                //    sw.WriteLine(gaussianCounts[0] * 0.4 + gaussianCounts[1] * 0.6);
                //    sw.WriteLine(pskCounts[0] * 0.4 + pskCounts[1] * 0.6);
                //}
            }
            private static int[] RunTestsInParallel(Func<int> testFunc, int coreCount)
            {
                Thread[] threads = new Thread[coreCount];                                                               //创建线程组，数目为逻辑核心数
                int[] testCounts = new int[coreCount];                                                                  //创建测试计数组，数目为逻辑核心数目
                for (int i = 0; i < coreCount; i++)
                {
                    int coreIndex = i;                                                                                  //闭包，此时还没join，i作为外部变量会被改变，因此固化为本地循环值
                    threads[i] = new Thread(() =>
                    {
                        testCounts[coreIndex] = testFunc();
                    });
                }
                foreach (Thread thread in threads) { thread.Start(); }
                foreach (Thread thread in threads) { thread.Join(); }
                return testCounts;                                                                                      //执行完毕后返回计数组
            }
        }

        public class RAM 
        {
            private int readSpeed = 0;
            private int writeSpeed = 0;
            public int readScore = 0;
            public int writeScore = 0;
            private readonly PCTestPage ParentClass;
            
            public RAM(PCTestPage ParentClass)
            {
                this.ParentClass = ParentClass;
            }

            public void RunTest() 
            {
                ParentClass.Dispatcher.Invoke(() => { ParentClass.RAMDialogScoreLabel.Content = "内存写入测试"; });
                writeSpeed = WorkLoad.RAM.RamWriteSpeed();
                writeScore = (int)(writeSpeed * 0.5 / 8.4);

                ParentClass.Dispatcher.Invoke(() => {ParentClass.RAMDialogScoreLabel.Content = "内存读取测试"; });
                readSpeed = WorkLoad.RAM.RamReadSpeed();
                readScore = (int)(readSpeed * 0.5 / 8.4);
            }
        }

        public class Disk 
        {
            private int readSpeed = 0;
            private int writeSpeed = 0;
            public int readScore = 0;
            public int writeScore = 0;
            private readonly PCTestPage ParentClass;

            public Disk(PCTestPage ParentClass)
            {
                this.ParentClass = ParentClass;
            }

            public void RunTest()
            {
                ParentClass.Dispatcher.BeginInvoke(() => { ParentClass.DiskDialogScoreLabel.Content = "硬盘写入测试"; });
                writeSpeed = WorkLoad.Disk.DiskWriteRandomSpeed();
                ParentClass.Dispatcher.BeginInvoke(() => { ParentClass.DiskDialogScoreLabel.Content = "硬盘读取测试"; });
                readSpeed = WorkLoad.Disk.DiskReadRandomSpeed();
                readScore = (int)(readSpeed * 0.04);
                writeScore = (int)(writeSpeed * 0.3);
            }
        }
    }

}
