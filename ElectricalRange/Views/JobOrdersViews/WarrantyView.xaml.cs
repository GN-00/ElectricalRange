using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class WarrantyView : UserControl, IView
    {
        public WarrantyView(Warranty warranty, ObservableCollection<Warranty> warranties)
        {
            InitializeComponent();
            DataContext = new WarrantyViewModel(warranty, warranties, this);
        }

        private void DurationUnit_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.ArrowsOnly(e);
        }

        private void Duration_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 3);
        }
    }
}
