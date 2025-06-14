using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.DataInput;
using ProjectsNow.Enums;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class SFWindow : Window
    {
        public User UserData { get; set; }
        public QPanel PanelData { get; set; }
        public ObservableCollection<QItem> Items { get; set; }
        public SFWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Input.ArrowsOnly(e);
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Input.IntOnly(e, 2);
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0";
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            int qty;
            string depth = PPDepth.Text;
            List<QItem> itemsData = new();
            List<QItem> references = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                int cubics = 0;
                string query;
                if (SF60.Text != "0")
                {
                    int width = 60;
                    qty = int.Parse(SF60.Text);
                    cubics += qty;
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Upright' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Base' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Door' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Rear' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Accessories'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }
                if (SF80.Text != "0")
                {
                    int width = 80;
                    qty = int.Parse(SF80.Text);
                    cubics += qty;
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Upright' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Base' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Door' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Rear' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Accessories'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }
                if (SF100.Text != "0")
                {
                    int width = 100;
                    qty = int.Parse(SF100.Text);
                    cubics += qty;
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Upright' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Base' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Door' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Rear' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Accessories'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }
                if (SF120.Text != "0")
                {
                    int width = 120;
                    qty = int.Parse(SF120.Text);
                    cubics += qty;
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Upright' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Base' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Door' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Rear' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Accessories'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }

                query = $"Select * From [Reference].[SFView] " +
                        $"Where GroupName = 'Side' And Depth ='{depth}'";
                references = connection.Query<QItem>(query).ToList();
                foreach (QItem reference in references)
                {
                    itemsData.Add(reference);
                }

                if (cubics > 1)
                {
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Coupling'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty = cubics - 1; itemsData.Add(reference); }
                }



                List<QItem> smartItemsData = itemsData.GroupBy(g => new { g.Article1, g.Category, g.Code, g.Description, g.Unit, g.Brand, g.ItemType, g.ItemCost, g.ItemDiscount, g.ItemSort })
                    .Select(g => new QItem()
                    {
                        Article1 = g.Key.Article1,
                        Category = g.Key.Category,
                        Code = g.Key.Code,
                        Description = g.Key.Description,
                        Unit = g.Key.Unit,
                        Brand = g.Key.Brand,
                        ItemType = g.Key.ItemType,
                        ItemQty = g.Sum(q => q.ItemQty),
                        ItemCost = g.Key.ItemCost,
                        ItemDiscount = g.Key.ItemDiscount,
                        ItemSort = g.Key.ItemSort,
                    })
                    .ToList();
                itemsData = smartItemsData.OrderBy(i => i.ItemSort).ThenBy(i => i.Code).ToList();
                int newDetailsCount = itemsData.Where(i => i.ItemTable == Tables.Details.ToString()).Count();
                int newEnclosureCount = itemsData.Where(i => i.ItemTable == Tables.Enclosure.ToString()).Count();

                List<QItem> smartEnclosure = new();
                smartEnclosure.AddRange(Items.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString() && i.ItemTable == Tables.Details.ToString()));
                smartEnclosure.AddRange(Items.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString() && i.ItemTable == Tables.Enclosure.ToString()));
                int oldDetailsCount = smartEnclosure.Where(i => i.ItemTable == Tables.Details.ToString()).Count();
                int oldEnclosureCount = smartEnclosure.Where(i => i.ItemTable == Tables.Enclosure.ToString()).Count();

                query = $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {PanelData.PanelID} And SelectionGroup = '{SelectionGroups.SmartEnclosure}';";
                query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {oldDetailsCount} + {newDetailsCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Details}'); ";
                query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {oldEnclosureCount} + {newEnclosureCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Enclosure}'); ";

                foreach (QItem item in smartEnclosure)
                {
                        _ = Items.Remove(item);
                }

                foreach (QItem item in Items.Where(x => x.ItemTable == Tables.Details.ToString()))
                {
                    item.ItemSort = item.ItemSort - oldDetailsCount + newDetailsCount;
                }

                foreach (QItem item in Items.Where(x => x.ItemTable == Tables.Enclosure.ToString()))
                {
                    item.ItemSort = item.ItemSort - oldEnclosureCount + newEnclosureCount;
                }

                foreach (QItem itemData in itemsData)
                {
                    itemData.ItemTable = Tables.Enclosure.ToString();
                    if (itemData.ItemType == null)
                    {
                        itemData.ItemType = ItemTypes.Standard.ToString();
                    }

                    itemData.SelectionGroup = SelectionGroups.SmartEnclosure.ToString();
                    query += $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                             $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort, SelectionGroup) " +
                             $"Values ";
                    itemData.ItemSort = itemsData.IndexOf(itemData);
                    query += $"({PanelData.PanelID}, '{itemData.Article1}', '{itemData.Article2}', '{itemData.Category}', '{itemData.Code}', '{itemData.Description}', '{itemData.Unit}', '{itemData.ItemQty}', '{itemData.Brand}', '{itemData.Remarks}', {itemData.ItemCost}, {itemData.ItemDiscount}, '{itemData.ItemTable}', '{itemData.ItemType}', {itemData.ItemSort}, '{itemData.SelectionGroup}'); ";

                    Items.Add(itemData);
                }

                _ = connection.Execute(query);

                PanelData.EnclosureType = "Universal or Equivalent";
                PanelData.EnclosureHeight = 200;
                PanelData.EnclosureWidth = (int.Parse(SF60.Text) * 60) + (int.Parse(SF80.Text) * 80) + (int.Parse(SF100.Text) * 100) + (int.Parse(SF120.Text) * 120);
                PanelData.EnclosureDepth = int.Parse(depth);

                PanelData.EnclosureMetalType = "Steel";
                PanelData.EnclosureColor = "7035";
                PanelData.EnclosureIP = "55";
                PanelData.EnclosureForm = "1";
                PanelData.EnclosureLocation = "Indoor";
                PanelData.EnclosureInstallation = "Floor Standing";
                PanelData.EnclosureFunctional = "With";

                PanelData.EnclosureName = $"Universal SF IP{PanelData.EnclosureIP} {PanelData.EnclosureLocation}";
                _ = connection.Update(PanelData);
            }

            Close();

        }
    }
}
