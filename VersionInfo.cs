namespace Installer
{
    using System;
    using System.Text.RegularExpressions;

    public class VersionInfo
    {
        public string Version = String.Empty;

        public DateTime Date = DateTime.MinValue;

        public string Description = String.Empty;

        public string Exe = String.Empty;

        public Version SysVer
        {
            get
            {
                var cleaned = new Regex("[^0-9.]").Replace(Version, string.Empty);
                System.Version.TryParse(cleaned, out Version sysVer);
                return sysVer ?? new Version();
            }
        }

        public bool IsEmpty => Date == DateTime.MinValue && Version == String.Empty;

        public static bool operator >(VersionInfo left, VersionInfo rigth)
        {
            return left.SysVer > rigth.SysVer;
        }

        public static bool operator <(VersionInfo left, VersionInfo rigth)
        {
            return left.SysVer > rigth.SysVer;
        }

        public static bool operator >=(VersionInfo left, VersionInfo rigth)
        {
            return left.SysVer >= rigth.SysVer;
        }

        public static bool operator <=(VersionInfo left, VersionInfo rigth)
        {
            return left.SysVer <= rigth.SysVer;
        }
    }
}