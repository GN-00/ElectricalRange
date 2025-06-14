using System.Windows;
using System.Threading;

namespace ProjectsNow.Events
{
    public static class ShowEvent
    {
        public static void Do()
        {
            _ = Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
    }
}
