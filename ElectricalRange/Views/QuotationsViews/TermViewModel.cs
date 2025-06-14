using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class TermViewModel : ViewModelBase
    {
        public TermViewModel(Term term, ObservableCollection<Term> terms)
        {
            TermData = term;
            TermsData = terms;
            NewData.Update(term);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }
        public Term NewData { get; } = new Term();
        public Term TermData { get; private set; }
        public ObservableCollection<Term> TermsData { get; private set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            if (TermsData != null)//Add
            {
                TermsData.Add(NewData);
            }
            else //Edit
            {
                TermData.Update(NewData);
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(NewData.Condition))
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