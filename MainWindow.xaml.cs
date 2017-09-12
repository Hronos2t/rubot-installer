namespace Installer
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;

    public partial class MainWindow : Window
    {
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

            if (lastver > currentVerions)
            {
                ButtonRun.Visibility = Visibility.Hidden;
            }
            else
            {
                ButtonRun.Visibility = Visibility.Visible;
            }
        }

        private async void ButtonUpdateClick(object sender, RoutedEventArgs e)
        {
            ButtonUpdate.IsEnabled = false;
            var unpacker = new Unpacker();
            string fileLocation;
            using (var fileDownloader = new FileHelper()
            {
                DownloadInTempDir = true
            })
            {
                fileDownloader.CountdownCompleted += new AsyncCompletedEventHandler(Completed);
                fileDownloader.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
                Progress.Visibility = Visibility.Visible;
                
                var InstalledSevenZipPath = unpacker.GetInstalled7Zip();
                // install 7zip
                if (string.IsNullOrEmpty(InstalledSevenZipPath))
                {
                    try
                    {
                        await fileDownloader.DownloadAsync(Urls.SevenZip, "7z.exe");
                    }
                    catch
                    {
                        ProgressText.Text = "Error downloading 7zip. Please check you internet connection.";
                        return;
                    }
                    unpacker.SilentInstall7Zip(fileDownloader.fileLocation, true);
                }

                ProgressText.Text = "Prepare system.";
                var defender = new WindowsDefenderHelper();
                defender.DisableRealTimeProtection();
                defender.DisableDefenderWin7();

                try
                {
                    await fileDownloader.DownloadAsync(Urls.Rubot);
                }
                catch
                {
                    ProgressText.Text = "Error downloading. Please check you internet connection.";
                    return;
                }
                fileLocation = fileDownloader.fileLocation;
            }

            ProgressText.Text = "Extracting...";

            var extractedLocation = unpacker.Extract(fileLocation, FileHelper.GetStartingPath, false);
            if (extractedLocation)
            {
                ProgressText.Text = "Updating successful!";
                ButtonRun.Visibility = Visibility.Visible;
            }
            else
            {
                ProgressText.Text = "7z.exe not found! You need unpack archive manually.";
                ButtonUpdate.IsEnabled = true;
            }
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
                ProcessHelper.Exec(FileHelper.GetInstalledBotExe);
            }
            Environment.Exit(0);
        }
    }
}
