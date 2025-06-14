using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.QuotationsViews
{
    public partial class AddCustomItemView : UserControl, IPopup
    {
        public AddCustomItemView(QItem item, ObservableCollection<QItem> items)
        {
            InitializeComponent();
            DataContext = new AddCustomItemViewModel(item, items);
        }

        private void QtyCost_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
    }
}
