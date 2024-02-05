﻿using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows;
using KotoKaze.Static;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Runtime.InteropServices;
using KotoKaze.Windows;
using HandyControl.Tools.Extension;
using SevenZip.Compression.LZ;

namespace XyliteeeMainForm.Views
{
    /// <summary>
    /// PCTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class PCTestPage : Page
    {
        private readonly MainWindow mainWindow;
        private readonly WorkLoadTest.CPU CPUTest;
        private readonly WorkLoadTest.RAM RAMTest;
        private readonly WorkLoadTest.Disk DiskTest;
        private GPUTestWindow GPUWindow;
        private int CPUScore = 0;
        private int GPUScore = 0;
        private int RamScore = 0;
        private int DiskScore = 0;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public PCTestPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            CPUTest = new(this);
            RAMTest = new(this);
            DiskTest = new(this);
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
            Dispatcher.Invoke(() => {CPUScoreLabel.Content = "正在测试";});
            CPUTest.RunTest();                                                                                          //执行CPU测试
            Dispatcher.Invoke(() =>
            {
                CPUScore = (int)(CPUTest.CPUSingleCoreScore * 0.4 + CPUTest.CPUMultiCoreScore * 0.6);                   //获取测试结果，单核和多核权重为0.4与0.6
                CPUScoreLabel.Content = CPUScore;
                CPUDialogScoreLabel.Content = $"{CPUTest.CPUSingleCoreScore}S-{CPUTest.CPUMultiCoreScore}M";
            });
            GC.Collect();
            Thread.Sleep(1000);
        }
        private void RunGPUTest() 
        {
            Dispatcher.Invoke(() => 
            {
                GPUWindow = new();
                GPUScoreLabel.Content = "正在测试";
                mainWindow.Hide();
                GPUWindow.Show();
                GPUWindow.Test();
            });
            Thread.Sleep(20000);
            Dispatcher.Invoke(() => 
            {
                double FPS = Math.Round(1000 / GPUWindow.frameTimes.Average(),2);
                GPUWindow.Close();
                mainWindow.Show();
                GPUScore = (int)(FPS*930);
                GPUScoreLabel.Content = GPUScore;
                GPUDialogScoreLabel.Content = $"平均帧率-{FPS}";
            });
        }
        private void RunRamTest() 
        {
            Dispatcher.Invoke(() => { RAMScoreLabel.Content = "正在测试"; });
            RAMTest.RunTest();
            Dispatcher.Invoke(() =>
            {
                RamScore = (int)(RAMTest.readScore * 0.5 + RAMTest.writeScore * 0.5);
                RAMScoreLabel.Content = RamScore;
                RAMDialogScoreLabel.Content = $"{RAMTest.writeScore}W-{RAMTest.readScore}R";
            });
            GC.Collect();
            Thread.Sleep(1000);
        }
        private void RunDiskTest() 
        {
            Dispatcher.Invoke(() => { DiskScoreLabel.Content = "正在测试"; });
            DiskTest.RunTest();
            Dispatcher.Invoke(() =>
            {
                DiskScore = (int)(DiskTest.readScore * 0.5 + DiskTest.writeScore * 0.5);
                DiskScoreLabel.Content = DiskScore;
                DiskDialogScoreLabel.Content = $"{DiskTest.writeScore}W-{DiskTest.readScore}R";
            });
            GC.Collect();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Animations.ImageTurnRound(TestImage, true);                                                                 //按下按钮，图标开始转
            TestButton.IsEnabled = false;                                                                               //此时禁用按钮
            ScoreLabel.Content = "正在测试";
            Task.Run(() => 
            {
                try
                {
                    RunCPUTest();
                    RunGPUTest();
                    RunRamTest();
                    RunDiskTest();
                    Dispatcher.Invoke(() =>                                                                                 //当所有测试完成时
                    {
                        ScoreLabel.Content = CPUScore + GPUScore + RamScore + DiskScore;
                        Animations.ImageTurnRound(TestImage, false);                                                        //别转了
                        TestButton.IsEnabled = true;                                                                        //恢复按钮
                    });
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
            private readonly double[] hdrCounts = [0, 0];
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

                        ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "单核HDR合成"; });
                        double hdrTime = WorkLoad.CPU.Floating.HDR();
                        hdrCounts[0] = 1.0 / hdrTime;
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

                    ParentClass.Dispatcher.Invoke(() => { ParentClass.CPUDialogScoreLabel.Content = "多核HDR合成"; });
                    double hdrtime = RunTestsInParallel(WorkLoad.CPU.Floating.HDR, theadNumber).Average() / theadNumber;
                    hdrCounts[1] = 1.0 / hdrtime;
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
                double rate = 1.28;
                double[] scoreWeight =
                    [
                        0.085 * rate, 
                        0.00001 * rate,
                        0.14 * rate,
                        0.000072 * rate,
                        2.08 * rate,
                        0.2 * rate,
                        0.05 * rate,
                        0.08 * rate,
                        10 * rate,
                        440000 * rate,
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
                        hdrCounts[0] * scoreWeight[9]
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
                        hdrCounts[1] * scoreWeight[9]
                    );
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
                int[] readSpeeds = new int[50];
                int[] writeSpeeds = new int[50];

                ParentClass.Dispatcher.Invoke(() => { ParentClass.RAMDialogScoreLabel.Content = "内存写入测试"; });
                for (int i = 0; i < 50; i++)
                {
                    writeSpeeds[i] = WorkLoad.RAM.RamWriteSpeed();
                }

                ParentClass.Dispatcher.Invoke(() => {ParentClass.RAMDialogScoreLabel.Content = "内存读取测试"; });
                for (int i = 0; i < 50; i++)
                {
                    readSpeeds[i] = WorkLoad.RAM.RamReadSpeed();
                }

                readSpeed = (int)readSpeeds.Average();
                writeSpeed = (int)writeSpeeds.Average();

                readScore = (int)(readSpeed*0.5);
                writeScore = (int)(writeSpeed*0.5);
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
                writeSpeed = WorkLoad.Disk.DiskWriteSpeed();
                ParentClass.Dispatcher.BeginInvoke(() => { ParentClass.DiskDialogScoreLabel.Content = "硬盘读取测试"; });
                readSpeed = WorkLoad.Disk.DiskReadSpeed();
                readScore = (int)(readSpeed/100.0);
                writeScore = (int)(writeSpeed/100.0);
            }
        }
    }

}
