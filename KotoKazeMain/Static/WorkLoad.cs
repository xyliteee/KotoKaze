
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Data.SQLite;
using System.Reflection;
using System.Security.Cryptography;
using System.Data;
using Markdig;
using SkiaSharp;
using KotoKaze.Dynamic;
using OpenCvSharp;
using System.Runtime.InteropServices;

namespace KotoKaze.Static
{
    public static class WorkLoad
    {
        public delegate void VoidFunction();
        public static class CPU
        {
            private static readonly int time = 5;
            public static class Crytography
            {
                public static int AES()
                {
                    int count = 0;
                    byte[] buffer = new byte[1 * 1024 * 1024];
                    using Aes aes = Aes.Create();
                    byte[] key = aes.Key;
                    byte[] iv = aes.IV;
                    using ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (stopwatch.Elapsed.TotalSeconds < time)
                    {
                        for (int i = 0; i < buffer.Length; i += aes.BlockSize)
                        {
                            encryptor.TransformBlock(buffer, i, aes.BlockSize, buffer, i);
                        }
                        count++;
                    }
                    return count;
                }
            }
            public static class Integer
            {
                private static int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
                {
                    int min = int.MaxValue;
                    int minIndex = 0;

                    for (int v = 0; v < verticesCount; ++v)
                    {
                        if (shortestPathTreeSet[v] == false && distance[v] <= min)
                        {
                            min = distance[v];
                            minIndex = v;
                        }
                    }
                    return minIndex;
                }
                public static int Navigation()
                {
                    int count = 0;
                    int[,] graph = new int[,]
                    {
                        { 0, 6, 0, 0, 0, 0, 0, 9, 0 },
                        { 6, 0, 9, 0, 0, 0, 0, 11, 0 },
                        { 0, 9, 0, 5, 0, 6, 0, 0, 2 },
                        { 0, 0, 5, 0, 9, 16, 0, 0, 0 },
                        { 0, 0, 0, 9, 0, 10, 0, 0, 0 },
                        { 0, 0, 6, 0, 10, 0, 2, 0, 0 },
                        { 0, 0, 0, 16, 0, 2, 0, 1, 6 },
                        { 9, 11, 0, 0, 0, 0, 1, 0, 5 },
                        { 0, 0, 2, 0, 0, 0, 6, 5, 0 }
                    };
                    int verticesCount = 9;
                    int source = 0;
                    int[] distance = new int[verticesCount];
                    bool[] shortestPathTreeSet = new bool[verticesCount];
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (stopwatch.Elapsed.TotalSeconds < time)
                    {
                        for (int i = 0; i < verticesCount; ++i)
                        {
                            distance[i] = int.MaxValue;
                            shortestPathTreeSet[i] = false;
                        }
                        distance[source] = 0;
                        for (int i = 0; i < verticesCount - 1; ++i)
                        {
                            int u = MinimumDistance(distance, shortestPathTreeSet, verticesCount);
                            shortestPathTreeSet[u] = true;

                            for (int v = 0; v < verticesCount; ++v)
                                if (!shortestPathTreeSet[v] && Convert.ToBoolean(graph[u, v]) && distance[u] != int.MaxValue && distance[u] + graph[u, v] < distance[v])
                                    distance[v] = distance[u] + graph[u, v];
                        }
                        count++;
                    }
                    
                    return count;
                }


                public static int TextCompress()
                {
                    int count = 0;
                    try 
                    {
                        using Stream inputFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("KotoKaze.otherSource.TestHtml.html")!;
                        SevenZip.Compression.LZMA.Encoder coder = new();
                        MemoryStream output = new();
                        coder.WriteCoderProperties(output);
                        output.Write(BitConverter.GetBytes(inputFile.Length), 0, 8);
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        while (stopwatch.Elapsed.TotalSeconds < time)
                        {
                            coder.Code(inputFile, output, inputFile.Length, -1, null);
                            output.SetLength(0);
                            count++;
                        }
                        inputFile.Close();
                    }catch (Exception e) {File.WriteAllText("text.txt",e.ToString()); }
                    return count;
                }

