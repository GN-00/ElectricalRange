using ProjectsNow.Data.Suppliers;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class SuppliersView : UserControl, IView
    {
        public SuppliersView(ObservableCollection<Supplier> suppliers = null)
        {
            InitializeComponent();
            DataContext = new SuppliersViewModel(suppliers, this);
        }
    }
}
