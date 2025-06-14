using System.Globalization;
using System.Windows;
using ProjectsNow.Views;
using Velopack;

namespace ElectricalRange
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            VelopackApp.Build().Run();

            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            //Process myProcess = Process.GetCurrentProcess();
            //int count = Process.GetProcesses().Where(pcProcess =>
            //    pcProcess.ProcessName == myProcess.ProcessName).Count();

            //if (count > 1)
            //{
            //    _ = MessageBox.Show("Application is running...");
            //    App.Current.Shutdown();
            //}

            //Auto Updater
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Navigation.ResetAccessKeys();
        }
    }

}
