using KotoKaze.Static;
using KotoKaze.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;

namespace XyliteeeMainForm.Views
{
    /// <summary>
    /// settingPage.xaml 的交互逻辑
    /// </summary>
    public partial class settingPage : Page
    {
        public settingPage()
        {
            InitializeComponent();
        }

        private void ChangeStartWindowWallpaperButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string sourceFilePath = openFileDialog.FileName;
                string destFilePath = Path.Combine(FileManager.WorkDirectory.BinDirectory, "StartWallpaper" + ".png"); // 目标文件路径
                File.Copy(sourceFilePath, destFilePath, true);
                KotoMessageBoxSingle.ShowDialog("已成功复制壁纸文件");
            }
        }
    }
}
