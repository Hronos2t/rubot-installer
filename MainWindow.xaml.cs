namespace Installer
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;

    public partial class MainWindow : Window
    {
        private MainService service;

        public MainWindow()
        {
            InitializeComponent();
            var lastver = VersionHelper.GetLastVersion();
            var currentVerions = VersionHelper.GetCurrentVesion();
            VersionText.Text = $"{lastver.Version}  ({lastver.Date.ToLongDateString()})";
            DescriptionText.Text = lastver.Description;
            Progress.Visibility = Visibility.Hidden;
            Progress.Value = 0;
            ProgressText.Text = "";
            service = new MainService();

            ButtonRun.Visibility = lastver > currentVerions 
                ? Visibility.Hidden 
                : Visibility.Visible;
        }

        private async void ButtonUpdateClick(object sender, RoutedEventArgs e)
        {
            ButtonUpdate.IsEnabled = false;

            service.CountdownCompleted += new AsyncCompletedEventHandler(Completed);
            service.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
            Progress.Visibility = Visibility.Visible;

            // install 7zip, if need
            var error = await service.InstallSevenZip();
            if (error != null)
            {
                ShowErrorMessage(error);
                return;
            }

            ProgressText.Text = "Prepare system.";
            var defender = new WindowsDefenderHelper();
            defender.DisableRealTimeProtection();
            defender.DisableDefenderWin7();

            ProgressText.Text = "Extracting...";
            error = await service.DownloadAndExtractRubot();
            if (error != null)
            {
                ShowErrorMessage(error);
                ButtonUpdate.IsEnabled = true;
            }
            else
            {
                ProgressText.Text = "Updating successful!";
                ButtonRun.Visibility = Visibility.Visible;
            }
        }

        private void ShowErrorMessage(ErrorMessage error)
        {
            ProgressText.Text = error.Title;
            DescriptionText.Text = error.Message;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                ProgressText.Text = "Canceled.";
            }
            else
            {
                Progress.Value = 100;
            }
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressText.Text = $"Downloading... {e.BytesReceived / 1024}/{e.TotalBytesToReceive / 1024}";
            Progress.Value = e.ProgressPercentage;
        }

        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            var installedBot = FileHelper.GetInstalledBotExe;
            if (installedBot != null)
            {
                ProcessHelper.Exec(installedBot);
            }
            Environment.Exit(0);
        }
    }
}
