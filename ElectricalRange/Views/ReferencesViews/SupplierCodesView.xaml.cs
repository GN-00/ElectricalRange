using ProjectsNow.Data.References;

using System.Windows.Controls;

namespace ProjectsNow.Views.ReferencesViews
{
    public partial class SupplierCodesView : UserControl, IView
    {
        public SupplierCodesView(Reference reference)
        {
            InitializeComponent();
            DataContext = new SupplierCodesViewModel(reference, this);
        }
    }
}
