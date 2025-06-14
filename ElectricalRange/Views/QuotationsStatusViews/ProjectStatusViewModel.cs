using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.QuotationsStatus;

using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    internal class ProjectStatusViewModel : ViewModelBase
    {
        public ProjectStatusViewModel(Inquiry inquiry, Quotation quotation = null)
        {
            QuotationData = quotation;
            InquiryData = inquiry;
            newInquiry.Update(inquiry);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                InquiryData.ProjectStatus = newInquiry.ProjectStatus;
                _ = connection.Update(InquiryData);
            }

            if (QuotationData != null)
            {
                QuotationData.ProjectStatus = InquiryData.ProjectStatus;
            }

            Navigation.ClosePopup();
        }

        private bool CanSave()
        {
            if (newInquiry.ProjectStatus == null)
                return false;

            return true;
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }

        public Inquiry InquiryData { get; }
        public Quotation QuotationData { get; }
        public Inquiry newInquiry { get; } = new Inquiry();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
    }
}