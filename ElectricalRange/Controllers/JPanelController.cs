using Dapper;

using ProjectsNow.Data.JobOrders;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class JPanelController
    {
        public static ObservableCollection<JPanel> GetJobOrderPanels(this SqlConnection connection, int jobOrderID)
        {
            string query = $"Select * From [JobOrder].[Panels(View)] " +
                           $"Where JobOrderID = {jobOrderID} " +
                           $"Order By PanelSN ";
            return new ObservableCollection<JPanel>(connection.Query<JPanel>(query));
        }
    }
}
