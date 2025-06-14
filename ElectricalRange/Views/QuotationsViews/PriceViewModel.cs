using Dapper.Contrib.Extensions;

using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;

using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.QuotationsViews
{

    internal class PriceViewModel : ViewModelBase
    {
        private decimal? _Discount;
        private decimal? _DiscountValue;
        public PriceViewModel(Quotation quotation)
        {
            UserData = Navigation.UserData;
            QuotationData = quotation;
            NewData.Update(QuotationData);

            Discount = NewData.Discount;
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public User UserData { get; }
        public Quotation NewData { get; } = new Quotation();
        public Quotation QuotationData { get; set; }

        public decimal? Discount
        {
            get => _Discount;
            set
            {
                if (value != null)
                {
                    if (value.Value > UserData.QuotationsDiscountValue)
                    {
                        _ = MessageView.Show($"Discount", 
                                             $"Max discount can be apply is {UserData.QuotationsDiscountValue}!!", 
                                             MessageViewButton.OK, MessageViewImage.Warning);

                        _Discount = NewData.Discount = UserData.QuotationsDiscountValue;
                        _DiscountValue = NewData.QuotationDiscountValue = NewData.QuotationPrice * (NewData.Discount / 100);
                        NewData.QuotationEstimatedPrice = NewData.QuotationPrice * (1 - NewData.Discount / 100);
                    }
                    else
                    {
                        _Discount = NewData.Discount = value.Value;
                        _DiscountValue = NewData.QuotationDiscountValue = NewData.QuotationPrice * (NewData.Discount / 100);
                        NewData.QuotationEstimatedPrice = NewData.QuotationPrice * (1 - NewData.Discount / 100);
                    }
                }
                else
                {
                    _Discount = NewData.Discount = 0;
                    _DiscountValue = NewData.QuotationDiscountValue = NewData.QuotationPrice * (NewData.Discount / 100);
                    NewData.QuotationEstimatedPrice = NewData.QuotationPrice * (1 - NewData.Discount / 100);
                }

                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(DiscountValue));
            }
        }
        public decimal? DiscountValue
        {
            get => _DiscountValue;
            set
            {
                if (value != null)
                {
                    var newDiscount = value / NewData.QuotationPrice * 100;

                    if (newDiscount > UserData.QuotationsDiscountValue)
                    {
                        _ = MessageView.Show($"Discount",
                                             $"Max discount can be apply is {UserData.QuotationsDiscountValue}!!",
                                             MessageViewButton.OK, MessageViewImage.Warning);

                        _Discount = NewData.Discount = UserData.QuotationsDiscountValue;
                        _DiscountValue = NewData.QuotationDiscountValue = NewData.QuotationPrice * (NewData.Discount / 100);
                        NewData.QuotationEstimatedPrice = NewData.QuotationPrice * (1 - NewData.Discount / 100);
                    }
                    else
                    {
                        _Discount = NewData.Discount = newDiscount.Value;
                        _DiscountValue = NewData.QuotationDiscountValue = value.Value;
                        NewData.QuotationEstimatedPrice = NewData.QuotationPrice * (1 - NewData.Discount / 100);
                    }
                }
                else
                {
                    _Discount = NewData.Discount = 0;
                    _DiscountValue = NewData.QuotationDiscountValue = NewData.QuotationPrice * (NewData.Discount / 100);
                    NewData.QuotationEstimatedPrice = NewData.QuotationPrice * (1 - NewData.Discount / 100);
                }

                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(DiscountValue));
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

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

        private void Save()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Update(NewData);
            }

            QuotationData.Update(NewData);
            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != QuotationData.EstimationID)
                return false;

            if (QuotationData.QuotationStatus != Statuses.Running.ToString())
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