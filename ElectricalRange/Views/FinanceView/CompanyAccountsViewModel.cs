using Dapper;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class CompanyAccountsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private Account _SelectedItem;
        private ObservableCollection<Account> _Items;

        private string _Name;
        private string _Balance;
        private string _Type;
        private string _Bank;
        private string _IBAN;
        private string _AccountNumber;

        private ICollectionView _ItemsView;

        public CompanyAccountsViewModel(IView view = null)
        {
            ViewData = view;
            AccessKeys.Add("AccountId");

            GetData();
            NewCommand = new RelayCommand(New, CanAccessNew);
            EditCommand = new RelayCommand<Account>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Account>(Delete, CanDelete);
            BankCommand = new RelayCommand<Account>(BankInfo, CanAccessBankInfo);
            TransactionsCommand = new RelayCommand<Account>(Transactions, CanAccessTransacctions);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData => Navigation.UserData;
        public string Indicator
        {
            get => _Indicator;
            set => SetValue(ref _Indicator, value);
        }
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (SetValue(ref _SelectedIndex, value))
                {
                    UpdateIndicator();
                }
            }
        }
        public Account SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<Account> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                }
            }
        }
        public ICollectionView ItemsView
        {
            get => _ItemsView;
            set => SetValue(ref _ItemsView, value);
        }

        public RelayCommand NewCommand { get; }
        public RelayCommand<Account> EditCommand { get; }
        public RelayCommand<Account> DeleteCommand { get; }
        public RelayCommand<Account> BankCommand { get; }
        public RelayCommand<Account> TransactionsCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string Name
        {
            get => _Name;
            set
            {
                if (SetValue(ref _Name, value))
                {
                    FilterProperty = nameof(Name);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Balance
        {
            get => _Balance;
            set
            {
                if (SetValue(ref _Balance, value))
                {
                    FilterProperty = nameof(Balance);
                    ItemsView.Refresh();
                }
            }
        }


        [FilterProperty]
        public string Type
        {
            get => _Type;
            set
            {
                if (SetValue(ref _Type, value))
                {
                    FilterProperty = nameof(Type);
                    ItemsView.Refresh();
                }
            }
        }


        [FilterProperty]
        public string Bank
        {
            get => _Bank;
            set
            {
                if (SetValue(ref _Bank, value))
                {
                    FilterProperty = nameof(Bank);
                    ItemsView.Refresh();
                }
            }
        }


        [FilterProperty]
        public string IBAN
        {
            get => _IBAN;
            set
            {
                if (SetValue(ref _IBAN, value))
                {
                    FilterProperty = nameof(IBAN);
                    ItemsView.Refresh();
                }
            }
        }

        [FilterProperty]
        public string AccountNumber
        {
            get => _AccountNumber;
            set
            {
                if (SetValue(ref _AccountNumber, value))
                {
                    FilterProperty = nameof(AccountNumber);
                    ItemsView.Refresh();
                }
            }
        }



        private bool DataFilter(object item)
        {
            if (FilterProperty == null)
                return true;

            bool result = true;
            string columnName = FilterProperty;

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (!value.Contains(checkValue.ToUpper()))
            {
                result = false;
            }

            return result;
        }
        private void DeleteFilter()
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                FilterProperty attribute = (FilterProperty)property.GetCustomAttribute(typeof(FilterProperty));
                if (attribute != null)
                {
                    property.SetValue(this, null);
                }
            }

            ItemsView.Refresh();
        }

        #endregion

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[Accounts(View)]" +
                    $"Order By Name";
            Items = new ObservableCollection<Account>(connection.Query<Account>(query));
        }
        private void CreateCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            ItemsView.Filter = new Predicate<object>(DataFilter);
            ItemsView.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));
            ItemsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            ItemsView.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsView);
        }

        private void New()
        {
            Account account = new() { CreateDate = DateTime.Now };
            Navigation.OpenPopup(new CompanyAccountView(account, Items), PlacementMode.Center, true);
        }
        private bool CanAccessNew()
        {
            if (!UserData.ModifyCompanyAccount)
                return false;

            return true;
        }

        private void Edit(Account account)
        {
            if (UserData.Access(account))
            {
                Navigation.OpenPopup(new CompanyAccountView(account, Items), PlacementMode.Center, true);
                Navigation.ClosePopupEvent += () => ItemsView.Refresh();
            }
        }
        private bool CanEdit(Account account)
        {
            if (!UserData.ModifyCompanyAccount)
                return false;

            if (account == null)
                return false;

            return true;
        }

        private void Delete(Account account)
        {
            if (UserData.Access(account))
            {
                Account checkAccount;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    string query = $"Select AccountID As ID From [Finance].[MoneyTransactions] Where AccountID = {account.ID}";
                    checkAccount = connection.QueryFirstOrDefault<Account>(query);
                }

                if (checkAccount == null)
                {
                    MessageBoxResult result = MessageWindow.Show("Account", $"Are you sure want to delete\n{account.Name}?", MessageWindowButton.YesNo, MessageWindowImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = new(Database.ConnectionString))
                        {
                            string query = $"Delete From [Finance].[CompanyAccounts] Where ID = {account.ID};" +
                                           $"Delete From [Finance].[BankAccounts] Where ID = {account.BankID};";
                            _ = connection.Execute(query);
                        }

                        _ = Items.Remove(account);
                    }
                }
                else
                {
                    _ = MessageWindow.Show("Account", "Can't delete this account!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                }
            }
        }
        private bool CanDelete(Account item)
        {
            return CanEdit(item);
        }

        private void BankInfo(Account account)
        {
            if (UserData.Access(account))
            {
                Navigation.OpenPopup(new BankInformationView(account), PlacementMode.Center, true);
            }
        }
        private bool CanAccessBankInfo(Account account)
        {
            if (!UserData.ModifyCompanyAccount)
                return false;

            if (account == null)
                return false;

            if (account.Type == "Cash")
                return false;

            return true;
        }

        private void Transactions(Account account)
        {
            Navigation.To(new AccountTransactionsView(account), ViewData);
        }
        private bool CanAccessTransacctions(Account account)
        {
            if (account == null)
                return false;

            return true;
        }
    }
}