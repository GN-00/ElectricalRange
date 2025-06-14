using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;

namespace ProjectsNow.Printing
{
    public partial class CheckList : UserControl
    {
        public JobOrder JobOrderData { get; set; }
        public JPanelDetails PanelData { get; set; }
        public CheckList()
        {
            InitializeComponent();
        }
    }
}
