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
    public partial class PrismaPWindow : Window
    {
        public User UserData { get; set; }
        public QPanel PanelData { get; set; }
        public ObservableCollection<QItem> Items { get; set; }

        public PrismaPWindow()
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
            string ip = PPIP.Text;
            string depth = PPDepth.Text;
            List<QItem> itemsData = new();
            List<QItem> references = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                if (PP30.Text != "0")
                {
                    int width = 30;
                    qty = int.Parse(PP30.Text);
                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Frame' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Top' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Door' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Back' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Gland Plate' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Hinged FP' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Sealing Kit' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Screws'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }

                if (PP40.Text != "0")
                {
                    int width = 40;
                    qty = int.Parse(PP40.Text);
                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Frame' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Top' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Door' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Back' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Gland Plate' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Hinged FP' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Sealing Kit' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Screws'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }

                if (PP65.Text != "0")
                {
                    int width = 65;
                    qty = int.Parse(PP65.Text);
                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Frame' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Top' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Door' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Back' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Gland Plate' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Hinged FP' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Sealing Kit' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Screws'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }

                if (PP6515.Text != "0")
                {
                    int width = 70;
                    qty = int.Parse(PP6515.Text);
                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Frame' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Top' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Door' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Back' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Gland Plate' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Hinged FP' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Sealing Kit' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Screws'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }

                if (PP80.Text != "0")
                {
                    int width = 80;
                    qty = int.Parse(PP80.Text);
                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Frame' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Top' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Door' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Back' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Gland Plate' And Width ='{width}' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Hinged FP' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Sealing Kit' And Width ='{width}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Screws'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }

                query = $"Select * From [Reference].[PrismaPView] " +
                        $"Where GroupName = 'Side' And Depth ='{depth}' And IP ='{ip}'";
                references = connection.Query<QItem>(query).ToList();
                foreach (QItem reference in references)
                {
                    itemsData.Add(reference);
                }

                qty = int.Parse(PP30.Text) + int.Parse(PP40.Text) + int.Parse(PP65.Text) + int.Parse(PP6515.Text) + int.Parse(PP80.Text) - 1;
                if (qty > 0)
                {
                    query = $"Select * From [Reference].[PrismaPView] " +
                            $"Where GroupName = 'Side Join' And Depth ='{depth}' And IP ='{ip}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
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

                PanelData.EnclosureType = "Prisma Set P";
                PanelData.EnclosureHeight = 200;
                PanelData.EnclosureWidth = (int.Parse(PP30.Text) * 30) + (int.Parse(PP40.Text) * 40) + (int.Parse(PP65.Text) * 65) + (int.Parse(PP6515.Text) * 80) + (int.Parse(PP80.Text) * 80);
                PanelData.EnclosureDepth = int.Parse(depth);

                PanelData.EnclosureMetalType = "Steel";
                PanelData.EnclosureColor = "9001";
                PanelData.EnclosureIP = ip;
                PanelData.EnclosureForm = "1";
                PanelData.EnclosureLocation = "Indoor";
                PanelData.EnclosureInstallation = "Floor Standing";
                PanelData.EnclosureFunctional = "With";

                PanelData.EnclosureName = $"Prisma Set P IP{PanelData.EnclosureIP} {PanelData.EnclosureLocation}";
                _ = connection.Update(PanelData);
            }

            Close();

        }
    }
}
