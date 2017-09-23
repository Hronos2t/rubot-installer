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

        private static string defaultExeName = "firefox.exe";

        private static string defaultSubDir = "RuBot";

        public volatile string fileLocation;

        public event AsyncCompletedEventHandler CountdownCompleted;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        public static string GetStartingPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        public static string GetTargetPath
        {
            get
            {
                var installed = GetInstalledBotExe;
                return string.IsNullOrEmpty(installed)
                    ? Path.Combine(GetStartingPath, defaultSubDir)
                    : Path.GetDirectoryName(installed);
            }
        }

        public static string GetInstalledBotExe
        {
            get
            {
                var startingPath = GetStartingPath;
                var apps = Directory.GetFiles(startingPath, "*.exe", SearchOption.TopDirectoryOnly)
                        .Where(a => a != Environment.GetCommandLineArgs()[0]).Select(a => Path.GetFileName(a));

                // check from args
                var args = Environment.GetCommandLineArgs();
                var exeFromArg = args.Length > 3 ? args[3] : null;
                if (!string.IsNullOrEmpty(exeFromArg) && apps.Any())
                {
                    return Path.Combine(startingPath, apps.First(a => a == exeFromArg));
                }

                // check default SubDir
                var subDirPath = Path.Combine(startingPath, defaultSubDir);
                if (exeFromArg == null && Directory.Exists(subDirPath))
                {
                    var appsSubDir = Directory.GetFiles(subDirPath, "*.exe", SearchOption.TopDirectoryOnly);
                    // only one
                    if (appsSubDir.Count() == 1)
                        return appsSubDir.First();
                }

                // check default
                var defaultApp = apps.FirstOrDefault(a => a == defaultExeName);
                if (!string.IsNullOrEmpty(defaultApp)) {
                    return Path.Combine(startingPath, defaultApp);
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
            client?.Dispose();
        }
    }
}
