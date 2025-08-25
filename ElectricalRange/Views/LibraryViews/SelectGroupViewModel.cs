using Dapper;

using Microsoft.Data.SqlClient;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Library;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Views.LibraryViews
{
    internal class SelectGroupViewModel : ViewModelBase
    {
        public int PanelId { get; }
        public ObservableCollection<QItem> ItemsData { get; set; }
        public SelectGroupViewModel(int panelId, ObservableCollection<QItem> items)
        {
            PanelId = panelId;
            ItemsData = items;
            GetData();

            SaveCommand = new RelayCommand(Save, CanSave);
            ResetSelectionsCommand = new RelayCommand(ResetSelections, CanResetSelections);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public void GetData()
        {
            string query;
            using SqlConnection connection = new(Database.PSConnectionString);

            query = $"Select Id, Description From [Estimator].[Groups] " +
                    $"Order By Id";
            Groups = connection.Query<GroupInfo>(query).ToList();

            query = $"Select Family From [Estimator].[Groups] " +
                    $"Group By Family " +
                    $"Order By Family ";
            Families = connection.Query<string>(query).ToList();

            query = $"Select Manufacturer From [Estimator].[Groups] " +
                    $"Group By Manufacturer " +
                    $"Order By Manufacturer";
            Manufacturers = connection.Query<string>(query).ToList();
        }

        public void UpdateData()
        {
            string query;
            using SqlConnection connection = new(Database.PSConnectionString);

            query = $"Select Manufacturer From [Estimator].[Groups] ";
            if (SelectedId != null && SelectedFamily != null)
                query += $"Where Id = '{SelectedId}' And Family = '{SelectedFamily}' ";
            else if (SelectedId != null && SelectedFamily == null)
                query += $"Where Id = '{SelectedId}' ";
            else if (SelectedId == null && SelectedFamily != null)
                query += $"Where Family = '{SelectedFamily}' ";

            query += $"Group By Manufacturer " +
                     $"Order By Manufacturer";
            Manufacturers = connection.Query<string>(query).ToList();


            query = $"Select Family From [Estimator].[Groups] ";
            if (SelectedId != null && SelectedManufacturer != null)
                query += $"Where Id = '{SelectedId}' And Manufacturer = '{SelectedManufacturer}' ";
            else if (SelectedId != null && SelectedManufacturer == null)
                query += $"Where Id = '{SelectedId}' ";
            else if (SelectedId == null && SelectedManufacturer != null)
                query += $"Where Manufacturer = '{SelectedManufacturer}' ";
            query += $"Group By Family " +
                    $"Order By Family ";
            Families = connection.Query<string>(query).ToList();


            query = $"Select Id, Description From [Estimator].[Groups] ";
            if (SelectedManufacturer != null && SelectedFamily != null)
                query += $"Where Manufacturer = '{SelectedManufacturer}' And Family = '{SelectedFamily}' ";
            else if (SelectedManufacturer != null && SelectedFamily == null)
                query += $"Where Manufacturer = '{SelectedManufacturer}' ";
            else if (SelectedManufacturer == null && SelectedFamily != null)
                query += $"Where Family = '{SelectedFamily}' ";
            query += $"Order By Id";
            Groups = connection.Query<GroupInfo>(query).ToList();
        }

        public void Selecting()
        {
            bool isReady = true;
            string message = "Please select group!";

            if (SelectedId == null)
                isReady = false;

            if (!isReady)
            {
                MessageView.Show("Error", message, MessageViewButton.OK, MessageViewImage.Warning);
                return;
            }

            //Navigation.ClosePopup();
            Navigation.OpenPopup(new SelectItemsView(SelectedId, PanelId, ItemsData), System.Windows.Controls.Primitives.PlacementMode.Center, true);
        }

        public void Reset()
        {
            IsReset = true;

            SelectedId =
            SelectedFamily =
            SelectedManufacturer = null;

            GetData();
            IsReset = false;
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand ResetSelectionsCommand { get; }
        public RelayCommand CancelCommand { get; }
        private void Save()
        {
            Selecting();
        }
        private bool CanSave()
        {
            return true;
        }


        private void ResetSelections()
        {
            Reset();
        }
        private bool CanResetSelections()
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


        public bool IsReset { get; private set; } = false;

        public BitmapImage ManufacturerImage
        {
            get => _selectedManufacturer == null ? gray : green;
        }

        public BitmapImage FamilyImage
        {
            get => _selectedFamily == null ? gray : green;
        }

        public BitmapImage GroupImage
        {
            get => _selectedId == null ? gray : green;
        }

        private readonly BitmapImage green =
            new(new Uri(@"/Images/Icons/Green.png", UriKind.Relative));

        private readonly BitmapImage gray =
            new(new Uri(@"/Images/Icons/Gray.png", UriKind.Relative));

        private string _selectedId;
        public string SelectedId
        {
            get => _selectedId;
            set
            {
                if (SetValue(ref _selectedId, value))
                {
                    OnPropertyChanged(nameof(GroupImage));
                    UpdateData();
                }
            }
        }

        private string _selectedFamily;
        public string SelectedFamily
        {
            get => _selectedFamily;
            set
            {
                if (SetValue(ref _selectedFamily, value))
                {
                    OnPropertyChanged(nameof(FamilyImage));
                    UpdateData();
                }
            }
        }

        private string _selectedManufacturer;
        public string SelectedManufacturer
        {
            get => _selectedManufacturer;
            set
            {
                if (SetValue(ref _selectedManufacturer, value))
                {
                    OnPropertyChanged(nameof(ManufacturerImage));
                    UpdateData();
                }
            }
        }

        private List<GroupInfo> _groups;
        public List<GroupInfo> Groups
        {
            get => _groups;
            set => SetValue(ref _groups, value);
        }

        private List<string> _manufacturers;
        public List<string> Manufacturers
        {
            get => _manufacturers;
            set => SetValue(ref _manufacturers, value);
        }

        private List<string> _families;
        public List<string> Families
        {
            get => _families;
            set => SetValue(ref _families, value);
        }


        public class GroupInfo
        {
            public string Id { get; set; }
            public string Description { get; set; }
        }
    }
}