using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.InquiriesViews;
using ProjectsNow.Views.QuotationsViews;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{
    public class TendaringViewModel : ViewModelBase
    {
        private double _MaxWidth = 1300;
        private User _UserData;

        public TendaringViewModel()
        {
            UserData = Navigation.UserData;
            InquiriesCommand = new RelayCommand(Inquiries, CanAccessInquiries);
            QuoteCommand = new RelayCommand(Quote, CanAccessQuote);
            QuotationsCommand = new RelayCommand(Quotations, CanAccessQuotations);

            int buttons = 0;
            if (CanAccessInquiries()) buttons += 1;
            if (CanAccessQuote()) buttons += 1;
            if (CanAccessQuotations()) buttons += 1;

            if (buttons == 4) MaxWidth = 900;
        }

        public double MaxWidth
        {
            get => _MaxWidth;
            set => SetValue(ref _MaxWidth, value);
        }
        public User UserData
        {
            get => _UserData;
            set => SetValue(ref _UserData, value);
        }

        public RelayCommand InquiriesCommand { get; }
        public RelayCommand QuoteCommand { get; }
        public RelayCommand QuotationsCommand { get; }

        public void Inquiries()
        {
            Navigation.To(new InquiriesView());
        }
        public bool CanAccessInquiries()
        {
            return UserData.AccessInquiries;
        }

        public void Quote()
        {
            Navigation.To(new QuoteView());
        }
        public bool CanAccessQuote()
        {
            return UserData.AccessQuote;
        }

        private void Quotations()
        {
            Navigation.To(new QuotationsView());
        }

        private bool CanAccessQuotations()
        {
            return UserData.AccessQuotations;
        }
    }
}