
namespace XylitolSignal4Csharp                                          //这个文件主要用来定义一些信号类型
{
    public class BaseBandSignal                                         //定义基带信号类
    {
        public double[] timeFomainWaveForm;                             //公有声明时域信号
        public double time;                                             //公有声明整个信号时间长度

        public BaseBandSignal(double fs,string message, double T = 1.0 / 200)
        {
            int N = (int)(fs * T);                                      //算每个码元的采样数
            List<int> unicodeBinaryValues = new List<int>();            //声明一个列表，用于存储unicode码
            for (int i = 0; i < message.Length; i++)
            {
                string binary = Convert.ToString(message[i], 2).PadLeft(16, '0');//对输入的字符串解码，春促在上述列表
                for (int j = 0; j < 16; j++)
                {
                    unicodeBinaryValues.Add(int.Parse(binary[j].ToString()));
                }
            }

            timeFomainWaveForm = new double[N * unicodeBinaryValues.Count]; //提前设置好长度，数组是这样的
            List<double> temp = new List<double>();                         //声明好列表，一会用于转换，ADD方法实在太好用辣（确信）
            foreach (int code in unicodeBinaryValues)
            {
                for (int i = 0; i < N; i++)
                {
                    temp.Add(code);                                         //重复N次
                }
            }
            timeFomainWaveForm = temp.ToArray();                            //赋值给时域信号
            time = timeFomainWaveForm.Length / SingleFunctions.globalFs;    //算信号长度，就是总采样数除以采样率
        }
    }
    public class SinWave                                                    //定义正弦信号类型
    {
        public double[] timeFomainWaveForm;                                 //公有声明时域信号
        public SinWave(double fs,int f0 = 1000, double A = 1, int phi = 0, double time = 1)//参数，懒得解释了
        {
            int N = (int)(fs * time);                                       //算采样点
            double T = 1.0 / fs;                                            //计算周期
            timeFomainWaveForm = new double[N];                             //设置好数组的长度
            for (int index = 0; index < N; index++)
            {
                timeFomainWaveForm[index] = A * Math.Sin(2 * f0 * Math.PI * index * T + phi);//将对应点的Sin值算好塞进数组对应索引
            }
        }
    }

    public class Filter                                                     //定义父类滤波器
    {
        public double[] impulseResponse =[];                                    //公有声明冲激响应
        protected double[] window =[];                                          //声明窗

        public Filter(int N = 32,string window = "hamming")                 //构造函数
        {
            double[] hanningWindow = new double[N];                         //声明窗，设置好长度
            double[] hammingWindow = new double[N];
            double[] blackmanWindow = new double[N];
            impulseResponse = new double[N];                                //设置好冲激响应的长度

            for (int i = 0; i < N; i++)                                     //构造窗
            {
                hanningWindow[i] = 0.5 - 0.5 * Math.Cos(2 * Math.PI * i / (N - 1));
                hammingWindow[i] = 0.54 - 0.46 * Math.Cos(2 * Math.PI * i / (N - 1));
                blackmanWindow[i] = 0.42 - 0.5 * Math.Cos((2 * Math.PI * i) / (N - 1)) + 0.08 * Math.Cos((4 * Math.PI * i) / (N - 1));
            }

            switch (window)                                                 //匹配选择                       
            {
                case "hamming":
                    this.window = hammingWindow;
                    break;
                case "blackman":
                    this.window = blackmanWindow;
                    break;
                case "hanning":
                    this.window = hanningWindow;
                    break;
            }
        }
        private static double[] Convolve(double[] kernel, double[] signal)  //声明私有方法卷积，因为目前只有滤波器在用，就放在滤波器类里了
        {
            int signal_len = signal.Length;                                 //获取两个信号的长度
            int kernel_len = kernel.Length;
            double[] result = new double[signal_len + kernel_len - 1];      //设置好输出信号的长度

            for (int i = 0; i < signal_len; i++)                            //卷呗
            {
                for (int j = 0; j < kernel_len; j++)
                {
                    result[i + j] += signal[i] * kernel[j];
                }
            }

            int start = (kernel_len - 1) / 2;
            double[] finalResult = new double[signal_len];
            Array.Copy(result, start, finalResult, 0, signal_len);          //舍弃前后保留中间的

            return finalResult;
        }


        public double[] Filte(double[] input)                               //调用卷积进行滤波
        {
            double[] res = Convolve(impulseResponse, input);                //我可以直接写一块的我为什么要分开写(-_-||)
            return res;
        }
    }

    public class Filter_LowPass : Filter                                    //滤波器的子类，低通滤波器
    {
        public Filter_LowPass(double fs,int N = 32, double f_stop = 500, string window = "hamming") : base(N,window)//构造函数，同时继承于父类
        {
            double omega_c = 2 * Math.PI * f_stop / fs;                     //归一化
            for (int i = 0; i < N; i++)                                     //计算每点冲激响应
            {
                if (i != N / 2)
                {
                    double value = Math.Sin(omega_c * (i - N / 2)) / (Math.PI * (i - N / 2));
                    impulseResponse[i] = value;
                }
            }
            impulseResponse[N / 2] = omega_c / Math.PI;                     //N/2单独赋值
            for (int i = 0; i < N; i++)
            {
                impulseResponse[i] *= this.window[i];                       //对应相乘即可完成加窗
            }
        }
    }

    public class Filter_HighPass : Filter                                   //一样的懒得写了
    {
        public Filter_HighPass(double fs,int N = 32, double f_stop = 500, string window = "hamming") : base(N,window)
        {
            double omega_c = 2 * Math.PI * f_stop / fs;
            for (int i = 0; i < N; i++)
            {
                if (i != N / 2)
                {
                    double value = -Math.Sin(omega_c * (i - N / 2)) / (Math.PI * (i - N / 2));
                    impulseResponse[i] = value;
                }
            }
            impulseResponse[N / 2] = 1 - omega_c / Math.PI;
            for (int i = 0; i < N; i++)
            {
                impulseResponse[i] *= this.window[i];
            }
        }
    }

    public class Filter_BandPass : Filter
    {
        public Filter_BandPass(double fs = 8000,int N = 32, double f_start = 1000, double f_stop = 2000, string window = "hamming") : base(N,window)
        {
            double omega_c1 = 2 * Math.PI * f_start / fs;
            double omega_c2 = 2 * Math.PI * f_stop / fs;
            for (int i = 0; i < N; i++)
            {
                if (i != N / 2)
                {
                    double value = (Math.Sin(omega_c2 * (i - N / 2)) - Math.Sin(omega_c1 * (i - N / 2))) / (Math.PI * (i - N / 2));
                    impulseResponse[i] = value;
                }
            }
            impulseResponse[N / 2] = (omega_c2 - omega_c1) / Math.PI;
            for (int i = 0; i < N; i++)
            {
                impulseResponse[i] *= this.window[i];
            }
        }
    }
}
