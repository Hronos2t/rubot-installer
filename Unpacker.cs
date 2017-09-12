namespace Installer
{
    using Microsoft.Win32;
    using System;
    using System.IO;

    class Unpacker
    {
        private string SevenZipExeName = "7z.exe";

        private bool CheckPath(string path)
        {
            return Directory.Exists(path) && File.Exists($@"{path}\{SevenZipExeName}");
        }

        private string GetRegAndCheckExitsExe(string keyName, string valueName)
        {
            try
            {
                var path = Registry.GetValue(keyName, valueName, string.Empty).ToString();
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                path = Path.GetDirectoryName(path);
                return CheckPath(path) ? $@"{path}\{SevenZipExeName}" : null;
            }
            catch
            {
                return null;
            }
        }

        private string CheckDefaultPaths() {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\7-Zip";
            if (CheckPath(path))
            {
                return $@"{path}\{SevenZipExeName}";
            }
            path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\7-Zip";
            if (CheckPath(path))
            {
                return $@"{path}\{SevenZipExeName}";
            }
            return null;
        }

        public string GetInstalled7Zip()
        {
            var regSoftKey = @"HKEY_USERS\S-1-5-21-3807274449-1507993403-17224015-1001\Software\7-Zip";
            return GetRegAndCheckExitsExe(regSoftKey, "Path")
                ?? GetRegAndCheckExitsExe(regSoftKey, "Path32")
                ?? GetRegAndCheckExitsExe(regSoftKey, "Path64")
                ?? GetRegAndCheckExitsExe(@"HKEY_CLASSES_ROOT\CLSID\{23170F69-40C1-278A-1000-000100020000}\InprocServer32", null)
                ?? CheckDefaultPaths();
        }

        public void SilentInstall7Zip(string installerLocation, bool removeInstaller)
        {
            ProcessHelper.ExecHidden(installerLocation, "/S");
            if (removeInstaller)
            {
                File.Delete(installerLocation);
            }
        }

        public bool Extract(string archive, string dest, bool removeArchive)
        {
            string path = GetInstalled7Zip();
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            ProcessHelper.ExecHidden(path, $"x \"{archive}\" -aoa -y -p123 -o\"{dest}\"");
            if (removeArchive)
            {
                File.Delete(archive);
            }
            return Directory.Exists(dest);
        }
    }
}