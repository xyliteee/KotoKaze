using static KotoKaze.Static.FileManager;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KotoKaze.Static
{
    public static class Network
    {
        private static readonly string Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.0.0";

        public static async Task<bool> Download(string url, string path)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.UserAgent.ParseAdd(Agent);
                using HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using FileStream fs = new(path, FileMode.Create);
                await response.Content.CopyToAsync(fs);
                return true;
            }
            catch(Exception e)
            {
                await LogManager.LogWriteAsync("ADB Download Error",e.ToString());
                return false;
            }
        }

    }
}
