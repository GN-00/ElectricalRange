using ClosedXML.Excel;

using Dapper;
using Dapper.Contrib.Extensions;

using FastMember;

using Microsoft.Win32;

using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Finance;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Printing.Finance;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.FinanceView
{
    public class PaymentsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private AccountTransaction _SelectedItem;
        private ObservableCollection<AccountTransaction> _Items;

        private string _Code;
        private string _DateInfo;
        private string _Name;
        private string _Account;
        private decimal _Amount;
        private string _Description;

        private ICollectionView _ItemsCollection;

        public PaymentsViewModel(IView view)
        {
            ViewData = view;

            GetData();

            NewCommand = new RelayCommand(New, CanAccessNew);
            EditCommand = new RelayCommand<AccountTransaction>(Edit, CanAccessEdit);
            ViewCommand = new RelayCommand<AccountTransaction>(View, CanAccessView);
            DeleteCommand = new RelayCommand<AccountTransaction>(Delete, CanAccessDelete);
            PostCommand = new RelayCommand<AccountTransaction>(Post, CanAccessPost);
            ExportCommand = new RelayCommand(Export, CanAccessExport);
            DeleteFilterCommand = new RelayCommand(DeleteFilter);

            AttachCommand = new RelayCommand<AccountTransaction>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<AccountTransaction>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<AccountTransaction>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<AccountTransaction>(ReadAttachment, CanAccessReadAttachment);
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
        public AccountTransaction SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<AccountTransaction> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                    UpdateIndicator();
                }
            }
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand NewCommand { get; }
        public RelayCommand<AccountTransaction> EditCommand { get; }
        public RelayCommand<AccountTransaction> ViewCommand { get; }
        public RelayCommand<AccountTransaction> DeleteCommand { get; }
        public RelayCommand<AccountTransaction> PostCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        public RelayCommand<AccountTransaction> AttachCommand { get; }
        public RelayCommand<AccountTransaction> DeleteAttachmentCommand { get; }
        public RelayCommand<AccountTransaction> DownloadAttachmentCommand { get; }
        public RelayCommand<AccountTransaction> ReadAttachmentCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string Code
        {
            get => _Code;
            set
            {
                if (SetValue(ref _Code, value))
                {
                    FilterProperty = nameof(Code);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string DateInfo
        {
            get => _DateInfo;
            set
            {
                if (SetValue(ref _DateInfo, value))
                {
                    FilterProperty = nameof(DateInfo);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Name
        {
            get => _Name;
            set
            {
                if (SetValue(ref _Name, value))
                {
                    FilterProperty = nameof(Name);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Account
        {
            get => _Account;
            set
            {
                if (SetValue(ref _Account, value))
                {
                    FilterProperty = nameof(Account);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public decimal Amount
        {
            get => _Amount;
            set
            {
                if (SetValue(ref _Amount, value))
                {
                    FilterProperty = nameof(Amount);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string Description
        {
            get => _Description;
            set
            {
                if (SetValue(ref _Description, value))
                {
                    FilterProperty = nameof(Description);
                    ItemsCollection.Refresh();
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
            ItemsCollection.Refresh();
        }

        #endregion

        private void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.ConnectionString);
            query = $"Select * From [Finance].[Payments(View)] " +
                    $"Order By Date Desc ";

            Items = new ObservableCollection<AccountTransaction>(connection.Query<AccountTransaction>(query));
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Descending));
            ItemsCollection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIndicator();
        }
        private void UpdateIndicator()
        {
            Indicator = DataGridIndicator.Get(SelectedIndex, ItemsCollection);
        }

        private void New()
        {
            AccountTransaction transaction = new()
            {
                Type = AccountingTransactions.Payment.ToString()
            };

            Navigation.OpenPopup(new PaymentView(transaction, Items), PlacementMode.Center, false);
        }
        private bool CanAccessNew()
        {
            if (!UserData.ModifyPayments)
                return false;

            return true;
        }

        private void Edit(AccountTransaction transaction)
        {
            Navigation.OpenPopup(new PaymentView(transaction, Items), PlacementMode.Center, false);
            Navigation.ClosePopupEvent += () => ItemsCollection.Refresh();
        }
        private bool CanAccessEdit(AccountTransaction transaction)
        {
            if (transaction == null)
                return false;

            if (transaction.Post)
                return false;

            if (!UserData.ModifyPayments)
                return false;

            return true;
        }

        private void View(AccountTransaction transaction)
        {
            PaymentForm receiptForm = new(transaction);
            Printing.Print.PrintPreview(receiptForm, transaction.DateInfo + " Receipt " + transaction.Name, ViewData);
        }

        private bool CanAccessView(AccountTransaction transaction)
        {
            if (transaction == null)
                return false;

            if (!transaction.Post)
                return false;

            return true;
        }

        private void Delete(AccountTransaction transaction)
        {
            MessageBoxResult result = MessageView.Show("Delete",
                                                        $"Are you sure want to delete this record:\n({transaction.Amount:N2})?",
                                                        MessageViewButton.YesNo,
                                                        MessageViewImage.Question);

            if (result == MessageBoxResult.No)
                return;

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Delete(transaction);
            }

            Items.Remove(transaction);
        }

        private bool CanAccessDelete(AccountTransaction transaction)
        {
            if (transaction == null)
                return false;

            if (transaction.Post)
                return false;

            if (!UserData.ModifyPayments)
                return false;

            return true;
        }
        private void Post(AccountTransaction transaction)
        {
            MessageBoxResult result = MessageWindow.Show("Posting",
                                                          $"Are you sure want to post this record:\n({transaction.Amount:N2})?",
                                                          MessageWindowButton.YesNo,
                                                          MessageWindowImage.Question);

            if (result == MessageBoxResult.No)
                return;

            using SqlConnection connection = new(Database.ConnectionString);
            transaction.Post = true;
            _ = connection.Update(transaction);
        }
        private bool CanAccessPost(AccountTransaction transaction)
        {
            if (transaction == null)
                return false;

            if (transaction.Post)
                return false;

            if (!UserData.ModifyPayments)
                return false;

            return true;
        }


        private class ExcelTransaction
        {
            public string Code { get; set; }
            public string Date { get; set; }
            public string Name { get; set; }
            public string Account { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
        }
        private void Export()
        {
            try
            {
                List<ExcelTransaction> list = new();
                foreach (AccountTransaction transaction in ItemsCollection.Cast<AccountTransaction>())
                {
                    ExcelTransaction excelTransaction = new()
                    {
                        Code = transaction.Code,
                        Date = transaction.DateInfo,
                        Name = transaction.Name,
                        Account = transaction.Account,
                        Amount = (decimal)transaction.Amount,
                        Description = transaction.Description,
                    };

                    list.Add(excelTransaction);
                }

                if (list.Count != 0)
                {
                    string fileName;
                    using XLWorkbook workbook = new();
                    DataTable table = new();
                    using (ObjectReader reader = ObjectReader.Create(list))
                    {
                        table.Load(reader);
                    }
                    table.Columns["Code"].SetOrdinal(0);
                    table.Columns["Date"].SetOrdinal(1);
                    table.Columns["Name"].SetOrdinal(2);
                    table.Columns["Account"].SetOrdinal(3);
                    table.Columns["Amount"].SetOrdinal(4);
                    table.Columns["Description"].SetOrdinal(5);

                    string worksheetName = $"Payments {DateTime.Now.Date:dd-MM-yyyy}";
                    worksheetName = $"{worksheetName}";
                    _ = workbook.Worksheets.Add(table, worksheetName);

                    IXLWorksheet workSheet = workbook.Worksheet(worksheetName);

                    _ = workSheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    _ = workSheet.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    _ = workSheet.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    _ = workSheet.Column(1).AdjustToContents();
                    _ = workSheet.Column(2).AdjustToContents();
                    _ = workSheet.Column(3).AdjustToContents();
                    _ = workSheet.Column(4).AdjustToContents();
                    _ = workSheet.Column(5).AdjustToContents();
                    _ = workSheet.Column(6).AdjustToContents();

                    _ = workSheet.Cell(1, 3).Value = "Paid By";
                    _ = workSheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    _ = workSheet.Cell(1, 4).Value = "Received In";
                    _ = workSheet.Cell(1, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    fileName = $"{worksheetName}.xlsx";
                    fileName = fileName.Replace("/", "-");

                    SaveFileDialog saveFileDialog = new()
                    {
                        FileName = fileName,
                        DefaultExt = ".xlsx",
                        Filter = "Excel Worksheets|*.xlsx",
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                }
                else
                {
                    _ = MessageView.Show("Records", "There is no record!!", MessageViewButton.OK, MessageViewImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _ = MessageView.Show("Error", ex.Message, MessageViewButton.OK, MessageViewImage.Warning);
            }
        }
        private bool CanAccessExport()
        {
            if (Items == null)
                return false;

            if (Items.Count == 0)
                return false;

            return true;
        }


        private void Attach(AccountTransaction transaction)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (transaction.AttachmentId == null)
            {
                TransactionAttachment attachment = new();

                Attachment.SaveFile<TransactionAttachment>(attachment);

                transaction.AttachmentId = attachment.Id;
                using SqlConnection connection = new(Database.ConnectionString);
                _ = connection.Update(transaction);
            }
            else
            {
                TransactionAttachment attachment = new()
                {
                    Id = transaction.AttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<TransactionAttachment>(attachment);
            }

            IsLoading = false;
        }

        private bool CanAccessAttach(AccountTransaction transaction)
        {
            if (transaction == null)
                return false;

            if (!UserData.ModifyPayments)
                return false;

            return true;
        }

        private void DeleteAttachment(AccountTransaction transaction)
        {
            TransactionAttachment attachment = new()
            {
                Id = transaction.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<TransactionAttachment>(attachment);

            transaction.AttachmentId = null;
            using SqlConnection connection = new(Database.ConnectionString);
            _ = connection.Update(transaction);
        }

        private bool CanAccessDeleteAttachment(AccountTransaction transaction)
        {
            if (transaction == null)
                return false;

            if (transaction.AttachmentId == null)
                return false;

            if (!UserData.ModifyPayments)
                return false;

            return true;
        }

        private void DownloadAttachment(AccountTransaction transaction)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            TransactionAttachment attachment = new()
            {
                Id = transaction.AttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<TransactionAttachment>(attachment);

            IsLoading = false;
        }

        private bool CanAccessDownloadAttachment(AccountTransaction transaction)
        {
            if (transaction == null)
                return false;

            if (transaction.AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(AccountTransaction transaction)
        {
            TransactionAttachment attachment = new()
            {
                Id = transaction.AttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<TransactionAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(AccountTransaction transaction)
        {
            if (transaction == null)
                return false;

            if (transaction.AttachmentId == null)
                return false;

            return true;
        }
    }
}