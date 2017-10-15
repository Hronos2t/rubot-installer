namespace Installer
{
    using System;
    using System.ComponentModel;
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

        public async Task<string> InstallSevenZip()
        {
            var InstalledSevenZipPath = unpacker.GetInstalled7Zip();
            // install 7zip
            if (string.IsNullOrEmpty(InstalledSevenZipPath))
            {
                return null;
            }
            try
            {
                await fileDownloader.DownloadAsync(Urls.SevenZip, "7z.exe");
            }
            catch
            {
                return "Error downloading 7zip. Please check you internet connection.";
            }

            unpacker.SilentInstall7Zip(fileDownloader.fileLocation, true);
            return null;
        }

        internal async Task<string> DownloadAndExtractRubot()
        {
            try
            {
                await fileDownloader.DownloadAsync(Urls.Rubot);
            }
            catch
            {
                return "Error downloading. Please check you internet connection.";
            }
            var extracted = unpacker.Extract(fileDownloader.fileLocation, FileHelper.GetTargetPath, false);

            return extracted ? null : $"7z.exe not found! You need manually unpack archive \"{fileDownloader.fileLocation}\" to \"{FileHelper.GetTargetPath}\". " +
                $"Or use link https://github.com/Hronos2t/rubot-binary/archive/master.zip";
        }
    }
}
