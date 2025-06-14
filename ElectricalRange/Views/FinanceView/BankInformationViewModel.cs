using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;

using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.FinanceView
{
    internal class BankInformationViewModel : Base
    {
        public BankInformationViewModel(Account account)
        {
            AccountData = account;

            NewData.ID = AccountData.BankID;
            NewData.Name = AccountData.Bank;
            NewData.Number = AccountData.AccountNumber;
            NewData.IBAN = AccountData.IBAN;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        public BankInformationViewModel(CustomerAccount account)
        {
            CustomerAccountData = account;

            NewData.ID = CustomerAccountData.BankID;
            NewData.Name = CustomerAccountData.Bank;
            NewData.Number = CustomerAccountData.AccountNumber;
            NewData.IBAN = CustomerAccountData.IBAN;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        public Account AccountData { get; }
        public CustomerAccount CustomerAccountData { get; }
        public BankAccount NewData { get; } = new BankAccount();
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            bool isReady = true;
            string message = "Please Enter:";

            if (string.IsNullOrWhiteSpace(NewData.Name)) { message += $"\n  Name."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.Number)) { message += $"\n  Number."; isReady = false; }
            if (string.IsNullOrWhiteSpace(NewData.IBAN)) { message += $"\n  IBAN."; isReady = false; }

            if (!isReady)
            {
                _ = MessageView.Show("Error", message, MessageViewButton.OK, MessageViewImage.Information);
                return;
            }

            if (NewData.ID != 0)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                _ = connection.Update(NewData);
            }
            else
            {
                using SqlConnection connection = new(Database.ConnectionString);

                if (AccountData != null)
                {
                    AccountData.BankID = (int)connection.Insert(NewData);
                    _ = connection.Update(AccountData);
                }
                else if (CustomerAccountData != null)
                {
                    CustomerAccountData.BankID = (int)connection.Insert(NewData);
                    _ = connection.Update(CustomerAccountData);
                }
            }

            if (AccountData != null)
            {
                AccountData.BankID = NewData.ID;
                AccountData.Bank = NewData.Name;
                AccountData.AccountNumber = NewData.Number;
                AccountData.IBAN = NewData.IBAN;
            }
            else if (CustomerAccountData != null)
            {
                CustomerAccountData.BankID = NewData.ID;
                CustomerAccountData.Bank = NewData.Name;
                CustomerAccountData.AccountNumber = NewData.Number;
                CustomerAccountData.IBAN = NewData.IBAN;
            }

            Navigation.ClosePopup();
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }
    }
}