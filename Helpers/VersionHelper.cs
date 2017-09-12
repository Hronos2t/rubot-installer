namespace Installer
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    public class VersionHelper
    {
        private static VersionInfo lastVersionCashed = null;

        public static VersionInfo GetLastVersion()
        {
            if (lastVersionCashed != null)
            {
                return lastVersionCashed;
            }

            var versionInfo = new VersionInfo();
            var doc1 = new XmlDocument();
            doc1.Load(Urls.LastVersionInfo);
            var root = doc1.DocumentElement;
            if (root.Name != "CurrentVersion")
            {
                return versionInfo;
            }

            versionInfo.Version = root["version"]?.InnerText ?? "error";
            versionInfo.Description = (root["comment"]?.InnerText ?? "error").Trim();
            try
            {
                versionInfo.Date = DateTime.ParseExact(root["date"]?.InnerText, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                versionInfo.Date = DateTime.MinValue;
            }
            versionInfo.Exe = root["exe"]?.InnerText ?? "";

            lastVersionCashed = versionInfo;
            return versionInfo;
        }

        public static VersionInfo GetCurrentVesion()
        {
            var versionInfo = new VersionInfo()
            {
                Description = String.Empty
            };

            var args = Environment.GetCommandLineArgs();
            if (args.Length <= 1)
            {
                var fileLocation = FileHelper.GetInstalledBotExe;
                if (File.Exists(fileLocation))
                {
                    versionInfo.Version = GetFileVersion(fileLocation);
                    versionInfo.Date = File.GetLastWriteTime(fileLocation);
                }
                return versionInfo;
            }
            versionInfo.Version = args[1];
            try
            {
                versionInfo.Date = DateTime.ParseExact(args[2], "yyyy-MM-ddTHH", CultureInfo.InvariantCulture);
            }
            catch
            {
                versionInfo.Date = DateTime.MinValue;
            }
            versionInfo.Exe = args[3];

            return versionInfo;
        }

        private static string GetFileVersion(string fileLocation)
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(fileLocation).FileVersion;
            }
            catch
            {
                return "0.0.0.0";
            }
        }
    }
}
