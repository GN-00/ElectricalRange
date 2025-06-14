using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Customers;
using ProjectsNow.Data.Inquiries;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.InquiriesViews
{
    internal class InquiryViewModel : ViewModelBase
    {
        private string _ContactsIndicator = "-";
        private int _SelectedContactIndex;

        private Contact _SelectedContact;
        private Contact _SelectedCustomerContact;
        private Customer _SelectedCustomer;
        private Salesman _SelectedSalesman;
        private Estimation _SelectedEstimator;
        private Consultant _SelectedConsultant;

        private ICollectionView _ContactsCollection;
        private ObservableCollection<Contact> _CustomerContacts;

        public InquiryViewModel(Inquiry inquiry, ObservableCollection<Inquiry> inquiries, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            InquiryData = inquiry;
            Inquiries = inquiries;
            NewInquiryData.Update(InquiryData);

            GetData();

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            DefaultCommand = new RelayCommand(Default, CanDefault);
            AddContactCommand = new RelayCommand<Contact>(AddContact, CanAddContact);
            AttentionCommand = new RelayCommand<Contact>(Attention, CanAttention);
            RemoveContactCommand = new RelayCommand<Contact>(RemoveContact, CanRemoveContact);
        }
        public InquiryViewModel(Inquiry inquiry, Quotation quotation, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            InquiryData = inquiry;
            QuotationData = quotation;
            NewInquiryData.Update(InquiryData);
            NewQuotationData.Update(QuotationData);

            GetData();

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            DefaultCommand = new RelayCommand(Default, CanDefault);
            AddContactCommand = new RelayCommand<Contact>(AddContact, CanAddContact);
            AttentionCommand = new RelayCommand<Contact>(Attention, CanAttention);
            RemoveContactCommand = new RelayCommand<Contact>(RemoveContact, CanRemoveContact);
        }

        public User UserData { get; }
        public Inquiry NewInquiryData { get; } = new Inquiry();
        public Quotation NewQuotationData { get; } = new Quotation();
        public Inquiry InquiryData { get; private set; }
        public Quotation QuotationData { get; private set; }
        public ObservableCollection<Inquiry> Inquiries { get; }

        public Contact SelectedContact
        {
            get => _SelectedContact;
            set => SetValue(ref _SelectedContact, value);
        }
        public Contact SelectedCustomerContact
        {
            get => _SelectedCustomerContact;
            set => SetValue(ref _SelectedCustomerContact, value);
        }
        public Customer SelectedCustomer
        {
            get => _SelectedCustomer;
            set
            {
                if (SelectedCustomer == null)
                {
                    SetValue(ref _SelectedCustomer, value);
                    GetContacts();
                }
                else
                {
                    SetValue(ref _SelectedCustomer, value);
                    GetContacts();
                    ResetProjectContacts();
                    NewInquiryData.SalesmanID = SelectedCustomer.SalesmanID;
                }

                NewInquiryData.CustomerName = SelectedCustomer.CustomerName;
                if (QuotationData != null)
                {
                    NewQuotationData.CustomerName = SelectedCustomer.CustomerName;
                }
            }
        }

        public Salesman SelectedSalesman
        {
            get => _SelectedSalesman;
            set
            {
                if (SetValue(ref _SelectedSalesman, value))
                {
                    //NewInquiryData.SalesmanID = SelectedSalesman.Id;
                    NewInquiryData.SalesmanName = SelectedSalesman.Name;
                    NewInquiryData.SalesmanCode = SelectedSalesman.Code;

                    if (QuotationData != null)
                    {
                        NewQuotationData.SalesmanID = SelectedSalesman.Id;
                    }
                }
            }
        }
        public Estimation SelectedEstimator
        {
            get => _SelectedEstimator;
            set
            {
                if (SetValue(ref _SelectedEstimator, value))
                {
                    //NewInquiryData.EstimationID = SelectedEstimator.Id;
                    NewInquiryData.EstimationName = SelectedEstimator.Name;
                    NewInquiryData.EstimationCode = SelectedEstimator.Code;

                    if (QuotationData != null)
                    {
                        NewQuotationData.EstimationID = SelectedEstimator.Id;
                        NewQuotationData.EstimationName = SelectedEstimator.Name;
                        NewQuotationData.EstimationCode = SelectedEstimator.Code;
                    }
                }
            }
        }
        public Consultant SelectedConsultant
        {
            get => _SelectedConsultant;
            set
            {
                if (SetValue(ref _SelectedConsultant, value))
                {
                    if (QuotationData != null)
                    {
                        NewQuotationData.ConsultantID = SelectedConsultant.ConsultantID;
                    }
                }
            }
        }

        public string ContactsIndicator
        {
            get => _ContactsIndicator;
            set => SetValue(ref _ContactsIndicator, value);
        }
        public int SelectedContactIndex
        {
            get => _SelectedContactIndex;
            set
            {
                if (SetValue(ref _SelectedContactIndex, value))
                {
                    UpdateIndicator();
                }
            }
        }

        public ObservableCollection<Customer> Customers { get; private set; }
        public ObservableCollection<Consultant> Consultants { get; private set; }
        public ObservableCollection<Contact> ProjectContacts { get; private set; }
        public ObservableCollection<Contact> CustomerContacts
        {
            get => _CustomerContacts;
            set => SetValue(ref _CustomerContacts, value);
        }
        public ObservableCollection<Estimation> Estimators { get; private set; }
        public ObservableCollection<Salesman> Salesmen { get; private set; }

        public ICollectionView ContactsCollection
        {
            get => _ContactsCollection;
            set => SetValue(ref _ContactsCollection, value);
        }

        public bool InquiryEnable
        {
            get
            {
                if (NewInquiryData.Status == "Order")
                    return false;

                if (NewInquiryData.Status == "Submitted")
                    return false;

                if (!UserData.ModifyInquiries)
                    return false;

                return true;
            }
        }
        public bool QuotationEnable
        {
            get
            {
                if (NewQuotationData.QuotationStatus != "Running")
                    return false;

                if (!UserData.ModifyQuotations)
                    return false;

                if (QuotationData == null)
                    return false;

                if (UserData.EmployeeId != QuotationData.EstimationID)
                    return false;

                return true;
            }
        }
        public Visibility IsQuotation
        {
            get
            {
                if (QuotationData != null)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand DefaultCommand { get; }
        public RelayCommand<Contact> AddContactCommand { get; }
        public RelayCommand<Contact> AttentionCommand { get; }
        public RelayCommand<Contact> RemoveContactCommand { get; }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            if (QuotationData == null)
            {
                if (NewInquiryData.Status == "Quoting")
                {
                    query = $"Select * From [Quotation].[Quotations(View)] Where QuotationID = {NewInquiryData.QuotationID}";
                    QuotationData = connection.QueryFirstOrDefault<Quotation>(query);
                    NewQuotationData.Update(QuotationData);
                }
            }
            GetCustomers(connection);
            GetConsultants(connection);

            query = $"Select * " +
                    $"From [User].[Estimations] " +
                    $"Order By Name";
            Estimators = new ObservableCollection<Estimation>(connection.Query<Estimation>(query)) { new() };

            query = $"Select * " +
                    $"From [User].[Salesmen] " +
                    $"Order By Name";
            Salesmen = new ObservableCollection<Salesman>(connection.Query<Salesman>(query)) { new() };

            if (InquiryData.RegisterCode == null)
            {
                GetRegisterCode(connection);
            }
        }

        private void GetRegisterCode(SqlConnection connection)
        {
            string query = $"Select IsNull(Number, 0) as Number From [Inquiry].[Inquiries(Number)] Where Year = {DateTime.Now.Year}; ";

            NewInquiryData.RegisterNumber = connection.QueryFirstOrDefault<int>(query) + 1;
            NewInquiryData.RegisterMonth = DateTime.Now.Month;
            NewInquiryData.RegisterYear = DateTime.Now.Year;
            NewInquiryData.RegisterCode =
            $"ER-Inquiry{NewInquiryData.RegisterNumber:000}/{NewInquiryData.RegisterMonth}/{NewInquiryData.RegisterYear.ToString().Substring(2, 2)}";
        }
        private void GetCustomers(SqlConnection connection)
        {
            string query;
            if (InquiryEnable)
            {
                query = $"Select * " +
                        $"From [Customer].[Customers(View)]" +
                        $"Order By CustomerName";
                Customers = new ObservableCollection<Customer>(connection.Query<Customer>(query));
            }
            else
            {
                query = $"Select * " +
                        $"From [Customer].[Customers(View)]" +
                        $"Where CustomerID = {NewInquiryData.CustomerID}";
                Customers = new ObservableCollection<Customer>(connection.Query<Customer>(query));
            }
        }
        private void GetContacts()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Customer].[Contacts] " +
                    $"Where CustomerID = {SelectedCustomer.Id} " +
                    $"And ContactID Not In " +
                    $"(Select ContactID From [Inquiry].[ProjectsContacts] Where InquiryID = {NewInquiryData.Id})" +
                    $"Order By ContactName ";
            CustomerContacts = new ObservableCollection<Contact>(connection.Query<Contact>(query));

            query = $"Select * From [Inquiry].[ProjectsContacts(View)] " +
                    $"Where InquiryID = {NewInquiryData.Id} " +
                    $"Order By ContactName ";
            ProjectContacts = new ObservableCollection<Contact>(connection.Query<Contact>(query));
            CreateCollectionView();
        }
        private void ResetProjectContacts()
        {
            ProjectContacts.Clear();
        }
        private void GetConsultants(SqlConnection connection)
        {
            string query;
            if (InquiryEnable)
            {
                query = $"Select * " +
                        $"From [Customer].[Consultants]" +
                        $"Order By ConsultantName";

                Consultants = new ObservableCollection<Consultant>(connection.Query<Consultant>(query));
            }
            else
            {
                query = $"Select * " +
                        $"From [Customer].[Consultants]" +
                        $"Where ConsultantID = {NewInquiryData.ConsultantID}";

                Consultants = new ObservableCollection<Consultant>(connection.Query<Consultant>(query));
            }
        }
        private void CreateCollectionView()
        {
            ContactsCollection = CollectionViewSource.GetDefaultView(ProjectContacts);

            ContactsCollection.SortDescriptions.Add(new SortDescription("ContactName", ListSortDirection.Descending));
            ContactsCollection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            ContactsIndicator = DataGridIndicator.Get(SelectedContactIndex, ContactsCollection);
        }

        private void Save()
        {
            string query;
            bool isReady = true;
            string message = "Please Enter:";
            if (NewInquiryData.CustomerID == 0) { message += $"\n  Customer Name."; isReady = false; }
            if (NewInquiryData.ProjectName == null || NewInquiryData.ProjectName == "") { message += $"\n  Project Name."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewInquiryData.ProjectStatus)) { message += $"\n  Project Status."; isReady = false; }
            if (ProjectContacts.Count == 0) { message += "\n  Attention."; isReady = false; }

            if (isReady)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    if (NewInquiryData.Id == 0)
                    {
                        GetRegisterCode(connection);

                        _ = connection.Insert(NewInquiryData);

                        if (Inquiries != null)
                        {
                            Inquiries.Insert(0, NewInquiryData);
                        }
                    }
                    else
                    {
                        _ = connection.Update(NewInquiryData);
                        InquiryData.Update(NewInquiryData);

                        if (QuotationData != null)
                        {
                            NewQuotationData.QuotationCode =
                                QuotationServices.UpdateCode(NewQuotationData.QuotationCode, SelectedEstimator.Code);

                            _ = connection.Update(NewQuotationData);
                            QuotationData.Update(NewQuotationData);
                        }
                    }

                    #region Contacts
                    query = $"Delete From [Inquiry].[ProjectsContacts] Where InquiryID = {NewInquiryData.InquiryID}";
                    _ = connection.Execute(query);

                    if (ProjectContacts.Count != 0)
                    {
                        query = $"Insert Into [Inquiry].[ProjectsContacts] " +
                                $"(InquiryID, ContactID, Attention) " +
                                $"Values " +
                                $"({NewInquiryData.InquiryID}, @ContactID, @Attention)";
                        _ = connection.Execute(query, ProjectContacts);
                    }
                    #endregion
                }

                Navigation.Back();

            }
            else
            {
                _ = MessageView.Show("Error", message, MessageViewButton.OK, MessageViewImage.Information);
            }
        }
        private bool CanSave()
        {
            if (NewInquiryData.CustomerID == 0)
                return false;

            if (!InquiryEnable && !QuotationEnable)
                return false;

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

        private void Default()
        {
            NewQuotationData.PowerVoltage = "230-400V";
            NewQuotationData.Phase = "3P + N";
            NewQuotationData.Frequency = "60Hz";
            NewQuotationData.NetworkSystem = "AC";
            NewQuotationData.ControlVoltage = "230V AC";
            NewQuotationData.TinPlating = "Bare Copper";
            NewQuotationData.NeutralSize = "Full of Phase";
            NewQuotationData.EarthSize = "Half of Neutral";
            NewQuotationData.EarthingSystem = "TNS";
        }
        private bool CanDefault()
        {
            return true;
        }

        private void AddContact(Contact contact)
        {
            if (ProjectContacts.Count == 0)
            {
                contact.Attention = true;
            }

            CustomerContacts.Remove(contact);
            ProjectContacts.Add(contact);
        }
        private bool CanAddContact(Contact contact)
        {
            if (contact == null)
                return false;

            return true;
        }

        private void Attention(Contact contact)
        {
            contact.Attention = true;
            using SqlConnection connection = new(Database.ConnectionString);
            foreach (Contact contactData in ProjectContacts)
            {
                if (contactData.ContactID != contact.ContactID)
                {
                    contactData.Attention = false;
                }
            }
        }
        private bool CanAttention(Contact contact)
        {
            if (contact == null)
                return false;

            if (contact.Attention)
                return false;

            if (!UserData.ModifyInquiries)
                return false;

            if (UserData.EmployeeId != InquiryData.EstimationID)
                return false;

            return true;
        }

        private void RemoveContact(Contact contact)
        {
            if (contact.Attention && ProjectContacts.Count > 1)
            {
                contact.Attention = false;
                Contact newAttention = ProjectContacts.First(x => x.ContactID != contact.ContactID);
                newAttention.Attention = true;
            }

            ProjectContacts.Remove(contact);
            CustomerContacts.Add(contact);
        }
        private bool CanRemoveContact(Contact contact)
        {
            if (contact == null)
                return false;

            if (!UserData.ModifyInquiries)
                return false;

            if (UserData.EmployeeId != InquiryData.EstimationID)
                return false;

            return true;
        }
    }
}