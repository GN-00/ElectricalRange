using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class AddStandardItemView : UserControl, IPopup
    {
        public AddStandardItemView(QItem item, ObservableCollection<QItem> items)
        {
            InitializeComponent();
            DataContext = new AddStandardItemViewModel(item, items);
        }

        private void QtyCost_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
