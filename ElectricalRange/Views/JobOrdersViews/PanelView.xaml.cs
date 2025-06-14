using ProjectsNow.Data.JobOrders;

using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class PanelView : UserControl, IView
    {
        public PanelView(JobOrder order, JPanel panel)
        {
            InitializeComponent();
            DataContext = new PanelViewModel(order, panel);
        }

        private void DoubleOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
