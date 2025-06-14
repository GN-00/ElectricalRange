using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Printing.JobOrderPages
{
    public partial class QuotationRequestPage : UserControl
    {
        public JobOrder JobOrderData { get; set; }
        public QuotationRequest RequestData { get; set; }
        public ObservableCollection<QuotationRequestItem> ItemsData { get; set; }

        public double Page { get; set; }
        public double Pages { get; set; }

        public QuotationRequestPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= UserControl_Loaded;

            DataContext = new { JobOrderData, RequestData, ItemsData, Page, Pages };
        }
    }
}
