using ProjectsNow.Commands;
using ProjectsNow.Data.Library;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectsNow.Views.LibraryViews
{
    internal class SelectItemsViewModel : Group
    {
        private string _Indicator = "-";
        private int _SelectedIndex;
        private QItem _SelectedItem;
        private ObservableCollection<QItem> _Codes = [];

        private ICollectionView _ItemsCollection;
        private bool _isRefreshing;

        public SelectItemsViewModel(string groupId, int panelId, ObservableCollection<QItem> items, Selection selection = null)
        {
            Items = items;
            Id = groupId;
            PanelId = panelId;
            GetData(selection);
            CreateCollectionView();

            SaveCommand = new RelayCommand(Save, CanSave);
            ResetSelectionsCommand = new RelayCommand(ResetSelections, CanResetSelections);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            InputIntCommand = new RelayCommand<KeyEventArgs>(InputInt, CanInputInt);

            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            CopyAllCommand = new RelayCommand(CopySheet, CanCopySheet);
            CopyItemCodeCommand = new RelayCommand<QItem>(CopyItemCode, CanCopyItemCode);
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
        public QItem SelectedItem
        {
            get => _SelectedItem;
            set => SetValue(ref _SelectedItem, value);
        }
        public ObservableCollection<QItem> Codes
        {
            get => _Codes;
            private set
            {
                if (SetValue(ref _Codes, value))
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

        private void CreateCollectionView()
        {
            ItemsCollection = CollectionViewSource.GetDefaultView(Codes);

            //ItemsCollection.Filter = new Predicate<object>(DataFilter);
            ItemsCollection.SortDescriptions.Add(new SortDescription("Sort", ListSortDirection.Ascending));
            ItemsCollection.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));

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

        public RelayCommand SaveCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand CopyAllCommand { get; }
        public RelayCommand<QItem> CopyItemCodeCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand<KeyEventArgs> InputIntCommand { get; }
        public RelayCommand ResetSelectionsCommand { get; }

        public Visibility LoadingIcon { get; set; } = Visibility.Collapsed;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            private set => SetValue(ref _isRefreshing, value);
        }

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


        private void InputInt(KeyEventArgs obj)
        {
            DataInput.Input.IntOnly(obj, 10);
        }

        private bool CanInputInt(KeyEventArgs arg)
        {
            return true;
        }


        private async void Refresh()
        {
            //IsRefreshing = true;

            //await CheckCode(Codes);

            //IsRefreshing = false;
        }
        private bool CanRefresh()
        {
            return true;
        }


        private void CopyItemCode(QItem item)
        {
            Clipboard.SetText(item.Code);
        }
        private bool CanCopyItemCode(QItem item)
        {
            if (item == null)
                return false;

            return true;
        }


        private void CopySheet()
        {
            //string data = "";
            //foreach (QItem item in Codes)
            //    data += $"{item.ItemType} {item.Code}: {item.Description} ({item.Qty}) \n\n";

            //Clipboard.SetText(data);
        }
        private bool CanCopySheet()
        {
            //if (Codes == null)
            //    return false;

            //if (Codes.Count == 0)
            //    return false;

            return true;
        }

    }
}