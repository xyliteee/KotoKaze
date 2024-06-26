﻿using KotoKaze.Dynamic;
using KotoKaze.Static;
using KotoKaze.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using static FileControl.FileManager;
using static KotoKaze.Dynamic.BackgroundTaskList;
using XyliNet;
using static XyliNet.Downloader;
using System.Diagnostics;

namespace XyliteeeMainForm.Views
{
    /// <summary>
    /// settingPage.xaml 的交互逻辑
    /// </summary>
    public partial class settingPage : Page
    {
        public NetworkBackgroundTask? ADBINSTALL;
        public settingPage()
        {
            InitializeComponent();
            SetAnimationLevelDefault();
        }

        private void ChangeStartWindowWallpaperButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string[] fileType = ["png", "jpeg", "jpg", "bmp"];
            string filter = string.Join(";", fileType.Select(x => "*." + x));
            string filterString = $"图片文件 ({filter})|{filter}";
            OpenFileDialog openFileDialog = new()
            {
                Filter = filterString
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string sourceFilePath = openFileDialog.FileName;
                string destFilePath = Path.Combine(WorkDirectory.BinDirectory, "StartWallpaper_temp.png"); // 目标文件路径
                File.Copy(sourceFilePath, destFilePath, true);
                KotoMessageBoxSingle.ShowDialog("已成功复制壁纸文件");
            }
        }
        private async void ReinstallADBButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string adbPath = Path.Combine(WorkDirectory.BinDirectory, "platform-tools\\adb.exe");
            if (IsTaskRunning(GlobalData.ToolsPageInstance.otherPage.ADBINSTALL) || IsTaskRunning(ADBINSTALL)) //检查当前任务列表是否包含ADB安装任务
            {
                KotoMessageBoxSingle.ShowDialog("正在下载ADB组件，耐心等待......");
                return;
            }

            var rr = KotoMessageBox.ShowDialog("是否重新安装ADB组件");
            if (!rr.IsYes) { return; }
            ADBINSTALL = new(new Downloader("ADB Download")) { Title = "ADB组件下载" };
            ADBINSTALL.Start();
            string adbZipFile = Path.Combine(WorkDirectory.softwareTempDirectory, "adb.zip");//ADB压缩包的下载路径
            int isDownloadSuccessful;
            for (int times = 0; times < 5; times++)
            {
                if (times != 0)
                {
                    ADBINSTALL.Description = $"下载出错，第{times}次重试";
                    await Task.Delay(1000);
                }
                string ADBINSTALLUrl = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip";
                isDownloadSuccessful = await ADBINSTALL.downloader.DownloadAsync(ADBINSTALLUrl, adbZipFile);
                if (isDownloadSuccessful == isSucessful) break;
                if (isDownloadSuccessful == isCancle)  return;
                if (times == 4 && isDownloadSuccessful == isError) 
                {
                    ADBINSTALL.SetFinished(() => { KotoMessageBoxSingle.ShowDialog("下载出错，建议检查网络状况"); });
                    await LogManager.LogWriteAsync($"{ADBINSTALL.Title} Error", ADBINSTALL.downloader.errorMessage);
                    return;
                } 
            }
            (bool isSucessfulShutdown,string message)  = await ProcessControl.ProcessShutdownAsync("adb");
            if (!isSucessfulShutdown) 
            {
                KotoMessageBoxSingle.ShowDialog("出现错误，已保存日志");
                await LogManager.LogWriteAsync("Process Shutdown Error", message, "手动中止adb进程并删除文件");
                return;
            }
            while (GlobalData.IsRunning) 
            {
                if (!Path.Exists(Path.Combine(WorkDirectory.BinDirectory, "platform-tools"))) 
                {
                    break;
                }
                try
                {
                    Directory.Delete(Path.Combine(WorkDirectory.BinDirectory, "platform-tools"), true);
                    break;
                }
                catch (Exception) { }
            }
            ADBINSTALL.Description = "正在解压....";
            bool isSuccessful = await UnzipAsync(adbZipFile, WorkDirectory.BinDirectory, "ADB UnZip");
            if (!isSuccessful || !Path.Exists(adbPath))
            {
                ADBINSTALL.SetFinished(() => { KotoMessageBoxSingle.ShowDialog("解压错误，已生成日志文件"); });
                return;
            }
            ADBINSTALL.SetFinished(() => { KotoMessageBoxSingle.ShowDialog("ADB组件安装完成"); });
        }
        private void ShutdownAllTaskButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalData.TasksList.Count == 0) 
            {
                KotoMessageBoxSingle.ShowDialog("没有后台任务正在执行");
                return;
            }
            while (GlobalData.TasksList.Count != 0) 
            {
                GlobalData.TasksList[0].Shutdown(false);
            }
            KotoMessageBoxSingle.ShowDialog("已终止所有任务");
        }

        private void AnimationLevelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = ((ComboBox)sender).SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    GlobalData.AnimationLevel = 0;
                    break;
                case 1:
                    GlobalData.AnimationLevel = 1;
                    break;
                case 2:
                    GlobalData.AnimationLevel = 2;
                    break;
            }
            IniManager.IniFileWrite("Application.ini", "SETTING", "ANIMATION_LEVEL", GlobalData.AnimationLevel.ToString());
        }
        private void SetAnimationLevelDefault()
        {
            if (GlobalData.AnimationLevel == 0) AnimationLevelList.SelectedIndex = 0;
            else if (GlobalData.AnimationLevel == 1) AnimationLevelList.SelectedIndex = 1;
            else if (GlobalData.AnimationLevel == 2) AnimationLevelList.SelectedIndex = 2;
        }

        private void ImportCleanRulesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CleanRuleEditWindow.ShowSettingPage();
        }
    }
}
