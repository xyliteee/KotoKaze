using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.Wpf.Charts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KotoKaze.Static;

namespace KotoKaze.Views
{
    /// <summary>
    /// RecordPage.xaml 的交互逻辑
    /// </summary>
    public partial class RecordPage : Page
    {
        private readonly Dictionary<string, List<double>> recordData;
        private readonly List<double> CPULoad;
        private readonly List<double> CPUPower;
        private readonly List<double> GPUPower;
        private readonly List<double> RAMLoad;
        private readonly List<double> CPUTemp;
        private readonly double RefreshTime;

        private readonly static double lineThick = 1;
        public RecordPage(Dictionary<string, List<double>> recordData)
        {
            InitializeComponent();
            this.recordData = recordData;
            CPULoad = this.recordData["CPU_Load"];
            CPUPower = this.recordData["CPU_Power"];
            GPUPower = this.recordData["CoreGPU_Power"];
            RAMLoad = this.recordData["RAM_Load"];
            CPUTemp = this.recordData["CPU_Temp"];
            RefreshTime = this.recordData["Refresh_Time"][0];
            DrawChart();
            DataSummary();
        }
        private void DrawChart() 
        {
            DrawCPU();
            DrawRAM();
            DrawGPU();
        }
        private void DrawCPU() 
        {
            SeriesCollection CPUseries =
            [
                new LineSeries
                {
                    Values = new ChartValues<double>(CPULoad),
                    Stroke = new SolidColorBrush(Colors.DeepSkyBlue),
                    Fill=new SolidColorBrush(Colors.Transparent),
                    Title = "CPU占用",
                    PointGeometry = null,
                    ScalesYAt = 0,
                    ScalesXAt = 0,
                    StrokeThickness = lineThick
                },
                new LineSeries
                {
                    Values = new ChartValues<double>(CPUPower),
                    Stroke = new SolidColorBrush(Colors.DarkOrange),
                    Fill=new SolidColorBrush(Colors.Transparent),
                    Title = "CPU功耗",
                    PointGeometry = null,
                    ScalesYAt = 1,
                    ScalesXAt = 0,
                    StrokeThickness = lineThick
                },
                new LineSeries
                {
                    Values = new ChartValues<double>(CPUTemp),
                    Stroke = new SolidColorBrush(Colors.OrangeRed),
                    Fill=new SolidColorBrush(Colors.Transparent),
                    Title = "CPU温度",
                    PointGeometry = null,
                    ScalesYAt = 2,
                    ScalesXAt = 0,
                    StrokeThickness = lineThick
                }

            ];
            Axis loadY = new()
            {
                MinValue = 0,
                MaxValue = 110,
                LabelFormatter = value => value.ToString() + "%",
            };
            Axis powerY = new()
            {
                MinValue = 0,
                MaxValue = CPUPower.Max()*1.1,
                LabelFormatter = value => value.ToString() + "W"
            };
            Axis tempY = new()
            {
                MinValue = 0,
                MaxValue = 100,
                LabelFormatter = value => value.ToString() + "°C",
                Position = AxisPosition.RightTop
            };
            Axis TimeX = new()
            {
                LabelFormatter = value => (value * RefreshTime).ToString() + "秒",
            };

            CPU_Chrat.AxisY.Clear();
            CPU_Chrat.AxisX.Clear();
            CPU_Chrat.AxisY.Add(loadY);
            CPU_Chrat.AxisY.Add(powerY);
            CPU_Chrat.AxisY.Add(tempY);
            CPU_Chrat.AxisX.Add(TimeX);
            CPU_Chrat.Series = CPUseries;
        }
        private void DrawRAM() 
        {
            SeriesCollection RAMseries =
            [
                new LineSeries
                {
                    Values = new ChartValues<double>(RAMLoad),
                    Stroke = new SolidColorBrush(Colors.DeepSkyBlue),
                    Fill=new SolidColorBrush(Colors.Transparent),
                    Title = "内存占用",
                    PointGeometry = null,
                    ScalesYAt = 0,
                    ScalesXAt = 0,
                    StrokeThickness = lineThick
                },
            ];
            Axis loadY = new()
            {
                MinValue = 0,
                MaxValue = 110,
                LabelFormatter = value => value.ToString() + "%",
            };
            Axis TimeX = new()
            {
                LabelFormatter = value => (value * RefreshTime).ToString() + "秒",
            };
            RAM_Chrat.AxisY.Clear();
            RAM_Chrat.AxisX.Clear();
            RAM_Chrat.AxisY.Add(loadY);
            RAM_Chrat.AxisX.Add(TimeX);
            RAM_Chrat.Series = RAMseries;
        }
        private void DrawGPU() 
        {
            SeriesCollection GPUseries =
            [
                new LineSeries
                {
                    Values = new ChartValues<double>(GPUPower),
                    Stroke = new SolidColorBrush(Colors.DarkOrange),
                    Fill=new SolidColorBrush(Colors.Transparent),
                    Title = "核心显卡功耗",
                    PointGeometry = null,
                    ScalesYAt = 0,
                    ScalesXAt = 0,
                    StrokeThickness = lineThick
                },
            ];
            Axis powerY = new()
            {
                MinValue = 0,
                MaxValue = GPUPower.Max()*1.1,
                LabelFormatter = value => value.ToString() + "W"
            };
            Axis TimeX = new()
            {
                LabelFormatter = value => (value*RefreshTime).ToString() + "秒",
            };
            GPU_Chrat.AxisY.Clear();
            GPU_Chrat.AxisX.Clear();
            GPU_Chrat.AxisY.Add(powerY);
            GPU_Chrat.AxisX.Add(TimeX);
            GPU_Chrat.Series = GPUseries;
        }
        private void DataSummary() 
        {
            Task.Run(() => 
            {
                double CPULoadAvg = Math.Round(CPULoad.Average(),2);
                double CPULoadMax = CPULoad.Max();
                double CPUPowerAvg = Math.Round(CPUPower.Average(),2);
                double CPUPowerMax = CPUPower.Max();
                double CPUTempMax = CPUTemp.Max();
                double RecordTime = (CPULoad.Count-1) * RefreshTime;
                Dispatcher.Invoke(() => 
                {
                    CPULoadAVGLable.Content = CPULoadAvg + "%";
                    CPULoadMaxLable.Content = CPULoadMax + "%";
                    CPUPowerAvgLable.Content = CPUPowerAvg + "W";
                    CPUPowerMaxLable.Content = CPUPowerMax + "W";
                    CPUTempMaxLable.Content = CPUTempMax + "°C";
                    RecordTimeLable.Content = RecordTime + "秒";
                });
            });
        }
    }
}
