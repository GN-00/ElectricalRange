using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using System.Globalization;
using System.Windows.Input;

namespace ProjectsNow.Views.QuotationsViews
{

    internal class NoteViewModel : ViewModelBase
    {
        private System.Windows.FlowDirection _DirectionData;

        public NoteViewModel(Quotation quotation)
        {
            QuotationData = quotation;
            NewData.Update(quotation);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);

            ArabicCommand = new RelayCommand(Arabic, CanArabic);
            EnglishCommand = new RelayCommand(English, CanEnglish);

            if (NewData.BillOfPriceNoteLanguage == "Arabic")
            {
                ArabicCommand.Execute();
            }
            else
            {
                EnglishCommand.Execute();
            }
        }

        public Quotation QuotationData { get; set; }
        public Quotation NewData { get; set; } = new Quotation();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ArabicCommand { get; }
        public RelayCommand EnglishCommand { get; }

        private void Save()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Update(NewData);
            }

            QuotationData.BillOfPriceNote = NewData.BillOfPriceNote;
            QuotationData.BillOfPriceNoteLanguage = NewData.BillOfPriceNoteLanguage;

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (NewData == null)
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

        public System.Windows.FlowDirection DirectionData
        {
            get => _DirectionData;
            set => SetValue(ref _DirectionData, value);
        }

        // Replace the problematic code with the following:

        private void Arabic()
        {
            DirectionData = System.Windows.FlowDirection.RightToLeft;
            NewData.BillOfPriceNoteLanguage = "Arabic";
            var culture = System.Globalization.CultureInfo.GetCultureInfo("ar-SA");
            var language = InputLanguageManager.Current.AvailableInputLanguages
                .OfType<CultureInfo>()
                .FirstOrDefault(c => c.Name == culture.Name);

            if (language != null)
                InputLanguageManager.Current.CurrentInputLanguage = language;
            else
                InputLanguageManager.Current.CurrentInputLanguage = CultureInfo.InstalledUICulture;
        }
        private bool CanArabic()
        {
            if (NewData.BillOfPriceNoteLanguage == "Arabic")
                return false;

            return true;
        }

        private void English()
        {
            DirectionData = System.Windows.FlowDirection.LeftToRight;
            NewData.BillOfPriceNoteLanguage = "English";
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            var language = InputLanguageManager.Current.AvailableInputLanguages
                .OfType<CultureInfo>()
                .FirstOrDefault(c => c.Name == culture.Name);

            if (language != null)
                InputLanguageManager.Current.CurrentInputLanguage = language;
            else
                InputLanguageManager.Current.CurrentInputLanguage = CultureInfo.InstalledUICulture;
        }
        private bool CanEnglish()
        {
            if (NewData.BillOfPriceNoteLanguage != "Arabic")
                return false;

            return true;
        }
    }
}