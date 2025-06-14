using Dapper;

using ProjectsNow.Attributes;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace ProjectsNow.Controllers
{
    public static class JPanelDetailsController
    {
        public static void Update(this JPanelDetails JPanel, QPanel QPanel)
        {
            foreach (PropertyInfo property in typeof(JPanelDetails).GetProperties())
            {
                QPanelProperty attributes = (QPanelProperty)typeof(JPanelDetails).GetProperty(property.Name).GetCustomAttribute(typeof(QPanelProperty));
                if (attributes != null)
                {
                    if (property.SetMethod != null)
                    {
                        property.
                        SetValue(JPanel, typeof(QPanel).GetProperty(property.Name).GetValue(QPanel));
                    }
                }
            }
        }

        public static ObservableCollection<JPanelDetails> GetJobOrderPanels(this SqlConnection connection, int jobOrderID)
        {
            string query = $"Select * From [JobOrder].[PanelsDetails] " +
                           $"Where JobOrderID = {jobOrderID} " +
                           $"Order By PanelSN ";
            System.Collections.Generic.IEnumerable<JPanelDetails> records = connection.Query<JPanelDetails>(query);
            return new ObservableCollection<JPanelDetails>(records);
        }

        public static JPanelDetails GetPanel(this SqlConnection connection, int panelID)
        {
            string query = $"Select * From [JobOrder].[PanelsDetails] " +
                           $"Where PanelID = {panelID} ";
            JPanelDetails record = connection.QueryFirstOrDefault<JPanelDetails>(query);
            return record;
        }
    }
}
