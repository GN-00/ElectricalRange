using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class TermsViewModel : ViewModelBase
    {
        private string _Title;
        private Term _SelectedTerm;
        private ObservableCollection<Term> _Terms;

        private bool _IsScope;
        private bool _IsPrice;
        private bool _IsPayment;
        private bool _IsValidity;
        private bool _IsShopDrawing;
        private bool _IsDelivery;
        private bool _IsGuarantee;
        private bool _IsRemarks;

        public TermsViewModel(Quotation quotation, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            QuotationData = quotation;

            GetData();

            IsScope = true;
            AddCommand = new RelayCommand(Add, CanAdd);
            EditCommand = new RelayCommand<Term>(Edit, CanEdit);
            DeleteCommand = new RelayCommand<Term>(Delete, CanDelete);
            CopyCommand = new RelayCommand<Term>(Copy, CanCopy);
            UpCommand = new RelayCommand<Term>(Up, CanUp);
            DownCommand = new RelayCommand<Term>(Down, CanDown);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public User UserData { get; }
        public bool IsEditable
        {
            get
            {
                if (!UserData.ModifyQuotations)
                    return false;

                if (UserData.EmployeeId != QuotationData.EstimationID)
                    return false;

                if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                    return false;

                return true;
            }
        }
        public Quotation QuotationData { get; }

        public string Title
        {
            get => _Title;
            set => SetValue(ref _Title, value);
        }
        public Term SelectedTerm
        {
            get => _SelectedTerm;
            set => SetValue(ref _SelectedTerm, value);
        }
        public ObservableCollection<Term> Terms
        {
            get => _Terms;
            private set
            {
                if (SetValue(ref _Terms, value))
                {
                    CreateCollectionView();
                }
            }
        }
        public ObservableCollection<Term> NewTerms { get; } = new ObservableCollection<Term>();
        public ObservableCollection<Term> DeletedTerms { get; } = new ObservableCollection<Term>();
        public ICollectionView TermsCollection { get; private set; }


        public bool IsScope
        {
            get => _IsScope;
            set
            {
                if (SetValue(ref _IsScope, value))
                {
                    if (value)
                    {
                        Title = "Scope Of Supply";
                        ConditionType = ConditionTypes.ScopeOfSupply.ToString();
                        TermsCollection.Refresh();
                    }
                }
            }
        }
        public bool IsPrice
        {
            get => _IsPrice;
            set
            {
                if (SetValue(ref _IsPrice, value))
                {
                    if (value)
                    {
                        Title = "Total Price";
                        ConditionType = ConditionTypes.TotalPrice.ToString();
                        TermsCollection.Refresh();
                    }
                }
            }
        }
        public bool IsPayment
        {
            get => _IsPayment;
            set
            {
                if (SetValue(ref _IsPayment, value))
                {
                    if (value)
                    {
                        Title = "Payment Conditions";
                        ConditionType = ConditionTypes.PaymentConditions.ToString();
                        TermsCollection.Refresh();
                    }
                }
            }
        }
        public bool IsValidity
        {
            get => _IsValidity;
            set
            {
                if (SetValue(ref _IsValidity, value))
                {
                    if (value)
                    {
                        Title = "Validity Period";
                        ConditionType = ConditionTypes.ValidityPeriod.ToString();
                        TermsCollection.Refresh();
                    }
                }
            }
        }
        public bool IsShopDrawing
        {
            get => _IsShopDrawing;
            set
            {
                if (SetValue(ref _IsShopDrawing, value))
                {
                    if (value)
                    {
                        Title = "Shop Drawing Submittals";
                        ConditionType = ConditionTypes.ShopDrawingSubmittals.ToString();
                        TermsCollection.Refresh();
                    }
                }
            }
        }
        public bool IsDelivery
        {
            get => _IsDelivery;
            set
            {
                if (SetValue(ref _IsDelivery, value))
                {
                    if (value)
                    {
                        Title = "Delivery";
                        ConditionType = ConditionTypes.Delivery.ToString();
                        TermsCollection.Refresh();
                    }
                }
            }
        }
        public bool IsGuarantee
        {
            get => _IsGuarantee;
            set
            {
                if (SetValue(ref _IsGuarantee, value))
                {
                    if (value)
                    {
                        Title = "Guarantee";
                        ConditionType = ConditionTypes.Guarantee.ToString();
                        TermsCollection.Refresh();
                    }
                }
            }
        }

        public bool IsRemarks
        {
            get => _IsRemarks;
            set
            {
                if (SetValue(ref _IsRemarks, value))
                {
                    if (value)
                    {
                        Title = "Remarks";
                        ConditionType = ConditionTypes.Remarks.ToString();
                        TermsCollection.Refresh();
                    }
                }
            }
        }
        public RelayCommand AddCommand { get; }
        public RelayCommand<Term> EditCommand { get; }
        public RelayCommand<Term> DeleteCommand { get; }
        public RelayCommand<Term> CopyCommand { get; }
        public RelayCommand<Term> UpCommand { get; }
        public RelayCommand<Term> DownCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            string query = $"Select * From [Quotation].[Terms&Conditions] " +
                           $"Where QuotationID = {QuotationData.QuotationID} Order By Sort";

            Terms = new ObservableCollection<Term>(connection.Query<Term>(query));
        }
        private void CreateCollectionView()
        {
            TermsCollection = CollectionViewSource.GetDefaultView(Terms);

            TermsCollection.Filter = new Predicate<object>(DataFilter);
            TermsCollection.SortDescriptions.Add(new SortDescription("Sort", ListSortDirection.Ascending));
        }

        private void Add()
        {
            int newSort = TermsCollection.Cast<Term>().Count() + 1;
            Term term = new()
            {
                QuotationID = QuotationData.QuotationID,
                ConditionType = this.ConditionType,
                IsUsed = true, 
                Sort = newSort 
            };
            Navigation.OpenPopup(new TermView(term, Terms), PlacementMode.Center, true);
        }
        private bool CanAdd()
        {
            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            return true;
        }

        private void Edit(Term term)
        {
            Navigation.OpenPopup(new TermView(term, null), PlacementMode.Center, true);
        }
        private bool CanEdit(Term term)
        {
            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (term == null)
                return false;

            if (term.IsDefault)
                return false;

            return true;
        }

        private void Delete(Term term)
        {
            MessageBoxResult result = MessageView.Show($"Deleting",
                                                       $"Are you Sure to delete this term?",
                                                       MessageViewButton.YesNo,
                                                       MessageViewImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _ = Terms.Remove(term);
                DeletedTerms.Add(term);

                IEnumerable<Term> affectedTerms = TermsCollection.Cast<Term>().Where(x => x.Sort > term.Sort);
                foreach (Term termData in affectedTerms)
                {
                    --termData.Sort;
                }
            }
        }
        private bool CanDelete(Term term)
        {
            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (term == null)
                return false;

            if (term.IsDefault)
                return false;

            return true;
        }

        private void Copy(Term term)
        {
            Clipboard.SetText(term.Condition);
        }
        private bool CanCopy(Term term)
        {
            return true;
        }

        private void Up(Term term)
        {
            term.Sort--;
            Term affectedTerm = TermsCollection.Cast<Term>().FirstOrDefault(x => x.Sort == term.Sort && x.TermID != term.TermID);

            if (affectedTerm != null)
            {
                affectedTerm.Sort++;
            }

            TermsCollection.Refresh();
        }
        private bool CanUp(Term term)
        {
            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (term == null)
                return false;

            if (term.Sort <= 1)
                return false;

            return true;
        }

        private void Down(Term term)
        {
            term.Sort++;
            Term affectedTerm = TermsCollection.Cast<Term>().FirstOrDefault(x => x.Sort == term.Sort && x.TermID != term.TermID);

            if (affectedTerm != null)
            {
                affectedTerm.Sort--;
            }

            TermsCollection.Refresh();
        }
        private bool CanDown(Term term)
        {
            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (term == null)
                return false;

            if (term.Sort >= Terms.Count)
                return false;

            return true;
        }

        private void Save()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Update(Terms.Where(x => x.TermID != 0));

                foreach (Term term in Terms.Where(x => x.TermID == 0))
                    NewTerms.Add(term);

                if (NewTerms.Count != 0)
                {
                    _ = connection.Insert(NewTerms);
                }

                if (DeletedTerms.Count != 0)
                {
                    _ = connection.Delete(DeletedTerms);
                }
            }

            Navigation.Back();
        }
        private bool CanSave()
        {
            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
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

        #region Data Filter

        public string ConditionType { get; set; }

        private bool DataFilter(object item)
        {
            bool result = true;
            string columnName = "ConditionType";

            string value = $"{item.GetType().GetProperty(columnName).GetValue(item)}".ToUpper();
            string checkValue = "" + GetType().GetProperty(columnName).GetValue(this);

            if (value != checkValue.ToUpper())
            {
                result = false;
            }

            return result;
        }

        #endregion
    }
}