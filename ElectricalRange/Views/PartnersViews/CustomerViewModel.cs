using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Customers;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace ProjectsNow.Views.PartnersViews
{
    public class CustomerViewModel : ViewModelBase
    {
        public CustomerViewModel(Customer customer, ObservableCollection<Customer> customers, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            NewData = new Customer();
            NewData.Update(customer);
            CustomerData = customer;
            CustomersData = customers;

            GetData();

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            ClosingCommand = new RelayCommand(Closing, CanClosing);
            AttachmentsCommand = new RelayCommand<string>(Attachments, CanAccessAttachments);
        }


        public User UserData { get; }
        public Customer NewData { get; private set; }
        public Customer CustomerData { get; private set; }
        public ObservableCollection<Customer> CustomersData { get; private set; }
        public ObservableCollection<Salesman> SalesmenData { get; private set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ClosingCommand { get; }
        public RelayCommand<string> AttachmentsCommand { get; }

        public void GetData()
        {
            using SqlConnection connection = new(Data.Database.ConnectionString);
            SalesmenData = SalesmanController.GetSalesmen(connection);
        }

        private void Save()
        {
            bool isReady = true;
            string message = "Please Enter:";
            if (string.IsNullOrWhiteSpace(NewData.CustomerName)) { message += $"\n  Name."; isReady = false; }
            else if (CustomersData.Any(i => i.CustomerName?.ToLower() == NewData.CustomerName.ToLower() && i.CustomerID != NewData.CustomerID))
            { message += $"\n  Name is already exist."; isReady = false; }

            if (string.IsNullOrWhiteSpace(NewData.VATNumber)) { message += $"\n  VAT Number."; isReady = false; }
            else if (!NewData.VATNumber.Length.Equals(15)) { message += $"\n  VAT Number Must Be 15 Digit."; isReady = false; }

            if (string.IsNullOrWhiteSpace(NewData.CR)) { message += $"\n  CR."; isReady = false; }
            else if (CustomersData.Any(i => i.CR == NewData.CR && i.CustomerID != NewData.CustomerID))
            { message += $"\n  CR is already exist."; isReady = false; }
            else if (!NewData.CR.Length.Equals(10)) { message += $"\n  CR Number Must Be 10 Digit."; isReady = false; }


            if (isReady)
            {
                if (NewData.CustomerID == 0)
                {
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {
                        _ = connection.Insert(NewData);

                        if (NewData.CRAttachmentId != null ||
                            NewData.VATAttachmentId != null ||
                            NewData.AddressAttachmentId != null)
                        {
                            string query = $"Update [Customer].[CustomersAttachments] " +
                                           $"Set CustomerId = {NewData.Id} " +
                                           $"Where CustomerId = 0";

                            _ = connection.Execute(query);
                        }
                    }

                    CustomersData.Add(NewData);
                }
                else
                {
                    using (SqlConnection connection = new(Data.Database.ConnectionString))
                    {
                        _ = connection.Update(NewData);
                    }

                    CustomerData.Update(NewData);
                }

                Navigation.Back();

            }
            else
            {
                _ = MessageWindow.Show("Saving", message, MessageWindowButton.OK, MessageWindowImage.Information);
            }
        }
        private bool CanSave()
        {
            return true;
        }
        private void Cancel()
        {
            Navigation.Back();
        }
        private bool CanCancel()
        {
            return true;
        }

        private void Closing()
        {
            using (SqlConnection connection = new(Data.Database.ConnectionString))
            {
                if (NewData.CRAttachmentId != null ||
                    NewData.VATAttachmentId != null ||
                    NewData.AddressAttachmentId != null)
                {
                    string query = $"Delete [Customer].[CustomersAttachments] " +
                                   $"Where CustomerId = 0";

                    _ = connection.Execute(query);
                }
            }

            UserData.Exist(CustomerData);
        }
        private bool CanClosing()
        {
            return true;
        }

        private void Attachments(string type)
        {
            Navigation.OpenPopup(new AttachmentsView(NewData, type), PlacementMode.MousePoint, false);
        }

        private bool CanAccessAttachments(string type)
        {
            return true;
        }
    }
}