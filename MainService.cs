namespace Installer
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public class MainService: IDisposable
    {
        private Unpacker unpacker;
        private FileHelper fileDownloader;

        public MainService()
        {
            unpacker = new Unpacker();
            fileDownloader = new FileHelper()
            {
                DownloadInTempDir = true
            };
        }

        public void Dispose()
        {
            fileDownloader?.Dispose();
        }

        public event AsyncCompletedEventHandler CountdownCompleted
        {
            add
            {
                fileDownloader.CountdownCompleted += value;
            }
            remove
            {
                fileDownloader.CountdownCompleted -= value;
            }
        }

        public event DownloadProgressChangedEventHandler DownloadProgressChanged
        {
            add
            {
                fileDownloader.DownloadProgressChanged += value;
            }
            remove
            {
                fileDownloader.DownloadProgressChanged -= value;
            }
        }

        public async Task<ErrorMessage> InstallSevenZip()
        {
            var InstalledSevenZipPath = unpacker.GetInstalled7Zip();

            // if allready installed
            if (!string.IsNullOrEmpty(InstalledSevenZipPath))
                return null;

            // install 7zip
            try
            {
                await fileDownloader.DownloadAsync(Urls.SevenZip, "7z.exe");
            }
            catch
            {
                return new ErrorMessage("Error downloading 7zip.", "Please check you internet connection.");
            }

            unpacker.SilentInstall7Zip(fileDownloader.fileLocation, true);
            return null;
        }

        internal async Task<ErrorMessage> DownloadAndExtractRubot()
        {
            try
            {
                await fileDownloader.DownloadAsync(Urls.Rubot);
            }
            catch
            {
                return new ErrorMessage("Error downloading.", "Please check you internet connection.");
            }
            var extracted = unpacker.Extract(fileDownloader.fileLocation, FileHelper.GetTargetPath, false);

            return extracted
                ? null
                : new ErrorMessage("$7z.exe not found!",
                    $"You need manually unpack archive {fileDownloader.fileLocation} to {FileHelper.GetTargetPath}.{Environment.NewLine}" +
                    $"Or use link {Path.Combine(Urls.RubotGithub, "archive/master.zip")}");
        }
    }
}
