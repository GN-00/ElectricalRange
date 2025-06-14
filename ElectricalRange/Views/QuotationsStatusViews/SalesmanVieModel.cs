using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    internal class SalesmanVieModel : ViewModelBase
    {
        private Salesman _Salesman;
        public SalesmanVieModel(ProjectsStatusViewModel model)
        {
            ModelData = model;
            GetData();
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public ProjectsStatusViewModel ModelData { get; }
        public Salesman Salesman
        {
            get => _Salesman;
            set => SetValue(ref _Salesman, value);
        }
        public ObservableCollection<Salesman> Salesmen { get; private set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            Salesmen = SalesmanController.GetSalesmen(connection);
        }

        private void Save()
        {
            ModelData.SalesmanID = Salesman.Id.ToString();
            ModelData.StatusInfo = $"Salesman: {Salesman.Name}";
            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
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