using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

using ProjectsNow.Controllers;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.References;
using ProjectsNow.Enums;
using ProjectsNow.Views;

using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Data.Library
{
    public class Group : Base
    {
        private string _image;
        private string _family;
        private string _label;
        private string _description;
        private List<Category> _categories;
        private int _qty = 1;

        public string Id { get; set; }
        public int PanelId { get; set; }
        public QPanel PanelData { get; set; }
        public int Sort { get; set; }
        public int Qty
        {
            get => _qty;
            set
            {
                if (value < 1)
                    SetValue(ref _qty, 1);
                else
                    SetValue(ref _qty, value);
            }
        }
        public string Image
        {
            get => _image;
            set => SetValue(ref _image, value)
                  .UpdateProperties(this, nameof(GroupImage));
        }
        public string Family
        {
            get => _family;
            set => SetValue(ref _family, value);
        }
        public string Label
        {
            get => _label;
            set => SetValue(ref _label, value);
        }
        public string Description
        {
            get => _description;
            set => SetValue(ref _description, value);
        }
        public int CubicalNumber { get; set; }
        public BitmapImage GroupImage
        {
            get => new(new Uri($"/Images/GroupIcons/{Image}.png", UriKind.Relative));
        }

        public string GroupView { get; set; }
        public Selection SelectionData { get; set; }
        public List<Category> Categories
        {
            get => _categories;
            set => SetValue(ref _categories, value);
        }
        public List<ItemType> ItemsTypes { get; set; }
        public ObservableCollection<QItem> Items { get; set; }
        public bool IsReset { get; private set; } = false;

        public void GetData(Selection selection = null)
        {
            if (selection != null)
            {
                Id = selection.GroupId;
                SelectionData = selection;
            }

            if (Id == null)
                return;

            string query;
            using SqlConnection connection = new(Database.PSConnectionString);

            //Get Group
            Group group;
            query = $"Select * From [Estimator].[Groups] Where Id = '{Id}'";
            group = connection.QueryFirst<Group>(query);
            Id = group.Id;
            Label = group.Label;
            Description = group.Description;
            Family = group.Family;
            Image = group.Image;
            GroupView = group.GroupView;

            //Get Categories
            query = $"Select * From [Estimator].[Categories] Where GroupId = '{Id}' " +
                    $"Order By Sort";
            Categories = connection.Query<Category>(query).ToList();

            //Get ItemsTypes
            query = $"Select * From [Estimator].[ItemsTypes] Where GroupId = '{Id}'";
            ItemsTypes = connection.Query<ItemType>(query).ToList();
            foreach (var itemType in ItemsTypes)
            {
                query = $"Select Linked As Name, i, j From [Estimator].[ItemsTypesLinks] " +
                        $"Where GroupId = '{Id}' And ItemTypeId = '{itemType.Id}' ";
                itemType.LinkedProperties = connection.Query<Linked>(query).ToList();
            }

            //Get Properties
            int i;
            int j;
            foreach (var category in Categories)
            {
                //i = category.Sort;
                query = $"Select * From [Estimator].[Properties] " +
                        $"Where GroupId = '{Id}' And CategoryId = '{category.Id}' " +
                        $"Order By i, j";
                category.Properties = connection.Query<Property>(query).ToList();

                foreach (var property in category.Properties)
                {
                    i = property.i;
                    j = property.j;
                    query = $"Select Linked As Name, i, j From [Estimator].[PropertiesLinks] " +
                            $"Where GroupId = '{Id}' " +
                            $"And CategoryId = '{category.Id}' " +
                            $"And PropertyId = '{property.Id}' ";
                    property.LinkedProperties = [.. connection.Query<Linked>(query)];

                    query = $"Select Property{i}{j} From [Estimator].[ItemsProperties] " + /*{GroupView}*/
                            $"Where GroupId = '{Id}' " +
                            $"And Property{i}{j} Not Like '%,%' " +
                            $"And Property{i}{j} Is Not NULL " +
                            $"And Property{i}{j} <> ' ' " +
                            $"Group By Property{i}{j}, Property{i}{j}Sort " +
                            $"Order By Property{i}{j}Sort";

                    property.Values = connection.Query<string>(query).ToList();

                    property.PropertyListChange += UpdateData;
                }
            }

            if (IsReset)
                return;

            if (SelectionData != null)
            {
                foreach (var category in Categories)
                {
                    foreach (var property in category.Properties)
                    {

                        if (typeof(Selection).GetProperty($"Property{property.i}{property.j}").GetValue(SelectionData) is string value)
                            property.Selected = value;
                    }
                }
            }
        }

        public void UpdateData(List<Linked> linkedProperties)
        {
            int i;
            int j;
            string value;
            string query;
            string selection = "";
            using SqlConnection connection = new(Database.PSConnectionString);
            foreach (var link in linkedProperties)
            {
                i = link.i;
                j = link.j;

                selection = "";
                var filters = Categories[i - 1].Properties[j - 1].LinkedProperties;

                if (filters.Count == 0)
                    continue;

                foreach (var filter in filters)
                {
                    value = Categories[filter.i - 1].Properties[filter.j - 1].Selected;
                    if (value != null)
                        selection += $"Property{filter.i}{filter.j} Like '%{value}%' And ";
                }

                selection = selection[..^5];

                query = $"Select Property{i}{j} From [Estimator].[ItemsProperties] " + /*{GroupView}*/
                        $"Where  GroupId = '{Id}' " +
                        $"And {selection} " +
                        $"And Property{i}{j} Not Like '%,%' " +
                        $"And Property{i}{j} Is Not NULL " +
                        $"And Property{i}{j} <> ' ' " +
                        $"Group By Property{i}{j}, Property{i}{j}Sort " +
                        $"Order By Property{i}{j}Sort";

                Categories[i - 1].Properties[j - 1].Values = connection.Query<string>(query).ToList();
            }
        }

        public void Selecting()
        {
            bool isReady = true;
            string message = "Please Select:";
            Navigation.ClosePopup(); //For Testing Only

            SelectionData ??= new()
            {
                GroupId = Id,
                Qty = Qty,
            };

            //if (SelectionData.GroupId != "Setlak Enclosure")
            //    return;

            foreach (Category category in Categories)
            {
                foreach (Property property in category.Properties)
                {
                    if (property.Selected == null)
                    {
                        message += $"\n* {property.Id}";
                        isReady = false;
                    }
                    else
                    {
                        typeof(Selection).GetProperty($"Property{property.i}{property.j}").SetValue(SelectionData, property.Selected);
                    }
                }
            }

            if (!isReady)
            {
                MessageView.Show("Error", message, MessageViewButton.OK, MessageViewImage.Warning);
                return;
            }

            bool isCalculation = ItemsTypes.Any(x => x.Type == "Calculation");
            if (isCalculation)
            {
                int sum = 0;
                foreach (Category category in Categories)
                {
                    foreach (Property property in category.Properties.Where(x => x.Type == "Calculation"))
                    {
                        sum += int.Parse(property.Selected);
                    }
                }

                if (sum == 0)
                {
                    MessageView.Show("Error", "Can't Calculate = 0", MessageViewButton.OK, MessageViewImage.Warning);
                    return;
                }
            }

            int i;
            int j;
            string value;
            string query;
            int detailsCounter;
            int enclosureCounter;
            int accessoriesCounter;

            int newDetailsCounter;
            int newEnclosureCounter;
            int newAccessoriesCounter;

            ObservableCollection<QItem> newItems = new();

            using SqlConnection connection = new(Database.ConnectionString);
            using SqlConnection psConnection = new(Database.PSConnectionString);

            //if (SelectionData.Id != 0)
            //{
            //    query = $"Delete From {DesignItem.Table} Where SelectionId = {SelectionData.Id}";
            //    _ = connection.Execute(query);
            //    _ = connection.Update(SelectionData);

            //    var deleteItems = new List<DesignItem>(Items.Where(x => x.SelectionId == SelectionData.Id));
            //    groupNumber = deleteItems.First().LabelNumber;
            //    foreach (var item in deleteItems)
            //    {
            //        Items.Remove(item);
            //    }
            //}
            //else
            //{
            //    var groupItems = Items.Where(x => x.LabelName == Label);
            //    if (groupItems.Any())
            //        groupNumber = groupItems.Max(i => i.LabelNumber) + 1;

            //    _ = connection.Insert(SelectionData);
            //}

            foreach (var type in ItemsTypes)
            {
                List<DesignItem> items;

                //if (type.LinkedProperties.Count == 0)
                //    continue;

                if (type.Type == "Calculation")
                {
                    query = $"Select * From [Estimator].[CalculationItems] " +
                            $"Where GroupId = '{Id}' " +
                            $"And ItemType = '{type.Id}'";

                    List<CalculationItem> calculationItems = [.. psConnection.Query<CalculationItem>(query)];

                    items = [];
                    DesignItem item = new DesignItem()
                    {
                        Code = calculationItems.FirstOrDefault().Code,
                        Description = calculationItems.FirstOrDefault().Description,
                        ItemTable = calculationItems.FirstOrDefault().ItemTable,
                    };

                    foreach (var calculationItem in calculationItems)
                    {
                        if (calculationItem.Condition == null)
                        {
                            if (calculationItem.Value == null)
                                item.Qty += Convert.ToDouble((string)SelectionData.GetType().GetProperty($"Property{calculationItem.i}{calculationItem.j}").GetValue(SelectionData));
                            else
                                item.Qty += Convert.ToDouble(calculationItem.Value);
                        }
                        else
                        {
                            if (calculationItem.ConditionValue == (string)SelectionData.GetType().GetProperty($"Property{calculationItem.n}{calculationItem.m}").GetValue(SelectionData))
                            {
                                if (calculationItem.Value == null)
                                    item.Qty += Convert.ToDouble((string)SelectionData.GetType().GetProperty($"Property{calculationItem.i}{calculationItem.j}").GetValue(SelectionData));
                                else
                                    item.Qty += Convert.ToDouble(calculationItem.Value);
                            }
                        }
                    }

                    items.Add(item);

                }
                else
                {
                    string selection = "";
                    foreach (var link in type.LinkedProperties)
                    {
                        i = link.i;
                        j = link.j;

                        value = Categories[i - 1].Properties[j - 1].Selected;
                        if (value != null)
                            selection += $"Property{i}{j} Like '%{value}%' And ";
                    }

                    selection = selection[..^5];

                    query = $"Select * From [Estimator].[ItemsProperties] " +
                            $"Where {selection} " +
                            $"And Code Is Not NULL " +
                            $"And ItemType = '{type.Id}' " +
                            $"And GroupId = '{Id}'";

                    items = [.. psConnection.Query<DesignItem>(query)];
                }


                if (items != null)
                {
                    QItem itemToAdd;
                    Reference reference;
                    foreach (var item in items)
                    {
                        if (item.Code == null)
                            continue;

                        query = $"Select * From [Reference].[References(View)] Where Code = '{item.Code}'";
                        reference = connection.QueryFirstOrDefault<Reference>(query);

                        if (reference == null)
                            continue;

                        itemToAdd = new QItem()
                        {
                            PanelID = PanelId,
                            Article1 = reference.Article1,
                            Article2 = reference.Article2,
                            Category = reference.Category,
                            Code = reference.Code,
                            Description = reference.Description,
                            Unit = reference.Unit,
                            ItemQty = (decimal)item.Qty,
                            Brand = reference.Brand,
                            Remarks = reference.Remarks,
                            ItemCost = reference.Cost,
                            ItemDiscount = reference.Discount,
                            ItemType = reference.ItemType,
                            ItemTable = item.ItemTable,
                        };

                        if (Family == "Enclosure")
                            itemToAdd.SelectionGroup = SelectionGroups.SmartEnclosure.ToString();

                        newItems.Add(itemToAdd);
                    }
                }
            }

            detailsCounter = Items.Where(x => x.ItemTable == Tables.Details.ToString()).Count();
            enclosureCounter = Items.Where(x => x.ItemTable == Tables.Enclosure.ToString()).Count();
            accessoriesCounter = Items.Where(x => x.ItemTable == Tables.Accessories.ToString()).Count();

            newDetailsCounter = newItems.Where(x => x.ItemTable == Tables.Details.ToString()).Count();
            newEnclosureCounter = newItems.Where(x => x.ItemTable == Tables.Enclosure.ToString()).Count();
            newAccessoriesCounter = newItems.Where(x => x.ItemTable == Tables.Accessories.ToString()).Count();

            if (Family == "Enclosure")
            {
                detailsCounter = 0;
                enclosureCounter = 0;
                accessoriesCounter = 0;

                foreach (var item in Items)
                {
                    if (item.ItemTable == Tables.Details.ToString())
                        item.ItemSort += newDetailsCounter;
                    else if (item.ItemTable == Tables.Enclosure.ToString())
                        item.ItemSort += newEnclosureCounter;
                    else
                        item.ItemSort += newAccessoriesCounter;
                }

                QPanelController.UpdateEnclosure(PanelData, this);
            }

            if (Family == "Cubical")
            {
                detailsCounter = 0;
                enclosureCounter = 0;
                accessoriesCounter = 0;

                foreach (var item in Items)
                {
                    if (item.ItemTable == Tables.Details.ToString())
                        item.ItemSort += newDetailsCounter;
                    else if (item.ItemTable == Tables.Enclosure.ToString())
                        item.ItemSort += newEnclosureCounter;
                    else
                        item.ItemSort += newAccessoriesCounter;
                }

                QPanelController.UpdateCubical(PanelData, this);
            }

            foreach (var item in newItems)
            {
                if (item.ItemTable == Tables.Details.ToString())
                    item.ItemSort = ++detailsCounter;
                else if (item.ItemTable == Tables.Enclosure.ToString())
                    item.ItemSort = ++enclosureCounter;
                else
                    item.ItemSort = ++accessoriesCounter;

                Items.Insert(item.ItemSort, item);
                _ = connection.Insert(item);
            }

            detailsCounter = 0;
            enclosureCounter = 0;
            accessoriesCounter = 0;

            foreach (var item in Items)
            {
                if (item.ItemTable == Tables.Details.ToString())
                    item.ItemSort = detailsCounter++;
                else if (item.ItemTable == Tables.Enclosure.ToString())
                    item.ItemSort = enclosureCounter++;
                else
                    item.ItemSort = accessoriesCounter++;
            }

            _ = connection.Update(Items);

            Navigation.ClosePopup();
        }

        public void Delete()
        {
            //string query;
            //using SqlConnection connection = new(Database.PSConnectionString);

            //query = $"Delete From {DesignItem.Table} Where SelectionId = {SelectionData.Id}";
            //_ = connection.Execute(query);

            //var delete = new List<DesignItem>(Items.Where(x => x.SelectionId == SelectionData.Id));
            //foreach (var item in delete)
            //{
            //    Items.Remove(item);
            //}
        }

        public void Reset()
        {
            IsReset = true;

            foreach (var category in Categories)
            {
                foreach (var property in category.Properties)
                {
                    property.PropertyListChange = null;
                    property.Selected = null;
                }
            }

            GetData();
            IsReset = false;
        }

        public async Task CheckCode(ObservableCollection<DesignItem> codes)
        {
            int i;
            int j;
            string value;
            string query;
            using SqlConnection connection = new(Database.PSConnectionString);

            codes.Clear();

            foreach (var type in ItemsTypes)
            {
                string selection = "";
                foreach (var link in type.LinkedProperties)
                {
                    i = link.i;
                    j = link.j;

                    value = Categories[i - 1].Properties[j - 1].Selected;
                    if (value != null)
                        selection += $"Property{i}{j} Like '%{value}%' And ";
                }


                query = $"Select * From [Estimator].[ItemsProperties] ";
                if (selection != "")
                {
                    selection = selection[..^5];
                    query += $"Where {selection} " +
                             $"And Code Is Not NULL " +
                             $"And ItemType = '{type.Id}' " +
                             $"And GroupId = '{Id}'";
                }
                else
                {
                    query += $"Where  Code Is Not NULL " +
                             $"And ItemType = '{type.Id}' " +
                             $"And GroupId = '{Id}'";
                }

                var items = await connection.QueryAsync<DesignItem>(query);
                foreach (var item in items)
                {
                    codes.Add(item);
                }
            }
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
