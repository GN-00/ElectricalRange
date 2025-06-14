using Dapper;

using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;

namespace ProjectsNow.Controllers
{
    public static class QuotationOptionPanelController
    {
        public static ObservableCollection<QuotationOptionPanel> GetPanels(SqlConnection connection, string optionsIDs)
        {
            string query = $"Select * From [Quotation].[OptionsPanels] " +
                           $"Where OptionID In ({optionsIDs}) " +
                           $"Order By PanelSN";

            ObservableCollection<QuotationOptionPanel> records = new(connection.Query<QuotationOptionPanel>(query));
            return records;
        }

        public static ObservableCollection<QuotationOptionPanel> GetQuotationPanels(SqlConnection connection, int quotationID, int optionID)
        {
            string query = $"Select *  From [Quotation].[OptionsPanels(CanSelect)] " +
                           $"Where QuotationID = {quotationID} And OptionID = {optionID}" +
                           $"Order By PanelSN";

            ObservableCollection<QuotationOptionPanel> records = new(connection.Query<QuotationOptionPanel>(query));
            return records;
        }
    }
}