                public static int TextDeCompress()
                {
                    int count = 0;

                    using Stream inputFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("KotoKaze.otherSource.TestHtml.html")!;
                    SevenZip.Compression.LZMA.Encoder coder = new();
                    MemoryStream output = new();
                    coder.WriteCoderProperties(output);
                    output.Write(BitConverter.GetBytes(inputFile.Length), 0, 8);
                    coder.Code(inputFile, output, inputFile.Length, -1, null);
                    output.Flush();

                    SevenZip.Compression.LZMA.Decoder decoder = new();
                    byte[] properties = new byte[5];
                    output.Read(properties, 0, 5);
                    byte[] fileLengthBytes = new byte[8];
                    output.Read(fileLengthBytes, 0, 8);
                    long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);
                    decoder.SetDecoderProperties(properties);

                    MemoryStream decompressedOutput = new();
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (stopwatch.Elapsed.TotalSeconds < time)
                    {
                        decoder.Code(output, decompressedOutput, output.Length, fileLength, null);
                        decompressedOutput.SetLength(0);
                        count++;
                    }
                    
                    return count;
                }

                public static int ImageCompress()
                {
                    int count = 0;
                    ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new(1);
                    EncoderParameter myEncoderParameter = new(myEncoder, 50L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    using Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KotoKaze.image.Header.TestImage.jpg")!;
                    using Image image = Image.FromStream(imageStream);
                    using MemoryStream ms = new();
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (stopwatch.Elapsed.TotalSeconds < time)
                    {
                        image.Save(ms, jpgEncoder, myEncoderParameters);
                        ms.SetLength(0);
                        count++;
                    }
                    
                    return count;
                }

                public static int SQLSearch()
                {
                    int count = 0;
                    string tempPath = Path.GetTempFileName();
                    using (Stream sqlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KotoKaze.otherSource.testsql.sqlite")!)
                    using (FileStream fileStream = File.Create(tempPath))
                    {
                        sqlStream.CopyTo(fileStream);
                    }
                    using (SQLiteConnection sqliteConn = new($"data source={tempPath}"))
                    {
                        sqliteConn.Open();
                        using SQLiteCommand cmd = new();
                        cmd.Connection = sqliteConn;
                        cmd.CommandText = @"SELECT * FROM MyTable WHERE Name = 'DTC_125_10' AND Penetration = 577 AND Weight = 5.86 AND Velocity = 1000 AND Caliber = 120 AND Manufacturer = 'ManufacturerA' AND ManufactureDate = '2022-01-01' AND Deceleration = 20 AND Warhead = 'Tungsten' AND Price = 100000";
                        using SQLiteDataReader reader = cmd.ExecuteReader();
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        while (stopwatch.Elapsed.TotalSeconds < time)
                        {
                            for (int i = 0; i < 1000000; i++) { while (reader.Read()) { } }
                            count++;
                        }
                    }
                    
                    return count;
                }

                //public static int SQLSave()
                //{
                //    int count = 0;
                //    SQLiteConnection.CreateFile("test.sqlite");
                //    using (SQLiteConnection sqliteConn = new("data source=F://test.sqlite"))
                //    {
                //        sqliteConn.Open();
                //        using SQLiteCommand cmd = new();
                //        cmd.Connection = sqliteConn;
                //        cmd.CommandText = "CREATE TABLE IF NOT EXISTS MyTable (Name varchar, Penetration int, Weight decimal, Velocity float, Caliber int, Manufacturer varchar, ManufactureDate datetime, Deceleration int, Warhead varchar, Price decimal)";
                //        cmd.ExecuteNonQuery();

                //        string[] data =
                //            [
                //                "DTC_125_10,577,5.86,1000,120,ManufacturerA,2022-01-01,20,Tungsten,100000",
                //                "DM53,646,5.73,1100,130,ManufacturerB,2022-02-01,21,DepletedUranium,200000",
                //                "JM53,617,5.54,1200,140,ManufacturerC,2022-03-01,22,Steel,300000",
                //                "M829A1,625,5.91,1300,150,ManufacturerD,2022-04-01,23,Tungsten,400000",
                //                "3MB60,580,5.46,1400,160,ManufacturerE,2022-05-01,24,DepletedUranium,500000"
                //            ];

                //        using SQLiteTransaction transaction = sqliteConn.BeginTransaction();
                //        cmd.CommandText = "INSERT INTO MyTable (Name, Penetration, Weight, Velocity, Caliber, Manufacturer, ManufactureDate, Deceleration, Warhead, Price) VALUES (@name, @penetration, @weight, @velocity, @caliber, @manufacturer, @manufactureDate, @deceleration, @warhead, @price)";
                //        cmd.Parameters.Add("@name", DbType.String);
                //        cmd.Parameters.Add("@penetration", DbType.Int32);
                //        cmd.Parameters.Add("@weight", DbType.Decimal);
                //        cmd.Parameters.Add("@velocity", DbType.Single);
                //        cmd.Parameters.Add("@caliber", DbType.Int32);
                //        cmd.Parameters.Add("@manufacturer", DbType.String);
                //        cmd.Parameters.Add("@manufactureDate", DbType.DateTime);
                //        cmd.Parameters.Add("@deceleration", DbType.Int32);
                //        cmd.Parameters.Add("@warhead", DbType.String);
                //        cmd.Parameters.Add("@price", DbType.Decimal);
                //        cmd.Prepare();
                //        for (int i = 0; i < 1000; i++)
                //        {
                //            foreach (string item in data)
                //            {
                //                string[] fields = item.Split(',');
                //                cmd.Parameters["@name"].Value = fields[0];
                //                cmd.Parameters["@penetration"].Value = int.Parse(fields[1]);
                //                cmd.Parameters["@weight"].Value = decimal.Parse(fields[2]);
                //                cmd.Parameters["@velocity"].Value = float.Parse(fields[3]);
                //                cmd.Parameters["@caliber"].Value = int.Parse(fields[4]);
                //                cmd.Parameters["@manufacturer"].Value = fields[5];
                //                cmd.Parameters["@manufactureDate"].Value = DateTime.Parse(fields[6]);
                //                cmd.Parameters["@deceleration"].Value = int.Parse(fields[7]);
                //                cmd.Parameters["@warhead"].Value = fields[8];
                //                cmd.Parameters["@price"].Value = decimal.Parse(fields[9]);
                //                cmd.ExecuteNonQuery();
                //            }
                //        }
                //        transaction.Commit();
                //    }
                //    return count;
                //}

                public static int MarkDown()
                {
                    int count = 0;


                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KotoKaze.otherSource.testmarkdown.MD")!)
                    using (StreamReader reader = new(stream))
                    {
                        string markdown = reader.ReadToEnd();

                        MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                        SKPaint paint = new() { Color = SKColors.Black, TextSize = 20 };
                        SKBitmap bitmap = new(1275, 9878);
                        SKCanvas canvas = new(bitmap);

                        using MemoryStream memoryStream = new();
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        while (stopwatch.Elapsed.TotalSeconds < time)
                        {
                            var html = Markdown.ToHtml(markdown, pipeline);
                            canvas.DrawText(html, 0, paint.TextSize, paint);
                            bitmap.Encode(memoryStream, SKEncodedImageFormat.Bmp, 100);
                            count++;
                            memoryStream.SetLength(0);
                        }
                    }
                    
                    return count;
                }
            }
            public static class Floating 
            {
                public static int SolarSystem() 
                {
                    int count = 0;
                    double dt = 86400;
                    Universe.Stellar sun = new(1.989e30, new Double3(0, 0, 0));
                    Universe.Stellar mercury = new(3.30e23, new Double3(57.91e9, 0, 0));
                    Universe.Stellar venus = new(4.87e24, new Double3(108.2e9, 0, 0));
                    Universe.Stellar earth = new(5.97e24, new Double3(149.6e9, 0, 0));
                    Universe.Stellar mars = new(6.42e23, new Double3(227.9e9, 0, 0));
                    Universe.Stellar jupiter = new(1.898e27, new Double3(778.5e9, 0, 0));
                    Universe.Stellar saturn = new(5.68e26, new Double3(1.43e12, 0, 0));
                    Universe.Stellar uranus = new(8.68e25, new Double3(2.87e12, 0, 0));
                    Universe.Stellar neptune = new(1.02e26, new Double3(4.50e12, 0, 0));
                    Universe.Stellar[] planets = [mercury, venus,earth,mars,jupiter,saturn, uranus, neptune];
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (stopwatch.Elapsed.TotalSeconds < time) 
                    {
                        for (int i = 0; i < 365; i++) 
                        {
                            Universe.GetTrackStep(sun, planets, dt);
                        }
                        count ++;
                    }
                    
                    return count;
                }
                public static int GaussianBlur()
                {
                    int count = 0;
                    int width = 256; 
                    int height = 256; 
                    double sigma = 0.3f;
                    int radius = 25 / 2;
                    double pi2 = 2 * Math.PI;
                    double[,] kernel = new double[radius * 2 + 1, radius * 2 + 1];
                    double sum = 0.0;

                    for (int i = -radius; i <= radius; i++)
                    {
                        for (int j = -radius; j <= radius; j++)
                        {
                            double x = (i * i + j * j) / (2 * sigma * sigma);
                            kernel[i + radius, j + radius] = (1.0 / (pi2 * sigma * sigma)) * Math.Exp(-x);
                            sum += kernel[i + radius, j + radius];
                        }
                    }
                    for (int i = -radius; i <= radius; i++)
                    {
                        for (int j = -radius; j <= radius; j++)
                        {
                            kernel[i + radius, j + radius] /= sum;
                        }
                    }

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (stopwatch.Elapsed.TotalSeconds < time)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                double r = 0.0, g = 0.0, b = 0.0;
                                for (int i = -radius; i <= radius; i++)
                                {
                                    for (int j = -radius; j <= radius; j++)
                                    {
                                        Color pixelColor = Color.FromArgb(128, 128, 128); 
                                        r += pixelColor.R * kernel[i + radius, j + radius];
                                        g += pixelColor.G * kernel[i + radius, j + radius];
                                        b += pixelColor.B * kernel[i + radius, j + radius];
                                    }
                                }
                            }
                        }
                        count++;
                    }
                    
