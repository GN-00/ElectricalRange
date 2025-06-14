using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.JobOrders;

using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    internal class CommunicationViewModel : ViewModelBase
    {
        private Communication _NewData;
        public CommunicationViewModel(Communication communicationData, ObservableCollection<Communication> communicationsData = null, Window window = null)
        {
            WindowData = window;
            CommunicationData = communicationData;
            CommunicationsData = communicationsData;

            NewData = new Communication();
            NewData.Update(CommunicationData);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        public Communication CommunicationData { get; }
        public ObservableCollection<Communication> CommunicationsData { get; }
        public Communication NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }


        private void Save()
        {
            JobOrder record = null;
            if (NewData.Status == "Cancel")
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Select * From [JobOrder].[JobOrdersInformation] " +
                                   $"Where InquiryID = {NewData.InquiryID} " +
                                   $"Order By CodeYear DESC, CodeNumber DESC";
                    record = connection.QueryFirstOrDefault<JobOrder>(query);
                }

                if (record != null)
                {
                    if (record.Invoices != 0 || record.Deliveries != 0)
                    {
                        string message = $"Can't Cancel this order!" +
                                         $"\nDeliveries: {record.Deliveries}." +
                                         $"\nInvoices: {record.Invoices}.";
                        MessageView.Show("Cancelation", message, MessageViewButton.OK, MessageViewImage.Information);

                        return;
                    }
                }
            }


            if (NewData.Id == 0)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Insert(NewData);
                }

                if (CommunicationsData != null)
                {
                    CommunicationsData.Add(NewData);

                    if (record != null)
                    {
                        if (NewData.Status == "Cancel")
                        {
                            CancelJobOrder cancelJobOrder = new()
                            {
                                JobOrderId = record.ID,
                                Date = DateTime.Now,
                            };

                            using SqlConnection connection = new(Database.ConnectionString);
                            _ = connection.Insert(cancelJobOrder);
                        }
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(NewData);
                }

                if (CommunicationData != null)
                {
                    CommunicationData.Update(NewData);
                }

                if (record != null)
                {
                    if (NewData.Status == "Cancel")
                    {
                        CancelJobOrder cancelJobOrder = new()
                        {
                            JobOrderId = record.ID,
                            Date = DateTime.Now,
                        };

                        using SqlConnection connection = new(Database.ConnectionString);
                        _ = connection.Insert(cancelJobOrder);
                    }
                }
            }

            Navigation.ClosePopup();

        }

        private bool CanSave()
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(NewData.Description))
            {
                result = true;
            }
            return result;
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }
    }
}