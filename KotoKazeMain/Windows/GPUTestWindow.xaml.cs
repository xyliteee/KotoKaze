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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using XyliteeeMainForm;

namespace KotoKaze.Windows
{
    /// <summary>
    /// GPUTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GPUTestWindow : Window
    {
        private readonly Stopwatch stopwatch;
        private double lastTime;
        public List<double> frameTimes = [0];
        private static bool isRecorded = false; 
        public GPUTestWindow()
        {
            InitializeComponent();
            stopwatch = new Stopwatch();
            //WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (isRecorded) 
            {
                stopwatch.Stop();
                double currentTime = stopwatch.Elapsed.TotalMilliseconds;
                double frameTime = currentTime - lastTime;
                lastTime = currentTime;
                stopwatch.Start();
                frameTimes.Add(frameTime);
                //messageLable.Content = $"FPS:{Math.Round(1000.0 / frameTime,2)}";
            }
        }

        private void TestAction()
        {
            messageLable.Content = "暂时弃置，等待更新";
            isRecorded = true;
        }



        public void Test()
        {
            TestAction();
            CompositionTarget.Rendering += OnRendering!;
        }
    }
}
