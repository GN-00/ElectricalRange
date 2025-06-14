using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Views.FinanceView
{
    internal class PaymentViewModel : ViewModelBase
    {
        private string _PartnerType;
        private Partner _SelectedPartner;
        private ObservableCollection<Partner> _Partners;

        private Account _SelectedAccount;
        private ObservableCollection<Account> _Accounts;

        private AccountTransaction _NewData;
        public PaymentViewModel(AccountTransaction transaction, ObservableCollection<AccountTransaction> transactions)
        {
            GetData();
            NewData = new AccountTransaction();
            NewData.Update(transaction);

            if (transaction.Id != 0)
            {
                if (NewData.CustomerId != null)
                {
                    PartnerType = "Customer";
                    SelectedPartner = Partners.FirstOrDefault(x => x.CustomerId == NewData.CustomerId);
                }
                else if (NewData.SupplierId != null)
                {
                    PartnerType = "Supplier";
                    SelectedPartner = Partners.FirstOrDefault(x => x.SupplierId == NewData.SupplierId);
                }
                else
                {
                    PartnerType = "Other";
                    SelectedPartner = Partners.FirstOrDefault(x => x.OtherId == NewData.OtherId);
                }

                SelectedAccount = Accounts.FirstOrDefault(x => x.ID == NewData.AccountId);
            }

            TransactionData = transaction;
            TransactionsData = transactions;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public AccountTransaction NewData
        {
            get => _NewData;
            set => SetValue(ref _NewData, value);
        }
        public User UserData => Navigation.UserData;
        public AccountTransaction TransactionData { get; }
        public ObservableCollection<AccountTransaction> TransactionsData { get; }

        public string PartnerType
        {
            get => _PartnerType;
            set
            {
                if (SetValue(ref _PartnerType, value))
                {
                    GetPartnerData();
                }
            }
        }
        public Partner SelectedPartner
        {
            get => _SelectedPartner;
            set
            {
                if (SetValue(ref _SelectedPartner, value))
                {
                    if (SelectedPartner != null)
                    {
                        NewData.CustomerId = SelectedPartner.CustomerId;
                        NewData.SupplierId = SelectedPartner.SupplierId;
                        NewData.OtherId = SelectedPartner.OtherId;

                        NewData.Name = SelectedPartner.Name;
                        NewData.VATNumber = SelectedPartner.VATNumber;
                    }
                    else
                    {
                        NewData.CustomerId = null;
                        NewData.SupplierId = null;
                        NewData.OtherId = null;

                        NewData.Name = null;
                        NewData.VATNumber = null;
                    }
                }
            }
        }
        public ObservableCollection<Partner> Partners
        {
            get => _Partners;
            set => SetValue(ref _Partners, value);
        }
        public ObservableCollection<Partner> Customers { get; private set; }
        public ObservableCollection<Partner> Suppliers { get; private set; }
        public ObservableCollection<Partner> Others { get; private set; }

        public Account SelectedAccount
        {
            get => _SelectedAccount;
            set
            {
                if (SetValue(ref _SelectedAccount, value))
                {
                    if (SelectedAccount != null)
                    {
                        NewData.AccountId = SelectedAccount.ID;
                        NewData.Account = SelectedAccount.Name;
                    }
                    else
                    {
                        NewData.AccountId = null;
                        NewData.Account = null;
                    }
                }
            }
        }
        public ObservableCollection<Account> Accounts
        {
            get => _Accounts;
            set => SetValue(ref _Accounts, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ClosingCommand { get; }

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[Accounts] " +
                    $"Order By Type, Name";
            Accounts = new ObservableCollection<Account>(connection.Query<Account>(query));

            query = $"Select * From [Finance].[Customers] " +
                    $"Order By Name ";
            Customers = new ObservableCollection<Partner>(connection.Query<Partner>(query));

            query = $"Select * From [Finance].[Suppliers] " +
                    $"Order By Name ";
            Suppliers = new ObservableCollection<Partner>(connection.Query<Partner>(query));

            query = $"Select * From [Finance].[Others] " +
                    $"Order By Name ";
            Others = new ObservableCollection<Partner>(connection.Query<Partner>(query));
        }

        private void GetPartnerData()
        {
            if (PartnerType == "Customer")
            {
                Partners = new ObservableCollection<Partner>(Customers);
            }

            else if (PartnerType == "Supplier")
            {
                Partners = new ObservableCollection<Partner>(Suppliers);
            }

            else
            {
                Partners = new ObservableCollection<Partner>(Others);
            }
        }

        private void Save()
        {
            if (NewData.Id == 0)
            {
                string query = $"Select IsNull(Max(Number), 0) " +
                               $"From [Finance].[MasterTransactions] " +
                               $"Where Type = '{Enums.AccountingTransactions.Payment}'";
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    NewData.Number = connection.QueryFirstOrDefault<int>(query) + 1;
                    NewData.Code = $"PAY{NewData.Number:000000}";

                    _ = connection.Insert(NewData);
                }

                TransactionsData.Add(NewData);
            }
            else
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    _ = connection.Update(NewData);
                }

                TransactionData.Update(NewData);
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (NewData == null)
                return false;

            if (NewData.Date == null)
                return false;

            if (NewData.CustomerId == null && NewData.SupplierId == null && NewData.OtherId == null)
                return false;

            if (NewData.AccountId == null)
                return false;

            if (NewData.Amount <= 0)
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