namespace Installer
{
    using System.Diagnostics;

    static class ProcessHelper
    {
        public static void ExecHidden(string fileName, string args = "")
        {
            Exec(fileName, ProcessWindowStyle.Hidden, args, true, false);
        }

        public static void ExecHiddenRunAs(string fileName, string args = "")
        {
            Exec(fileName, ProcessWindowStyle.Hidden, args, true, true);
        }

        public static void Exec(string fileName, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal, string args = "", bool waitForExit = false, bool runAs = false)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = windowStyle,
                    FileName = fileName,
                    Arguments = args,
                    Verb = runAs ? "runas" : ""
                }
            };
            process.Start();
            if (waitForExit)
            {
                process.WaitForExit(10000);
            }
            process.Close();
            process.Dispose();
        }
    }
}
