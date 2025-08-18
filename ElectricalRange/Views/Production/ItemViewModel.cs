using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Production;

using System.Collections.ObjectModel;

namespace ProjectsNow.Views.Production
{
    internal class ItemViewModel : ViewModelBase
    {
        private Reference _SelectedReference;
        public ItemViewModel(Item item, ObservableCollection<Item> items)
        {
            ItemData = item;
            ItemsData = items;
            NewData.Update(item);

            GetData();

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public Item NewData { get; set; } = new Item();
        public Item ItemData { get; private set; }
        public ObservableCollection<Item> ItemsData { get; private set; }
        public ObservableCollection<Reference> ReferencesData { get; private set; }
        public Reference SelectedReference
        {
            get => _SelectedReference;
            set
            {
                if (SetValue(ref _SelectedReference, value))
                {
                    if (SelectedReference != null)
                    {
                        NewData.Code = SelectedReference.Code;
                        NewData.Description = SelectedReference.Description;
                        NewData.Unit = SelectedReference.Unit;
                        NewData.Note = SelectedReference.Note;
                        NewData.Sort1 = SelectedReference.Sort1;
                        NewData.Sort2 = SelectedReference.Sort2;
                    }
                    else
                    {
                        NewData.Code =
                            NewData.Description =
                                 NewData.Unit =
                                    NewData.Note = null;

                        NewData.Qty = 1;
                        NewData.Sort1 =
                            NewData.Sort2 = null;
                    }
                }
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void GetData()
        {
            string query = "SELECT * FROM [Production].[References] Order By Sort1, Sort2";
            using SqlConnection connection = new(Database.ConnectionString);
            ReferencesData = new ObservableCollection<Reference>(connection.Query<Reference>(query));
        }

        private void Save()
        {
            using SqlConnection connection = new(Database.ConnectionString);

            if (NewData.Id != 0)
            {
                _ = connection.Update(NewData);
                ItemData.Update(NewData);
            }
            else
            {
                _ = connection.Insert(NewData);
                ItemsData.Add(NewData);

                Item item = ItemsData.FirstOrDefault(i => i.Id == -1);
                if (item != null)
                    ItemsData.Remove(item);
            }

            MaterialsRequest request = new()
            {
                RequestId = ItemData.RequestId.Value,
                Date = DateTime.Now,
                JobOrderId = ItemData.JobOrderId,
                PanelId = ItemData.PanelId
            };

            string query = $"SELECT * FROM [Production].[MaterialsRequests] " +
                           $"WHERE RequestId = {request.RequestId} " +
                           $"And JobOrderId = {request.JobOrderId}";
            MaterialsRequest existingRequest = connection.QueryFirstOrDefault<MaterialsRequest>(query);
            if (existingRequest == null)
                _ = connection.Insert(request);

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (NewData == null)
                return false;

            if (NewData.Code == null)
                return false;

            if (NewData.Qty <= 0)
                return false;

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

//SELECT JobOrder.JobOrdersInformation.ID, JobOrder.JobOrdersInformation.Code, JobOrder.JobOrdersInformation.CodeNumber, JobOrder.JobOrdersInformation.CodeMonth, JobOrder.JobOrdersInformation.CodeYear, 
//                  JobOrder.JobOrdersInformation.Date, JobOrder.JobOrdersInformation.QuotationID, JobOrder.JobOrdersInformation.QuotationCode, JobOrder.JobOrdersInformation.InquiryID, JobOrder.JobOrdersInformation.ProjectName, 
//                  JobOrder.JobOrdersInformation.CustomerID, JobOrder.JobOrdersInformation.CustomerName, JobOrder.JobOrdersInformation.EstimationName, JobOrder.JobOrdersInformation.VAT, 
//                  JobOrder.AcknowledgmentsAttachments.Id AS AcknowledgmentAttachmentId
//FROM     JobOrder.JobOrdersInformation LEFT OUTER JOIN
//                  JobOrder.AcknowledgmentsAttachments ON JobOrder.JobOrdersInformation.ID = JobOrder.AcknowledgmentsAttachments.JobOrderId LEFT OUTER JOIN
//                  JobOrder.Panels ON JobOrder.JobOrdersInformation.ID = JobOrder.Panels.JobOrderID
//WHERE  (JobOrder.Panels.Status = N'New') OR
//                  (JobOrder.Panels.Status = N'Designing') OR
//                  (JobOrder.Panels.Status = N'Waiting_Approval') OR
//                  (JobOrder.Panels.Status = N'Production') OR
//                  (JobOrder.Panels.Status = N'Hold') OR
//                  (JobOrder.Panels.Status IS NULL) OR
//                  (JobOrder.Panels.Status = N'QC')
//GROUP BY JobOrder.JobOrdersInformation.ID, JobOrder.JobOrdersInformation.Code, JobOrder.JobOrdersInformation.CodeNumber, JobOrder.JobOrdersInformation.CodeMonth, JobOrder.JobOrdersInformation.CodeYear,
//                  JobOrder.JobOrdersInformation.Date, JobOrder.JobOrdersInformation.QuotationID, JobOrder.JobOrdersInformation.QuotationCode, JobOrder.JobOrdersInformation.InquiryID, JobOrder.JobOrdersInformation.ProjectName,
//                  JobOrder.JobOrdersInformation.CustomerID, JobOrder.JobOrdersInformation.CustomerName, JobOrder.JobOrdersInformation.EstimationName, JobOrder.JobOrdersInformation.VAT, JobOrder.AcknowledgmentsAttachments.Id