using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;

using System.Windows;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class PanelProgressionWindow : Window
    {
        public User UserData { get; set; }
        public JPanel PanelData { get; set; }

        public PanelProgressionWindow()
        {
            InitializeComponent();
        }
    }
}
