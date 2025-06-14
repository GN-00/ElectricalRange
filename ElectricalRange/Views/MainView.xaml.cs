using System.Net;
using System.Reflection;
using System.Windows;
using ProjectsNow.Data.Application;
using ProjectsNow.Views.UserViews;
using Velopack;
using Velopack.Sources;

namespace ProjectsNow.Views
{
    public partial class MainView : Window
    {
        private static MainViewModel mainViewModel;

        public MainView()
        {
            InitializeComponent();
            mainViewModel = new MainViewModel(popup);
            DataContext = Navigation.MainPage = mainViewModel;
        }

        LoginView loginView;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string assembly = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            AppData.Version = $"V: {assembly.Substring(0, assembly.Length - 2)}";

            loginView = new();
            mainViewModel.CurrentView = loginView;
            await Update();
        }

        private async Task Update()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            try
            {
                GithubSource githubSource = new(@"https://github.com/GN-00/ElectricalRange.Release", null, false);
                var mgr = new UpdateManager(githubSource);

                // check for new version
                var newVersion = await mgr.CheckForUpdatesAsync();
                if (newVersion == null)
                {
                    loginView.LoginViewModel.VersionInfo = "App is up to date.";
                    return; // no update available
                }

                // download new version
                loginView.LoginViewModel.VersionInfo = "Download new version.";
                await mgr.DownloadUpdatesAsync(newVersion);

                // install new version and restart app
                loginView.LoginViewModel.VersionInfo = "Install new version and restart app.";
                mgr.ApplyUpdatesAndRestart(newVersion);

            }
            catch
            {
                loginView.LoginViewModel.VersionInfo = "App is up to date.";
                return;
            }
        }
    }
}
