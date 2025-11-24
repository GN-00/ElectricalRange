using ElectricalRange;
using Velopack;

namespace ProjectsNow
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            VelopackApp.Build().Run();
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }
}

//Auto Updater

//If vpk commands aren't working, try the following:
//dotnet tool update -g vpk


//If you want to go with publishing the app, you can use the following commands: 
//dotnet publish ElectricalRange.csproj -c Release --self-contained -r win-x64 -o .\publish
//vpk pack -u YourAppId -v 1.0.0 -p.\publish -e yourMainApp.exe --icon ".\ElectricalRange\ER.ico"


//If you want to go with Release the app, you can use the following commands: 
//vpk pack -u ERI -v 1.2.5 -p.\ElectricalRange\bin\Release\net8.0-windows -e 'Electrical Range.exe' --icon ".\ElectricalRange\logo.ico"