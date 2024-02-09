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
        public void CreateCube()
        {
            // 创建一个3D立方体
            var cube = new GeometryModel3D();
            var mesh = new MeshGeometry3D();
            // 添加立方体的顶点和三角形到mesh中
            // ...
            cube.Geometry = mesh;

            // 添加立方体到3D视图中
            var modelVisual3D = new ModelVisual3D { Content = cube };
            v3D.Children.Add(modelVisual3D);

            // 创建一个旋转变换
            var rotateTransform = new RotateTransform3D();
            cube.Transform = rotateTransform;

            // 创建一个旋转动画
            var rotationAnimation = new Rotation3DAnimation
            {
                From = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0),
                To = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 360),
                Duration = TimeSpan.FromSeconds(2),
                RepeatBehavior = RepeatBehavior.Forever
            };

            // 开始旋转动画
            rotateTransform.BeginAnimation(RotateTransform3D.RotationProperty, rotationAnimation);
        }


        public void Test()
        {
            CreateCube();
            CompositionTarget.Rendering += OnRendering!;
            stopwatch.Start();
        }


    }
}
