using Dapper;

using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class EnclosuresWindow : Window
    {
        public User UserData { get; set; }
        public QPanel PanelData { get; set; }
        public ObservableCollection<QItem> Items { get; set; }

        private List<Enclosure> enclosures;
        public EnclosuresWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                enclosures = connection.Query<Enclosure>($"Select Name As Name From [Reference].[GroupProperties] Where Category ='Enclosure' Order By Sort").ToList();
            }
            EnclosuresList.ItemsSource = enclosures;
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (EnclosuresList.SelectedItem is Enclosure enclosureData)
            {
                if (enclosureData.Name == "NSYCRN" || enclosureData.Name == "NSYSM" || enclosureData.Name == "Disbo")
                {
                    SelectionWindow selectionWindow = new()
                    {
                        UserData = UserData,
                        GroupName = enclosureData.Name,
                        PanelData = PanelData,
                        Items = Items,
                    };
                    Close();
                    _ = selectionWindow.ShowDialog();
                }
                else if (enclosureData.Name == "NSYSF")
                {
                    SFWindow SFWindow = new()
                    {
                        UserData = UserData,
                        PanelData = PanelData,
                        Items = Items,
                    };
                    Close();
                    _ = SFWindow.ShowDialog();
                }
                else if (enclosureData.Name == "Prisma P")
                {
                    PrismaPWindow prismaPWindow = new()
                    {
                        UserData = UserData,
                        PanelData = PanelData,
                        Items = Items,
                    };
                    Close();
                    _ = prismaPWindow.ShowDialog();
                }
                else if (enclosureData.Name == "Prisma G")
                {
                    PrismaGWindow prismaGWindow = new()
                    {
                        UserData = UserData,
                        PanelData = PanelData,
                        Items = Items,
                    };
                    Close();
                    _ = prismaGWindow.ShowDialog();
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
