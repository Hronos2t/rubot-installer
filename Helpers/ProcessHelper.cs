namespace Installer
{
    using System;
    using System.Diagnostics;
    using System.IO;

    static class ProcessHelper
    {
        public static bool ExecHidden(string fileName, string args = "")
        {
            return Exec(fileName, ProcessWindowStyle.Hidden, args, true, false);
        }

        public static bool ExecHiddenRunAs(string fileName, string args = "")
        {
            return Exec(fileName, ProcessWindowStyle.Hidden, args, true, true);
        }

        public static bool Exec(string fileName, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal, string args = "", bool waitForExit = false, bool runAs = false)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = windowStyle,
                    FileName = fileName,
                    WorkingDirectory = Path.GetDirectoryName(fileName),
                    Arguments = args,
                    Verb = runAs ? "runas" : ""
                }
            };
            try
            {
                process.Start();
                if (waitForExit)
                {
                    process.WaitForExit(10000);
                }
                process.Close();
                process?.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
