using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    public partial class NewPurchaseOrderView : UserControl, IPopup
    {
        public NewPurchaseOrderView(Quotation quotation, ObservableCollection<Quotation> quotations)
        {
            InitializeComponent();
            DataContext = new NewPurchaseOrderViewModel(quotation, quotations, this);
        }
    }
}
