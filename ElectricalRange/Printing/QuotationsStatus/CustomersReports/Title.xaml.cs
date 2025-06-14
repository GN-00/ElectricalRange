using System.Windows.Controls;

namespace ProjectsNow.Printing.QuotationsStatus.CustomersReports
{
    public partial class Title : UserControl, IRow
    {
        public Title(string status)
        {
            InitializeComponent();
            DataContext = status;
        }
    }
}
