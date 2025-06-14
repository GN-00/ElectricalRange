using System.Windows.Controls;

using static ProjectsNow.Services.QuotationsStatusServices;

namespace ProjectsNow.Printing.QuotationsStatus.CustomersReports
{
    public partial class Record_Lossed : UserControl, IRow
    {
        public Record_Lossed(Quotation quotation)
        {
            InitializeComponent();
            DataContext = quotation;
        }
    }
}
