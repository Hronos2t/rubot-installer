namespace Installer
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    class FileHelper: IDisposable
    {
        private volatile bool _completed;

        private WebClient client;

        private string defaultArhiveName = "arhive.7z";

        public volatile string fileLocation;

        public event AsyncCompletedEventHandler CountdownCompleted;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        public static string GetStartingPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        public static string GetInstalledBotExe
        {
            get
            {
                var apps = Directory.GetFiles(GetStartingPath, "*.exe", SearchOption.TopDirectoryOnly)
                        .Where(a => a != Environment.GetCommandLineArgs()[0]).Select(a => Path.GetFileName(a));

                if (!apps.Any())
                    return null;

                // check from args
                var args = Environment.GetCommandLineArgs();
                var exeFromArg = args.Length > 3 ? args[3] : null;
                if (!string.IsNullOrEmpty(exeFromArg))
                {
                    return $@"{GetStartingPath}\{apps.FirstOrDefault(a => a == exeFromArg)}";
                }

                // only one
                if (apps.Count() == 1)
                {
                    return $@"{GetStartingPath}\{apps.Single()}";
                }

                // check default
                var defaultApp = apps.FirstOrDefault(a => a == "firefox.exe");
                if (!string.IsNullOrEmpty(defaultApp)) {
                    return $@"{GetStartingPath}\{defaultApp}";
                }

                return null;
            }
        }

        public bool DownloadInTempDir { get; set; } = false;

        public FileHelper()
        {
            client = new WebClient();
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
            fileLocation = String.Empty;
        }

        public Task DownloadAsync(string address, string fileName = null)
        {
            var Uri = new Uri(address);
            _completed = false;
            if (fileName == null)
            {
                fileName = defaultArhiveName;
            }

            fileLocation = $@"{(DownloadInTempDir ? Path.GetTempPath() : GetStartingPath)}\{fileName}";
            return client.DownloadFileTaskAsync(Uri, fileLocation);
        }

        public void CancelDownloading()
        {
            client.CancelAsync();
        }

        public bool DownloadCompleted
        {
            get
            {
                return _completed;
            }
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(sender, e);
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            CountdownCompleted?.Invoke(sender, e);
            _completed = true;
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
