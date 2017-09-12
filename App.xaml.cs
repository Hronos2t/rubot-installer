namespace Installer
{
    using System;
    using System.Windows;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //var lastver = VersionHelper.GetLastVersion();
            //var curver = VersionHelper.GetCurrentVesion();
            //if (lastver < curver)
            //{
            //    Environment.Exit(0);
            //}
        }
    }
}
