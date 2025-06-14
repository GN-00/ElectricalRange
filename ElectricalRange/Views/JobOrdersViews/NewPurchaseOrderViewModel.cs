using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;

using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.Views.JobOrdersViews
{
    internal class NewPurchaseOrderViewModel : ViewModelBase
    {
        public NewPurchaseOrderViewModel(Quotation quotation, ObservableCollection<Quotation> quotations, IPopup popup)
        {
            PopUpData = popup;
            QuotationData = quotation;
            QuotationsData = quotations;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);

            AttachCommand = new RelayCommand<PurchaseOrder>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<PurchaseOrder>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<PurchaseOrder>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<PurchaseOrder>(ReadAttachment, CanAccessReadAttachment);
        }

        public PurchaseOrderAttachment Attachment { get; set; } = new PurchaseOrderAttachment();
        public User UserData => Navigation.UserData;
        public PurchaseOrder NewData { get; } = new PurchaseOrder();
        public IPopup PopUpData { get; }
        public Quotation QuotationData { get; }
        public ObservableCollection<Quotation> QuotationsData { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public RelayCommand<PurchaseOrder> AttachCommand { get; }
        public RelayCommand<PurchaseOrder> DeleteAttachmentCommand { get; }
        public RelayCommand<PurchaseOrder> DownloadAttachmentCommand { get; }
        public RelayCommand<PurchaseOrder> ReadAttachmentCommand { get; }

        private async void Save()
        {
            Navigation.ClosePopup();
            Navigation.OpenLoading(Visibility.Visible, "Loading....");
            await Adding();
            Navigation.CloseLoading();
        }
        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(NewData.Number))
                return false;

            return true;
        }

        public async Task Adding()
        {
            //IsLoading = true;
            //Events.ShowEvent.Do();

            //if (purchaseOrder.AttachmentId == null)
            //{
            //    PurchaseOrderAttachment attachment = new PurchaseOrderAttachment()
            //    {
            //        PurchaseOrderNumber = purchaseOrder.Number,
            //    };

            //    Attachment.SaveFile<PurchaseOrderAttachment>(attachment);

            //    purchaseOrder.AttachmentId = attachment.Id;
            //}
            //else
            //{
            //    PurchaseOrderAttachment attachment = new PurchaseOrderAttachment()
            //    {
            //        Id = purchaseOrder.AttachmentId.GetValueOrDefault(),
            //    };

            //    Attachment.UpdateFile<PurchaseOrderAttachment>(attachment);
            //}

            //IsLoading = false;
            JobOrder jobOrder;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                jobOrder = new JobOrder()
                {
                    QuotationID = QuotationData.QuotationID,
                    Date = DateTime.Now,
                    CodeNumber = JobOrderController.GetCodeNumber(connection),
                    CodeMonth = DateTime.Now.Month,
                    CodeYear = DateTime.Now.Year,
                };

                if (jobOrder.CodeNumber != 0)
                {
                    jobOrder.Code = $"{(jobOrder.CodeYear - Database.CompanyCreationYear) * 1000 + jobOrder.CodeNumber}/{jobOrder.CodeMonth:00}/{jobOrder.CodeYear.ToString().Substring(2, 2)}";
                }

                _ = connection.Insert(jobOrder);

                string query = $"Select * From [JobOrder].[JobOrdersInformation] " +
                               $"Where ID = {jobOrder.ID} ";
                jobOrder = connection.QueryFirstOrDefault<JobOrder>(query);

                Acknowledgment acknowledgementData = new() { JobOrderID = jobOrder.ID };
                _ = connection.Insert(acknowledgementData);

                QuotationsData.Remove(QuotationData);

                NewData.QuotationID = jobOrder.QuotationID;
                NewData.JobOrderID = jobOrder.ID;
                NewData.JobOrderCode = jobOrder.Code;
                NewData.Customer = jobOrder.CustomerName;

                _ = connection.Insert(NewData);
                Attachment.PurchaseOrderId = NewData.ID;

                _ = connection.Insert(Attachment);
                NewData.AttachmentId = Attachment.Id;

                query = $"Select * From [Quotation].[Panels(View)]" +
                        $"WHERE QuotationID = {QuotationData.Id} And PurchaseOrdersNumber Is Null " +
                        $"Order By PanelSN";
                ObservableCollection<QPanel> quotationPanels = new(connection.Query<QPanel>(query));

                foreach (QPanel panelData in quotationPanels)
                {
                    panelData.PurchaseOrdersID = NewData.ID;
                    JPanelDetails newPanelData = new() { JobOrderID = jobOrder.ID };
                    newPanelData.Update(panelData);

                    _ = connection.Update(panelData);

                    query = $"Select DrawingNo From [JobOrder].[DrawingNo] Where Year = {DateTime.Now.Year}";
                    int? drawingNo = connection.QueryFirstOrDefault<int?>(query);
                    if (drawingNo == null)
                    {
                        newPanelData.DrawingNo = 1;
                    }
                    else
                    {
                        newPanelData.DrawingNo = ++drawingNo;
                    }

                    query = Database.InsertRecordWithID<JPanelDetails>();
                    connection.Execute(query, newPanelData);

                    connection.InsertSelect<JItem, QItem>($"Where PanelID = {newPanelData.PanelID} ");
                }
            }

            await Task.Delay(5000);
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }
        private bool CanCancel()
        {
            return true;
        }


        private void Attach(PurchaseOrder purchaseOrder)
        {
            Window.GetWindow((UserControl)PopUpData).WindowState = WindowState.Minimized;

            Data.Attachment.GetFileReady<PurchaseOrderAttachment>(Attachment);
            purchaseOrder.AttachmentId = 0;

            Window.GetWindow((UserControl)PopUpData).WindowState = WindowState.Normal;
        }

        private bool CanAccessAttach(PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(PurchaseOrder purchaseOrder)
        {
            Attachment.Data = null;
            purchaseOrder.AttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder == null)
                return false;

            if (purchaseOrder.AttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(PurchaseOrder purchaseOrder)
        {
            Window.GetWindow((UserControl)PopUpData).WindowState = WindowState.Minimized;

            Data.Attachment.DownloadFile<PurchaseOrderAttachment>(Attachment);

            Window.GetWindow((UserControl)PopUpData).WindowState = WindowState.Normal;
        }

        private bool CanAccessDownloadAttachment(PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder == null)
                return false;

            if (purchaseOrder.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(PurchaseOrder purchaseOrder)
        {
            Window.GetWindow((UserControl)PopUpData).WindowState = WindowState.Minimized;
            Data.Attachment.OpenFile<PurchaseOrderAttachment>(Attachment);
        }

        private bool CanAccessReadAttachment(PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder == null)
                return false;

            if (purchaseOrder.AttachmentId == null)
                return false;

            return true;
        }
    }
}