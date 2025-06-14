using System.Collections.Generic;
using System.Windows.Controls;

namespace ProjectsNow.Printing.QuotationsStatus.CustomersReports
{
    public partial class CustomerQuotationsReport : UserControl
    {
        public List<IRow> ListData { get; set; }
        public CustomerQuotationsReport(string customer, string salesman)
        {
            InitializeComponent();
            DataContext = new { customer, salesman };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            if (ListData.Count == 0)
                return;

            foreach(IRow row in ListData)
            {
                var control = (UserControl)row;
                Grid.SetRow(control, ListData.IndexOf(row));
                Body.Children.Add(control);
            } 
        }
    }
}
