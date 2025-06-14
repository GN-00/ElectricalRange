using System.Windows.Controls;

using static ProjectsNow.Services.QuotationsStatusServices;

namespace ProjectsNow.Printing.QuotationsStatus.CustomersReports
{
    public partial class Record : UserControl, IRow
    {
        public Record(Quotation quotation)
        {
            InitializeComponent();
            DataContext = quotation;
        }
    }
}
