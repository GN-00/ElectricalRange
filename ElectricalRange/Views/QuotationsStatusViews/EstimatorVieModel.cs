using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Inquiries;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Views.QuotationsStatusViews
{
    internal class EstimatorVieModel : ViewModelBase
    {
        private Estimation _Estimator;
        public EstimatorVieModel(ProjectsStatusViewModel model)
        {
            ModelData = model;
            GetData();
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public ProjectsStatusViewModel ModelData { get; }
        public Estimation Estimator
        {
            get => _Estimator;
            set => SetValue(ref _Estimator, value);
        }
        public ObservableCollection<Estimation> Estimators { get; private set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void GetData()
        {
            using SqlConnection connection = new(Database.ConnectionString);
            Estimators = EstimationController.GetEstimation(connection);
        }

        private void Save()
        {
            ModelData.EstimationID = Estimator.Id.ToString();
            ModelData.StatusInfo = $"Estimator: {Estimator.Name}"; 
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