using System.Windows.Controls;

namespace ProjectsNow.Printing.QuotationsStatus.CustomersReports
{
    public partial class Total : UserControl, IRow
    {
        public Total(string status, decimal value)
        {
            InitializeComponent();
            DataContext = new { status, value };
        }
    }
}
