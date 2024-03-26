using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace XylitolSignal4Csharp
{
    public class SingleFunctions                                                    //这是一个一个的单独功能函数或者构造
    {
        public static double globalFs = 4000;
        private static double[] SubArray(double[] data, int index, int length, int step)
        {
            double[] result = new double[length / step];
            for (int i = 0; i < length / step; i++)
            {
                result[i] = data[index + i * step];
            }
            return result;
        }

        static public double[] Complex2Double(Complex[] input)                      //将Complex数组转为Double数组
        {
            double[] l = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                l[i] = (input[i].Real);
            }
            return l;
        }

        static public Complex[] Double2Complex(double[] input)                      //将Double数组转为Complex数组
        {
            Complex[] l = new Complex[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                l[i] = new Complex(input[i], 0);
            }
            return l;
        }
        public static double[] ExtendToPowerOfTwo(double[] input)                   //将列表元素补0直到长度满足2的幂次
        {
            int n = input.Length;
            int m = (int)Math.Ceiling(Math.Log(n, 2));
            int newLength = (int)Math.Pow(2, m);
            double[] result = new double[newLength];
            Array.Copy(input, result, n);
            return result;
        }

        static public double[] CoherentDemodulation(double[] wave1, double[]wave2) //相干解调，其实就是两个信号相乘
        {
            double[] result = new double[wave1.Length];
            for (int i = 0; i < wave1.Length; i++) 
            {
                result[i] = wave1[i] * wave2[i];
            }
            return result;
        }

        public class JudgeAndDecoded                                                //创建类，用于管理最后的判定和解码
        {
            private double[] symbolGroup =[];
            private double[] reSymbolGroup = [];
            public double[] baseBandWave = [];
            public double[] reBaseBandWave =[];
            public string originalStr = string.Empty;
            public string reOriginalStr = string.Empty;
            private readonly double[] input =[];
            public JudgeAndDecoded(double[] input)                                  //构造函数，接受一个输入的信号流
            {
                this.input = input;
                Judege();
                Decoded();
            }
            private void Judege()                                                   //判决函数
            {
                double[][] symbolDataGroups = SplitArray(input, (int)(1 / 200.0 * globalFs));//把函数按照码元长度分组
                List<double> symbolGroup = new List<double>();                      //这个是码元组
                List<double> reSymbolGroup = new List<double>();
                List<double> baseBandWave = new List<double>();                     //这个是基带信号组（每个码元进行重复）
                List<double> reBaseBandWave = new List<double>();
                double maxValue = input.Max();                                      
                double minValue = input.Min();
                double judgeValue = (maxValue+minValue)/2;                          //选择一个合适的判定阈值
                foreach (double[] symbolDataGroup in symbolDataGroups)
                {
                    int flag = 0;
                    foreach (double symbolData in symbolDataGroup)
                    {
                        if (symbolData >= judgeValue) { flag++; }
                        else { flag--; }
                    }
                    if (flag >= 0 ) { symbolGroup.Add(0); reSymbolGroup.Add(1); }        
                    else { symbolGroup.Add(1); reSymbolGroup.Add(0); }
                }
                this.symbolGroup = symbolGroup.ToArray();                           //赋值给码元组
                this.reSymbolGroup = reSymbolGroup.ToArray();

                foreach(double baseBand in this.symbolGroup)                        //重复采样为基带信号组
                {
                    for (int i = 0; i < (int)(1 / 200.0 * globalFs); i++) 
                    {
                        baseBandWave.Add(baseBand);
                    }
                }
                this.baseBandWave = baseBandWave.ToArray();
                foreach (double baseBand in this.reSymbolGroup)
                {
                    for (int i = 0; i < (int)(1 / 200.0 * globalFs); i++)
                    {
                        reBaseBandWave.Add(baseBand);
                    }
                }
                this.reBaseBandWave = reBaseBandWave.ToArray();
            }
            private void Decoded()                                                  //解码
            {
                double[][] wordsBitsGroup = SplitArray(symbolGroup, 16);            //将码元按照16个（unicode）分一组
                double[][] reWordsBitsGroup = SplitArray(reSymbolGroup, 16);
                foreach (double[] wordsBits in wordsBitsGroup) 
                {
                    string binaryString = string.Join("", wordsBits);               //将每组码元转为字符串
                    int num = Convert.ToInt32(binaryString, 2);                     //字符串转整数串
                    char ch = Convert.ToChar(num);                                  //整数串编码回字符
                    originalStr += ch;                                              //字符添加到整个字符串上
                }

                foreach (double[] wordsBits in reWordsBitsGroup)
                {
                    string binaryString = string.Join("", wordsBits);               
                    int num = Convert.ToInt32(binaryString, 2);                     
                    char ch = Convert.ToChar(num);                                  
                    reOriginalStr += ch;                                            
                }
            }
        }

        

        public static T[][] SplitArray<T>(T[] data, int subSize)                    //分组的函数，其实就是新建立个二维组然后对原本的组进行索引往里塞
        {
            int count = (data.Length + subSize - 1) / subSize;
            T[][] result = new T[count][];
            for (int i = 0; i < count; i++)
            {
                int startIndex = i * subSize;
                int length = Math.Min(subSize, data.Length - startIndex);
                result[i] = new T[length];
                Array.Copy(data, startIndex, result[i], 0, length);
            }
            return result;
        }

        public static Complex[] FFT(Complex[] x)                                    //FFT涉及复数，使用Complex类型，但是整个生命周期没有使用复数的虚部，后续数据使用转换提取实部保存为double
        {
            int N = x.Length;
            if (N == 1)
                return new Complex[] { x[0] };

            Complex[] even = new Complex[N / 2];
            Complex[] odd = new Complex[N / 2];
            for (int i = 0; i < N / 2; i++)
            {
                even[i] = x[2 * i];
                odd[i] = x[2 * i + 1];
            }

            Complex[] q = FFT(even);
            Complex[] r = FFT(odd);

            Complex[] y = new Complex[N];
            for (int k = 0; k < N / 2; k++)
            {
                Complex t = Complex.Exp(new Complex(0, -2.0 * Math.PI * k / N)) * r[k];
                y[k] = q[k] + t;
                y[k + N / 2] = q[k] - t;
            }
            return y;
        }
    
        public static double[] PSKCarrier(double[] input)                           //PSK调制波的制作，就是两个if的事情
        {
            SinWave sin1 = new (f0: 1000, phi: 90, fs: globalFs, time: input.Length / globalFs);
            SinWave sin2 = new (f0: 1000, phi: 270, fs: globalFs, time: input.Length / globalFs);
            double[] carrier = new double[input.Length];
            for (int i = 0; i < input.Length; i++) 
            {
                if (input[i] <= 0.5) 
                {
                    carrier[i] = sin1.timeFomainWaveForm[i];
                }
                else 
                {
                    carrier[i] = sin2.timeFomainWaveForm[i];
                }
            }
            return carrier;
        }
    }

    public class NoiseMix                                                           //噪声混合类，因为有另外一个方法，丢在一个类中
    {
        private static readonly Random random = new Random();
        public static double[] AddNoise(double[] oriSignal, double SNR = 0.5)
        {
            Complex[] signal = SingleFunctions.Double2Complex(oriSignal);
            int len = signal.Length;
            double signalPower = signal.Select(x => x.Magnitude).Average();
            double noisePower = signalPower / Math.Pow(10, SNR / 10);
            Complex[] noise = new Complex[len];
            for (int i = 0; i < len; i++)
            {
                double real = Math.Sqrt(noisePower) * NextGaussian();
                double imaginary = Math.Sqrt(noisePower) * NextGaussian();
                noise[i] = new Complex(real, imaginary);
            }
            Complex[] noisySignal = new Complex[len];
            for (int i = 0; i < len; i++)
            {
                noisySignal[i] = signal[i] + noise[i];
            }
            double[] finnalSignal = SingleFunctions.Complex2Double(noisySignal);

            return finnalSignal;
        }

        private static double NextGaussian()
        {
            double u, v, S;

            do
            {
                u = 2.0 * random.NextDouble() - 1.0;
                v = 2.0 * random.NextDouble() - 1.0;
                S = u * u + v * v;
            }
            while (S >= 1.0);

            double fac = Math.Sqrt(-2.0 * Math.Log(S) / S);
            return u * fac;
        }
    }
}
