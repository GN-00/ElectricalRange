using ProjectsNow.Data.Finance;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.FinanceView
{
    public partial class JobOrderPanelsByPriceView : UserControl, IPopup
    {
        public JobOrderPanelsByPriceView(ObservableCollection<InvoiceItem> transactions, ObservableCollection<PanelPrices> panels)
        {
            InitializeComponent();
            DataContext = new JobOrderPanelsByPriceViewModel(transactions, panels);
        }

        private void Invoicing_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
