using static KotoKaze.Static.FileManager;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KotoKaze.Windows;

namespace KotoKaze.Dynamic
{
    public class Network
    {
        private static readonly string Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.0.0";

        public class Downloader
        {
            public string title;
            public long fileSize = 0;
            private long _fileDateHaveAlreadyDownloaded;
            public HttpClient client = new();
            public Action? action;
            public long FileDateHaveAlreadyDownloaded
            {
                get { return _fileDateHaveAlreadyDownloaded; }
                set
                {
                    _fileDateHaveAlreadyDownloaded = value;
                    action?.Invoke();
                }
            }
            public Downloader(string title) 
            {
                this.title = title;
                client = new HttpClient();
            }

            ~Downloader() 
            {
                client.Dispose();
            }

            public async Task<bool> DownloadAsync(string url, string path)
            {
                try
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(Agent);
                    using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    fileSize = response.Content.Headers.ContentLength ?? 0;

                    using Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                    var totalRead = 0L;
                    var buffer = new byte[8192];
                    var isMoreToRead = true;

                    do
                    {
                        var read = await contentStream.ReadAsync(buffer);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await fileStream.WriteAsync(buffer.AsMemory(0, read));

                            totalRead += read;
                            FileDateHaveAlreadyDownloaded = totalRead;
                        }
                    } while (isMoreToRead);

                    return true;
                }
                catch (Exception e)
                {
                    await LogManager.LogWriteAsync("ADB Download Error", e.ToString());
                    return false;
                }
            }

        }
    }
}
