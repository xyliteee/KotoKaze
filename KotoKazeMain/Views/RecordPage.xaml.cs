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
        public RecordPage(Dictionary<string, List<double>> recordData)
        {
            InitializeComponent();
            this.recordData = recordData;
            CPULoad = recordData["CPU_Load"];
            CPUPower = recordData["CPU_Power"];
            GPUPower = recordData["CoreGPU_Power"];
            RAMLoad = recordData["RAM_Load"];
            CPUTemp = recordData["CPU_Temp"];
            SeriesCollection series =
            [
                new LineSeries
                {
                    Values = new ChartValues<double>(CPULoad),
                    Stroke = new SolidColorBrush(Colors.Chocolate),
                    Fill=new SolidColorBrush(Colors.Transparent),
                    Title = "CPU占用",
                    ScalesYAt = 0,
                },
                new LineSeries
                {
                    Values = new ChartValues<double>(CPUPower),
                    Stroke = new SolidColorBrush(Colors.DarkRed),
                    Fill=new SolidColorBrush(Colors.Transparent),
                    Title = "CPU功耗",
                    ScalesYAt = 1,
                },
                new LineSeries
                {
                    Values = new ChartValues<double>(CPUTemp),
                    Stroke = new SolidColorBrush(Colors.MediumVioletRed),
                    Fill=new SolidColorBrush(Colors.Transparent),
                    Title = "CPU温度",
                    ScalesYAt = 2,
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
                 MaxValue = CPUPower.Max(),
                 LabelFormatter = value => value.ToString() + "W"
            };
            Axis tempY = new()
            {
                MinValue = 0,
                MaxValue = 100,
                LabelFormatter = value => value.ToString() + "°C"
            };
            CPU_Chrat.AxisY.Clear();
            CPU_Chrat.AxisY.Add(loadY);
            CPU_Chrat.AxisY.Add(powerY);
            CPU_Chrat.AxisY.Add(tempY);
            CPU_Chrat.Series = series;
        }
    }
}
