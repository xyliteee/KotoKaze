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
using System.Windows.Shapes;

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
        public void CreateImage()
        {
            // 创建Image控件
            Image TestImage = new()
            {
                // 设置Image的宽度和高度
                Width = this.ActualWidth * 1.5,
                Height = this.ActualHeight * 1.5
            };

            // 设置Image的位置
            Canvas.SetLeft(TestImage, this.ActualWidth * -0.25);
            Canvas.SetTop(TestImage, this.ActualHeight * -0.25);

            // 设置Image的源
            TestImage.Source = new BitmapImage(new Uri(@"F:\project\VS\Cs\Graduate\Xyliteee\XyliteeeMainForm\image\Header\TestImage.jpg"));

            // 将Image添加到Canvas中
            Base.Children.Add(TestImage);
            TestImages.Add(TestImage);
        }


        public void Test()
        {
            for (int i = 0; i < 50; i++) 
            {
                CreateImage();
            }

            CompositionTarget.Rendering += OnRendering!;

            // 创建一个改变模糊度的动画
            BlurEffect blur = new() { Radius = 0 };
            DoubleAnimation blurAnimation = new(0, 20, new Duration(TimeSpan.FromSeconds(10)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            blur.BeginAnimation(BlurEffect.RadiusProperty, blurAnimation);

            // 创建一个改变位置的动画
            TranslateTransform transform = new();

            DoubleAnimation translateAnimation = new(-40, 40, new Duration(TimeSpan.FromSeconds(2)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            transform.BeginAnimation(TranslateTransform.XProperty, translateAnimation);
            transform.BeginAnimation(TranslateTransform.YProperty, translateAnimation);

            // 创建一个改变透明度的动画
            DoubleAnimation opacityAnimation = new(1, 0, new Duration(TimeSpan.FromSeconds(10)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            foreach (Image image in TestImages) 
            {
                image.Effect = blur;
                image.RenderTransform = transform;
                image.BeginAnimation(Image.OpacityProperty, opacityAnimation);
            }

            stopwatch.Start();
        }

    }
}
