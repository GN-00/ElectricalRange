using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Services;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.InquiriesViews
{
    internal class AssignViewModel : ViewModelBase
    {
        private Salesman _Salesman;
        private Estimation _Estimator;
        private string _QuotationCode;
        private int _SalesmanId;
        private int _EstimationId;

        public AssignViewModel(Inquiry inquiry)
        {
            InquiryData = inquiry;

            GetData();

            EstimationId = inquiry.EstimationID;
            SalesmanId = inquiry.SalesmanID;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public Inquiry InquiryData { get; }
        public Quotation QuotationData { get; private set; }

        public string QuotationCode
        {
            get => _QuotationCode;
            set => SetValue(ref _QuotationCode, value);
        }
        public int SalesmanId
        {
            get => _SalesmanId;
            set => SetValue(ref _SalesmanId, value);
        }
        public int EstimationId
        {
            get => _EstimationId;
            set => SetValue(ref _EstimationId, value);
        }
        public Salesman Salesman
        {
            get => _Salesman;
            set => SetValue(ref _Salesman, value);
        }
        public Estimation Estimator
        {
            get => _Estimator;
            set
            {
                if (SetValue(ref _Estimator, value))
                {
                    if (QuotationCode != null)
                    {
                        QuotationCode = QuotationServices.UpdateCode(QuotationCode, Estimator.Code);
                    }
                }
            }
        }

        public ObservableCollection<Salesman> Salesmen { get; private set; }
        public ObservableCollection<Estimation> Estimators { get; private set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            if (QuotationData == null)
            {
                if (InquiryData.Status == "Quoting")
                {
                    query = $"Select * From [Quotation].[Quotations(View)] Where QuotationID = {InquiryData.QuotationID}";
                    QuotationData = connection.QueryFirstOrDefault<Quotation>(query);
                }
            }

            if (QuotationData != null)
                QuotationCode = QuotationData.QuotationCode;

            query = "Select * From [User].[Salesmen] Order By Name";
            Salesmen = new ObservableCollection<Salesman>(connection.Query<Salesman>(query));

            query = "Select * From [User].[Estimations] Order By Name";
            Estimators = new ObservableCollection<Estimation>(connection.Query<Estimation>(query));
        }

        private void Save()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int id;
                if (InquiryData != null)
                {
                    id = InquiryData.InquiryID;

                    InquiryData.SalesmanID = Salesman.Id;
                    InquiryData.SalesmanName = Salesman.Name;
                    InquiryData.SalesmanCode = Salesman.Code;

                    InquiryData.EstimationID = Estimator.Id;
                    InquiryData.EstimationName = Estimator.Name;
                    InquiryData.EstimationCode = Estimator.Code;
                }
                else //QuotationData != null
                {
                    id = QuotationData.InquiryID;

                    QuotationData.SalesmanID = Salesman.Id;

                    QuotationData.EstimationID = Estimator.Id;
                    QuotationData.EstimationName = Estimator.Name;
                    QuotationData.EstimationCode = Estimator.Code;

                    QuotationData.QuotationCode = QuotationCode;
                    _ = connection.Update(QuotationData);
                }

                string query = $"Update [Inquiry].[Inquiries] Set " +
                               $"SalesmanID = {SalesmanId}, " +
                               $"EstimationId = {EstimationId} " +
                               $"Where InquiryID = {id}";

                _ = connection.Execute(query);
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            return true;
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }
        private bool CanCancel()
        {
            return true;
        }
    }
}