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
        private readonly List<Image> TestImages = [];
        public GPUTestWindow()
        {
            InitializeComponent();
            stopwatch = new Stopwatch();
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            stopwatch.Stop();
            double currentTime = stopwatch.Elapsed.TotalMilliseconds;
            double frameTime = currentTime - lastTime;
            lastTime = currentTime;
            stopwatch.Start();
            frameTimes.Add(frameTime);
        }

        private void TestAction() 
        {
            for (int i = 0; i < 10; i++) 
            {
                Image image = new()
                {
                    Source = BitMapImages.TestImage
                };
                Base.Children.Add(image);
                BlurEffect blur = new();
                DoubleAnimation blurAnimation = new DoubleAnimation(0, 10, TimeSpan.FromSeconds(2))
                {
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(4)
                };
                blur.BeginAnimation(BlurEffect.RadiusProperty, blurAnimation);
                image.Effect = blur;
                ScaleTransform scale = new ScaleTransform(1.5, 1.5);
                DoubleAnimation scaleAnimation = new DoubleAnimation(1.5, 1, TimeSpan.FromSeconds(2));
                scaleAnimation.AutoReverse = true;
                scaleAnimation.RepeatBehavior = new RepeatBehavior(4);
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
                image.RenderTransform = scale;
                image.RenderTransformOrigin = new Point(0.5, 0.5); // 设置变换原点为中心
            }
        }

        public void Test()
        {
            TestAction();
            CompositionTarget.Rendering += OnRendering!;
            stopwatch.Start();
        }
    }
}
