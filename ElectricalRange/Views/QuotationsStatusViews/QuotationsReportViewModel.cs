using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Customers;
using ProjectsNow.Data.Inquiries;

using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;

using static ProjectsNow.Views.PrintView;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    public class QuotationsReportViewModel : ViewModelBase
    {
        private bool _Cancel = true;
        private bool _Lost = true;
        private bool _Win = true;
        private bool _OnGoing = true;
        private bool _Hold = true;
        private bool _OnHand = true;
        private bool _Bidding = true;

        private double _Zoom;
        PageOrientation _PageOrientationData;
        private FixedDocument _DocumentData;
        private DocumentViewer _DocumentViewerData;

        private DateTime _StartDate = DateTime.Parse($"{DateTime.Today.Year}-01-01");
        private DateTime _EndDate = DateTime.Today;

        private Customer _Customer;
        private Estimation _Estimator;
        private Salesman _Salesman;

        public QuotationsReportViewModel(DocumentViewer documentViewer)
        {
            string query;
            using (SqlConnection connection = new(Data.Database.ConnectionString))
            {
                query = "Select * From [User].[Estimations] Order By Name";
                Estimators = new ObservableCollection<Estimation>(connection.Query<Estimation>(query));

                query = "Select * From [User].[Salesmen] Order By Name";
                Salesmen = new ObservableCollection<Salesman>(connection.Query<Salesman>(query));


                query = "Select * From [Customer].[Customers(View)] Order By CustomerName";
                Customers = new ObservableCollection<Customer>(connection.Query<Customer>(query));
            }

            ReloadCommand = new RelayCommand(Reload, CanAccessReload);
            ReportCommand = new RelayCommand(Report, CanAccessReport);
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            RemoveCustomerCommand = new RelayCommand(RemoveCustomer, CanAccessRemoveCustomer);
            RemoveEstimatorCommand = new RelayCommand(RemoveEstimator, CanAccessRemoveEstimator);
            RemoveSalesmanCommand = new RelayCommand(RemoveSalesman, CanAccessRemoveSalesman);
        }

        public bool Cancel
        {
            get => _Cancel;
            set => SetValue(ref _Cancel, value);
        }
        public bool Lost
        {
            get => _Lost;
            set => SetValue(ref _Lost, value);
        }
        public bool Win
        {
            get => _Win;
            set => SetValue(ref _Win, value);
        }
        public bool OnGoing
        {
            get => _OnGoing;
            set => SetValue(ref _OnGoing, value);
        }
        public bool Hold
        {
            get => _Hold;
            set => SetValue(ref _Hold, value);
        }
        public bool OnHand
        {
            get => _OnHand;
            set => SetValue(ref _OnHand, value);
        }
        public bool Bidding
        {
            get => _Bidding;
            set => SetValue(ref _Bidding, value);
        }

        public DateTime StartDate
        {
            get => _StartDate;
            set => SetValue(ref _StartDate, value);
        }
        public DateTime EndDate
        {
            get => _EndDate;
            set => SetValue(ref _EndDate, value);
        }

        public double Zoom
        {
            get => _Zoom;
            set => SetValue(ref _Zoom, value);
        }
        public PageOrientation PageOrientationData
        {
            get => _PageOrientationData;
            set => SetValue(ref _PageOrientationData, value);
        }
        public FixedDocument DocumentData
        {
            get => _DocumentData;
            set => SetValue(ref _DocumentData, value);
        }
        public DocumentViewer DocumentViewerData
        {
            get => _DocumentViewerData;
            set => SetValue(ref _DocumentViewerData, value);
        }

        public Customer Customer
        {
            get => _Customer;
            set => SetValue(ref _Customer, value);
        }
        public Estimation Estimator
        {
            get => _Estimator;
            set => SetValue(ref _Estimator, value);
        }
        public Salesman Salesman
        {
            get => _Salesman;
            set => SetValue(ref _Salesman, value);
        }

        public ObservableCollection<Customer> Customers { get; }
        public ObservableCollection<Estimation> Estimators { get; }
        public ObservableCollection<Salesman> Salesmen { get; }
        public RelayCommand ReloadCommand { get; }
        public RelayCommand ReportCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand RemoveCustomerCommand { get; }
        public RelayCommand RemoveEstimatorCommand { get; }
        public RelayCommand RemoveSalesmanCommand { get; }

        private void Reload()
        {
            if (StartDate.Date == EndDate.Date)
            {
                MessageView.Show("Report", "Please select different dates!",
                                            MessageViewButton.OK,
                                            MessageViewImage.Information);

                return;
            }

            if (Customer != null)
            {
                Zoom = 80;
                PageOrientationData = PageOrientation.Portrait;
                DocumentData = Services.QuotationsStatusServices.PrintCustomerStatus(this);
            }
            else
            {
                Zoom = 60;
                PageOrientationData = PageOrientation.Landscape;
                DocumentData = Services.QuotationsStatusServices.Print(this);
            }
        }

        private bool CanAccessReload()
        {
            if (!Win & !Lost & !Hold & !Cancel & !OnGoing)
                return false;

            if (!Bidding & !OnHand)
                return false;

            return true;
        }

        private void Report()
        {
            PrintInfo printInfo = new()
            {
                DocumentName = "Quotations Report",
                DocumentViewer = DocumentViewerData,
                PageOrientation = PageOrientationData,
                FixedDocument = DocumentData,
            };
            PrintView.Print_Document(printInfo);
        }

        private bool CanAccessReport()
        {
            if (DocumentData == null)
                return false;

            return true;
        }



        private void Export()
        {
            if (StartDate.Date == EndDate.Date)
            {
                MessageView.Show("Report","Please select different dates!",
                                           MessageViewButton.OK,
                                           MessageViewImage.Information);
                return;
            }

            Services.QuotationsStatusServices.Export(this);
        }

        private bool CanAccessExport()
        {
            if (!Win & !Lost & !Hold & !Cancel & !OnGoing)
                return false;

            if (!Bidding & !OnHand)
                return false;

            return true;
        }

        private void RemoveCustomer()
        {
            Customer = null;
        }

        private bool CanAccessRemoveCustomer()
        {
            if (Customer == null)
                return false;

            return true;
        }

        private void RemoveEstimator()
        {
            Estimator = null;
        }

        private bool CanAccessRemoveEstimator()
        {
            if (Estimator == null)
                return false;

            return true;
        }

        private void RemoveSalesman()
        {
            Salesman = null;
        }

        private bool CanAccessRemoveSalesman()
        {
            if (Salesman == null)
                return false;

            return true;
        }
    }
}