using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Printing
{
    public partial class ModificationForm : UserControl
    {
        public int PanelID { get; set; }
        public ModificationItem ItemData { get; set; }
        public Actions ActionData { get; set; }
        public Modification ModificationData { get; set; }
        public ObservableCollection<ModificationItem> ItemsData { get; set; }

        public ModificationForm()
        {
            InitializeComponent();
        }
    }
}