                    return count;
                }

                public static int HDR()
                {
                    Stopwatch stopwatch = new();
                    stopwatch.Start();
                    Mat image1 = Mat.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("KotoKaze.image.Header.3000.jpg")!, ImreadModes.Color);
                    Mat image2 = Mat.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("KotoKaze.image.Header.50.jpg")!, ImreadModes.Color);
                    Mat image3 = Mat.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("KotoKaze.image.Header.5.jpg")!, ImreadModes.Color);
                    List<Mat> images = [image1, image2, image3];
                    float[] times = [1 / 3000.0f, 1 / 50.0f, 1 / 5.0f];
                    Mat response = new();
                    CalibrateDebevec calibrate = CalibrateDebevec.Create();
                    calibrate.Process(images, response, times);
                    Mat hdr = new();
                    MergeDebevec mergeDebevec = MergeDebevec.Create();
                    mergeDebevec.Process(images, hdr, times, response);
                    Mat ldr = new();
                    Tonemap tonemap = Tonemap.Create(2.2f);
                    tonemap.Process(hdr, ldr);
                    stopwatch.Stop();
                    int time = (int)stopwatch.Elapsed.TotalMilliseconds;
                    image1.Dispose();
                    image2.Dispose();
                    image3.Dispose();
                    response.Dispose();
                    hdr.Dispose();
                    ldr.Dispose();
                    
                    return time;
                }
            }
        }
        public static class RAM 
        {
            [DllImport("TestModule.dll", CallingConvention = CallingConvention.Cdecl)]
            public extern static int RamWriteSpeed();
            [DllImport("TestModule.dll", CallingConvention = CallingConvention.Cdecl)]
            public extern static int RamReadSpeed();
            
        }

        public static class Disk 
        {

            [DllImport("TestModule.dll", CallingConvention = CallingConvention.Cdecl)]
            public extern static int DiskWriteSpeed();
            [DllImport("TestModule.dll", CallingConvention = CallingConvention.Cdecl)]
            public extern static int DiskReadSpeed();
        }
    }
}
