using ProjectsNow.Attributes;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Services;
using ProjectsNow.Windows.JobOrderWindows;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ProjectsNow.Views.JobOrdersViews
{
    public class PanelsViewModel : ViewModelBase
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private JPanel _SelectedItem;
        private ObservableCollection<JPanel> _Items;

        private string _PanelSN;
        private string _PanelNameInfo;
        private string _PanelQty;
        private string _EnclosureType;
        private string _EnclosureHeight;
        private string _EnclosureWidth;
        private string _EnclosureDepth;
        private string _EnclosureIP;
        private string _PanelProfit;
        private string _PanelCost;
        private string _PanelPrice;
        private string _PanelsCost;
        private string _PanelsPrice;

        private ICollectionView _ItemsCollection;

        public PanelsViewModel(JobOrder order, IView view)
        {
            ViewData = view;
            UserData = Navigation.UserData;
            OrderData = order;

            GetData();

            InfoCommand = new RelayCommand<JPanel>(Info, CanAccessInfo);
            CopyNameCommand = new RelayCommand<JPanel>(CopyName, CanAccessCopyName);
            ItemsCommand = new RelayCommand<JPanel>(GetItems, CanAccessGetItems);
            ModificationCommand = new RelayCommand(Modification, CanAccessModification);
            DesignCommand = new RelayCommand(Design, CanAccessDesign);
            ApprovalCommand = new RelayCommand(Approval, CanAccessApproval);
            ProductionCommand = new RelayCommand(Production, CanAccessProduction);
            InspectionCommand = new RelayCommand(Inspection, CanAccessInspection);
            ClosingCommand = new RelayCommand(Closing, CanAccessClosing);
            HoldCommand = new RelayCommand(Hold, CanAccessHold);
            CancelCommand = new RelayCommand(Cancel, CanAccessCancel);

            DeleteFilterCommand = new RelayCommand(DeleteFilter);
        }

        public User UserData { get; }
        public JobOrder OrderData { get; }

        public Visibility HasInfoButtons
        {
            get
            {
                if (CanAccessInfo(SelectedItem))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        public Visibility HasItemsButtons
        {
            get
            {
                if (CanAccessGetItems(SelectedItem))
                    return Visibility.Visible;

                if (CanAccessModification())
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

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
        public JPanel SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value)
                  .UpdateProperties(this,
                                    nameof(HasInfoButtons),
                                    nameof(HasItemsButtons));
        }
        public ObservableCollection<JPanel> Items
        {
            get => _Items;
            private set
            {
                if (SetValue(ref _Items, value))
                {
                    CreateCollectionView();
                }
            }
        }
        public ICollectionView ItemsCollection
        {
            get => _ItemsCollection;
            set => SetValue(ref _ItemsCollection, value);
        }

        public RelayCommand<JPanel> InfoCommand { get; }
        public RelayCommand<JPanel> CopyNameCommand { get; }
        public RelayCommand<JPanel> ItemsCommand { get; }
        public RelayCommand ModificationCommand { get; }
        public RelayCommand DesignCommand { get; }
        public RelayCommand ApprovalCommand { get; }
        public RelayCommand ProductionCommand { get; }
        public RelayCommand InspectionCommand { get; }
        public RelayCommand ClosingCommand { get; }
        public RelayCommand HoldCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand DeleteFilterCommand { get; }

        #region Data Filter

        [FilterProperty]
        public string FilterProperty { get; set; }

        [FilterProperty]
        public string PanelSN
        {
            get => _PanelSN;
            set
            {
                if (SetValue(ref _PanelSN, value))
                {
                    FilterProperty = nameof(PanelSN);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelNameInfo
        {
            get => _PanelNameInfo;
            set
            {
                if (SetValue(ref _PanelNameInfo, value))
                {
                    FilterProperty = nameof(PanelNameInfo);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelQty
        {
            get => _PanelQty;
            set
            {
                if (SetValue(ref _PanelQty, value))
                {
                    FilterProperty = nameof(PanelQty);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureType
        {
            get => _EnclosureType;
            set
            {
                if (SetValue(ref _EnclosureType, value))
                {
                    FilterProperty = nameof(EnclosureType);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureHeight
        {
            get => _EnclosureHeight;
            set
            {
                if (SetValue(ref _EnclosureHeight, value))
                {
                    FilterProperty = nameof(EnclosureHeight);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureWidth
        {
            get => _EnclosureWidth;
            set
            {
                if (SetValue(ref _EnclosureWidth, value))
                {
                    FilterProperty = nameof(EnclosureWidth);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureDepth
        {
            get => _EnclosureDepth;
            set
            {
                if (SetValue(ref _EnclosureDepth, value))
                {
                    FilterProperty = nameof(EnclosureDepth);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string EnclosureIP
        {
            get => _EnclosureIP;
            set
            {
                if (SetValue(ref _EnclosureIP, value))
                {
                    FilterProperty = nameof(EnclosureIP);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelProfit
        {
            get => _PanelProfit;
            set
            {
                if (SetValue(ref _PanelProfit, value))
                {
                    FilterProperty = nameof(PanelProfit);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelCost
        {
            get => _PanelCost;
            set
            {
                if (SetValue(ref _PanelCost, value))
                {
                    FilterProperty = nameof(PanelCost);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelsCost
        {
            get => _PanelsCost;
            set
            {
                if (SetValue(ref _PanelsCost, value))
                {
                    FilterProperty = nameof(PanelsCost);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelPrice
        {
            get => _PanelPrice;
            set
            {
                if (SetValue(ref _PanelPrice, value))
                {
                    FilterProperty = nameof(PanelPrice);
                    ItemsCollection.Refresh();
                }
            }
        }

        [FilterProperty]
        public string PanelsPrice
        {
            get => _PanelsPrice;
            set
            {
                if (SetValue(ref _PanelsPrice, value))
                {
                    FilterProperty = nameof(PanelsPrice);
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
            using SqlConnection connection = new(Database.ConnectionString);
            Items = JPanelController.GetJobOrderPanels(connection, OrderData.ID);
        }
        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Items);

            ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
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

        private void Info(JPanel panel)
        {
            Navigation.To(new PanelView(OrderData, panel), ViewData);
        }
        private bool CanAccessInfo(JPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void CopyName(JPanel panel)
        {
            Clipboard.SetText(panel.PanelName);
        }
        private bool CanAccessCopyName(JPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void GetItems(JPanel panel)
        {
            PanelItemsWindow panelItemsWindow = new()
            {
                UserData = UserData,
                PanelData = panel,
                JobOrderData = OrderData,
            };
            _ = panelItemsWindow.ShowDialog();
        }
        private bool CanAccessGetItems(JPanel panel)
        {
            if (panel == null)
                return false;

            return true;
        }

        private void Modification()
        {
            ItemsModificationWindow itemsModificationWindow = new()
            {
                UserData = UserData,
                JobOrderData = OrderData,
            };

            _ = itemsModificationWindow.ShowDialog();
        }
        private bool CanAccessModification()
        {
            return true;
        }

        private void Design()
        {
            Panels_Design_Window window = new()
            {
                UserData = UserData,
                PanelsData = Items,
                JobOrderData = OrderData,
            };
            _ = window.ShowDialog();
        }
        private bool CanAccessDesign()
        {
            return true;
        }

        private void Approval()
        {
            JobOrderServices.Approvals(OrderData, ViewData);
        }
        private bool CanAccessApproval()
        {
            return true;
        }

        private void Production()
        {
            JobOrderServices.Production(OrderData, ViewData);
        }
        private bool CanAccessProduction()
        {
            return true;
        }

        private void Inspection()
        {
            JobOrderServices.Inspection(OrderData, ViewData);
        }
        private bool CanAccessInspection()
        {
            return true;
        }

        private void Closing()
        {
            JobOrderServices.Closing(OrderData, ViewData);
        }
        private bool CanAccessClosing()
        {
            return true;
        }


        private void Hold()
        {
            Windows.JobOrderWindows.Panels_Hold_Windows.PanelsWindow window =
                new()
                {
                    UserData = UserData,
                    PanelsData = Items,
                    JobOrderData = OrderData,
                };
            _ = window.ShowDialog();
        }
        private bool CanAccessHold()
        {
            return true;
        }

        private void Cancel()
        {
            Windows.JobOrderWindows.Panels_Cancelling_Windows.PanelsWindow window =
                new()
                {
                    UserData = UserData,
                    PanelsData = Items,
                    JobOrderData = OrderData,
                };
            _ = window.ShowDialog();
        }
        private bool CanAccessCancel()
        {
            return true;
        }
    }
}