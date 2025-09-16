using ProjectsNow.Data.References;

using System.Collections.ObjectModel;
using System.Windows.Controls;

using Velopack.Locators;

namespace ProjectsNow.Views.ReferencesViews
{
    public partial class ReferenceView : UserControl, IView
    {
        public ReferenceView(Reference reference, ObservableCollection<Reference> references)
        {
            InitializeComponent();
            DataContext = new ReferenceViewModel(reference, references, this);
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
