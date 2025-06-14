using System.Windows.Controls;

namespace ProjectsNow.Views.PartnersViews
{
    public partial class OthersView : UserControl, IView
    {
        public OthersView()
        {
            InitializeComponent();
            DataContext = new OthersViewModel();
        }

        private void VATNumber_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 15);
        }
    }
}
