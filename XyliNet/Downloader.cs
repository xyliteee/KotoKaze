namespace XyliNet
{
    public class Downloader
    {
        private readonly string Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.0.0";
        public string title;
        public readonly static int isCancle = 1;
        public readonly static int isSucessful = 0;
        public readonly static int isError = -1;
        public long fileSize = 0;
        private long _fileDateHaveAlreadyDownloaded;
        public bool isDownloading = true;
        public string errorMessage = string.Empty;
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
        }

        public async Task<int> DownloadAsync(string url, string path)
        {
            try
            {
                isDownloading = true;
                using HttpClient client = new();
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
                    if (!isDownloading)
                    {
                        throw new TaskCanceledException();
                    }
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

                return isSucessful;
            }
            catch (TaskCanceledException)
            { return isCancle; }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                return isError;
            }
        }
    }
    
}
