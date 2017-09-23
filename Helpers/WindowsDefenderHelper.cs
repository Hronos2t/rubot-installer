namespace Installer
{
    using System;
    using System.IO;

    class WindowsDefenderHelper
    {
        private bool IsWindowsSevenOrLess => Environment.OSVersion.Version < new Version(6, 2);

        private void SetProtection(bool disable)
        {
            // only on windows 8+
            if (IsWindowsSevenOrLess)
            {
                return;
            }
            var fileName = Path.GetTempPath() + "disableDefender.bat";
            File.WriteAllText(fileName, $"echo Set-MpPreference -DisableRealtimeMonitoring ${disable} | powershell.exe -noprofile -");
            ProcessHelper.ExecHiddenRunAs("cmd.exe", $"/c {fileName}");
            File.Delete(fileName);
        }

        private void SetAllProtection(bool disable)
        {
            // only on windows 8+
            if (IsWindowsSevenOrLess)
            {
                return;
            }
            var fileName = Path.GetTempPath() + "disableDefender.ps1";
            File.WriteAllText(fileName, $@"New-ItemProperty -Path ""HKLM:\SOFTWARE\Policies\Microsoft\Windows Defender"""
                + $"-Name DisableAntiSpyware -Value {(disable ? "1" : "0")} -PropertyType DWORD -Force {Environment.NewLine}"
                + $"Set-MpPreference -DisableRealtimeMonitoring ${disable}");
            ProcessHelper.ExecHiddenRunAs("powershell.exe", $"-ExecutionPolicy UnRestricted -File {fileName}");
            File.Delete(fileName);
        }

        public void DisableRealTimeProtection()
        {
            SetAllProtection(true);
        }

        public void EnableRealTimeProtection()
        {
            SetAllProtection(false);
        }

        public void DisableDefenderWin7()
        {
            // only on windows 7
            if (!IsWindowsSevenOrLess)
            {
                return;
            }
            var serviceName = "windefend";
            var fileName = Path.GetTempPath() + "disableDefenderW7.ps1";
            File.WriteAllText(fileName, $@"Set-Service {serviceName} -startupType manual {Environment.NewLine}"
                + $"Stop-Service {serviceName}");
            ProcessHelper.ExecHiddenRunAs("powershell.exe", $"-ExecutionPolicy UnRestricted -File {fileName}");
            File.Delete(fileName);
        }
    }
}
