using KotoKaze.Static;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KotoKaze.Windows
{
    /// <summary>
    /// StartLoadingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartLoadingWindow : Window
    {
        public StartLoadingWindow()
        {
            InitializeComponent();
            SetWallpaper();
        }

        private async void SetWallpaper() 
        {
            string wallpaperPathTemp = System.IO.Path.Combine(FileManager.WorkDirectory.BinDirectory, "StartWallpaper_Temp.png");
            string wallpaperPath = System.IO.Path.Combine(FileManager.WorkDirectory.BinDirectory, "StartWallpaper.png");
            if (File.Exists(wallpaperPathTemp)) 
            {
                File.Delete(wallpaperPath);
                File.Move(wallpaperPathTemp, wallpaperPath);
            } 
            if (File.Exists(wallpaperPath)) 
            {
                try
                {
                    BitmapImage wallpaper = new(new Uri(wallpaperPath));
                    BackgroundImage.Source = wallpaper;
                }
                catch (Exception e)
                {
                    await FileManager.LogManager.LogWriteAsync("Wallpaper Error", e.ToString(),"检查Bin目录下的StartWallpaper.png是否为正确的图片");
                }
            }
        }
    }
}
