using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.DataInput;
using ProjectsNow.Enums;
using ProjectsNow.Windows.MessageWindows;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class PrismaGWindow : Window
    {
        private class IP
        {
            public int Value { get; set; }
        }
        private class Module
        {
            public int Value { get; set; }
            public int IP { get; set; }
            public string Installation { get; set; }
        }
        private class Installation
        {
            public string Type { get; set; }
            public int IP { get; set; }
        }

        private int oldIP = 55;
        private readonly List<IP> iPs = new()
        {
            new IP() { Value = 30 },
            new IP() { Value = 40 },
            new IP() { Value = 41 },
            new IP() { Value = 43 },
            new IP() { Value = 55},
        };
        private readonly List<Module> modules = new()
        {
            new Module(){ Value = 6, IP = 30, Installation = "Wall Mounted" },
            new Module(){ Value = 9, IP = 30, Installation = "Wall Mounted" },
            new Module(){ Value = 12, IP = 30, Installation = "Wall Mounted" },
            new Module(){ Value = 15, IP = 30, Installation = "Wall Mounted" },
            new Module(){ Value = 18, IP = 30, Installation = "Wall Mounted" },
            new Module(){ Value = 21, IP = 30, Installation = "Wall Mounted" },
            new Module(){ Value = 24, IP = 30, Installation = "Wall Mounted" },
            new Module(){ Value = 25, IP = 30, Installation = "Wall Mounted" },
            new Module(){ Value = 27, IP = 30, Installation = "Wall Mounted" },

            new Module(){ Value = 27, IP = 30, Installation = "Floor Standing" },
            new Module(){ Value = 30, IP = 30, Installation = "Floor Standing" },
            new Module(){ Value = 33, IP = 30, Installation = "Floor Standing" },
            new Module(){ Value = 36, IP = 30, Installation = "Floor Standing" },

            new Module(){ Value = 7, IP = 55, Installation = "Wall Mounted" },
            new Module(){ Value = 11, IP = 55, Installation = "Wall Mounted" },
            new Module(){ Value = 15, IP = 55, Installation = "Wall Mounted" },
            new Module(){ Value = 19, IP = 55, Installation = "Wall Mounted" },
            new Module(){ Value = 23, IP = 55, Installation = "Wall Mounted" },
            new Module(){ Value = 27, IP = 55, Installation = "Wall Mounted" },
            new Module(){ Value = 33, IP = 55, Installation = "Wall Mounted" },
        };
        private readonly List<Installation> installations = new()
        {
            new Installation(){ Type = "Wall Mounted" , IP = 55 },
            new Installation(){ Type = "Floor Standing", IP = 30 },
        };


        public User UserData { get; set; }
        public QPanel PanelData { get; set; }
        public ObservableCollection<QItem> Items { get; set; }
        public PrismaGWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnclosureIP.ItemsSource = iPs;
            EnclosureIP.SelectedItem = iPs.Where(i => i.Value == 30).First();
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
        private void IP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnclosureIP.SelectedItem is IP iP)
            {
                if (iP.Value == 55)
                {
                    EnclosureInstallation.ItemsSource = installations.Where(i => i.IP == 55);
                    EnclosureInstallation.SelectedIndex = 0;
                    Size.ItemsSource = modules.Where(s => s.Installation == "Wall Mounted" && s.IP == iP.Value);
                    Size.SelectedIndex = 0;

                    if (Size.SelectedItem is Module module)
                    {
                        if (module.Value == 7)
                        {
                            DuctStackPanel.Visibility = Visibility.Collapsed;
                            Duct.Text = "0";
                        }
                        else
                        {
                            DuctStackPanel.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    DuctStackPanel.Visibility = Visibility.Visible;
                    EnclosureInstallation.ItemsSource = installations;
                    if (oldIP == 55)
                    {
                        EnclosureInstallation.SelectedIndex = 0;
                        if (EnclosureInstallation.SelectedItem is Installation installation)
                        {
                            Size.ItemsSource = modules.Where(s => s.Installation == installation.Type && s.IP < 55);
                            Size.SelectedIndex = 0;
                        }
                    }
                }

                oldIP = iP.Value;
            }
        }
        private void Installation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnclosureIP.SelectedItem is IP iP)
            {
                if (iP.Value == 55)
                {
                    return;
                }

                if (EnclosureInstallation.SelectedItem is Installation installation)
                {
                    if (installation.Type == "Wall Mounted")
                    {
                        Size.ItemsSource = modules.Where(s => s.Installation == "Wall Mounted" && s.IP < 55);
                    }
                    else
                    {
                        Size.ItemsSource = modules.Where(s => s.Installation == "Floor Standing" && s.IP < 55);
                    }

                    Size.SelectedIndex = 0;
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {

            int qty;
            string ip = (EnclosureIP.SelectedItem as IP).Value.ToString();
            string module = (Size.SelectedItem as Module).Value.ToString();
            string installation = (EnclosureInstallation.SelectedItem as Installation).Type;

            List<QItem> itemsData = new();
            List<QItem> references = new();
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query;
                if (Enclosure.Text != "0")
                {
                    int width = 60;
                    qty = int.Parse(Enclosure.Text);
                    query = $"Select * From [Reference].[PrismaGView] " +
                            $"Where GroupName = 'Enclosure' And Width ='{width}' And Module ='{module}' And Installation ='{installation}' And IP <= {ip}";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    qty = int.Parse(Enclosure.Text);
                    query = $"Select * From [Reference].[PrismaGView] " +
                            $"Where GroupName = 'Door' And Width ='{width}' And Module ='{module}' And Installation ='{installation}' And IP <= {ip}";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    qty = int.Parse(Enclosure.Text);
                    query = $"Select * From [Reference].[PrismaGView] " +
                            $"Where GroupName = 'Gland Plate' And Width ='{width}' And Installation ='{installation}' And IP <= {ip}";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    qty = int.Parse(Enclosure.Text);
                    query = $"Select * From [Reference].[PrismaGView] " +
                            $"Where GroupName = 'Gasket' And IP = {ip}";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    if (qty > 1)
                    {
                        query = $"Select * From [Reference].[PrismaGView] " +
                                $"Where GroupName = 'Join' And IP = {(ip == "55" ? 55 : 30)}";
                        references = connection.Query<QItem>(query).ToList();
                        foreach (QItem reference in references)
                        { reference.ItemQty = qty - 1; itemsData.Add(reference); }
                    }
                }

                if (Duct.Text != "0")
                {
                    int width = 30;
                    qty = int.Parse(Enclosure.Text);
                    query = $"Select * From [Reference].[PrismaGView] " +
                            $"Where GroupName = 'Enclosure' And Width ='{width}' And Module ='{module}' And Installation ='{installation}' And IP <= {ip}";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    qty = int.Parse(Enclosure.Text);
                    query = $"Select * From [Reference].[PrismaGView] " +
                            $"Where GroupName = 'Door' And Width ='{width}' And Module ='{module}' And Installation ='{installation}' And IP <= {ip}";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    qty = int.Parse(Enclosure.Text);
                    query = $"Select * From [Reference].[PrismaGView] " +
                            $"Where GroupName = 'Gland Plate' And Width ='{width}' And Installation ='{installation}' And IP <= {ip}";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    qty = int.Parse(Enclosure.Text);
                    query = $"Select * From [Reference].[PrismaGView] " +
                            $"Where GroupName = 'Gasket' And IP = {ip}";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }

                query = $"Select * From [Reference].[PrismaGView] " +
                        $"Where GroupName = 'Canopy' And IP <= {ip} And EnclosureQty ='{Enclosure.Text}' And DuctQty = '{Duct.Text}'";
                references = connection.Query<QItem>(query).ToList();
                if (references.Count == 0 && int.Parse(ip) > 40)
                {
                    _ = MessageWindow.Show("Canopy",
                        "There are no standard solutions for canopy!"
                        + "\nStandard:"
                        + "\n Enclosure."
                        + "\n Enclosure + Duct."
                        + "\n Enclosure + Enclosure."
                        + "\n Enclosure + 2Ducts."
                        + "\n 2Enclosures + Duct."
                        + "", MessageWindowButton.OK, MessageWindowImage.Warning);
                }
                foreach (QItem reference in references)
                { reference.ItemQty = 1; itemsData.Add(reference); }

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

                PanelData.EnclosureType = "Prisma Set G";
                PanelData.EnclosureHeight = int.Parse(module) * 5;
                PanelData.EnclosureWidth = (int.Parse(Duct.Text) * 30) + (int.Parse(Enclosure.Text) * 60);
                PanelData.EnclosureDepth = 25;

                PanelData.EnclosureMetalType = "Steel";
                PanelData.EnclosureColor = "9001";
                PanelData.EnclosureIP = ip;
                PanelData.EnclosureForm = "1";
                PanelData.EnclosureLocation = "Indoor";
                PanelData.EnclosureInstallation = installation;
                PanelData.EnclosureFunctional = "With";

                PanelData.EnclosureName = $"Prisma Set G IP{PanelData.EnclosureIP} {PanelData.EnclosureLocation}";
                _ = connection.Update(PanelData);
            }

            Close();

        }

        private void Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnclosureIP.SelectedItem is IP iP)
            {
                if (iP.Value == 55)
                {
                    if (Size.SelectedItem is Module module)
                    {
                        if (module.Value == 7)
                        {
                            DuctStackPanel.Visibility = Visibility.Collapsed;
                            Duct.Text = "0";
                        }
                        else
                        {
                            DuctStackPanel.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    DuctStackPanel.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
