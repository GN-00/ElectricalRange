using ProjectsNow.Data.References;

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ProjectsNow.Views.ReferencesViews
{
    public partial class ReferenceView : UserControl, IPopup
    {
        public ReferenceView(Reference reference, ObservableCollection<Reference> references)
        {
            InitializeComponent();
            DataContext = new ReferenceViewModel(reference, references);
        }
        private void Cost_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }

        private void Unit_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.ArrowsOnly(e);
        }
    }
}
