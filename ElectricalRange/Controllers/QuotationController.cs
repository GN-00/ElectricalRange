using Dapper;

using ProjectsNow.Data.Quotations;
using ProjectsNow.Enums;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Controllers
{
    public static class QuotationController
    {
        public static int NewQuotationNumber(SqlConnection connection, int year)
        {
            int newQuotationNumber = connection.Query<Quotation>($"Select MAX(QuotationNumber) as QuotationNumber From [Quotation].[Quotations] Where QuotationYear = {year}").FirstOrDefault().QuotationNumber;
            return ++newQuotationNumber;
        }
        public static Quotation GetRevision(SqlConnection connection, int inquiryID, int revise)
        {
            string query = $"Select Quotations.QuotationID, Quotations.InquiryID, QuotationCode, QuotationStatus, " +                                //Quotations
                           $"SubmitDate, QuotationNumber, QuotationRevise, QuotationReviseDate, QuotationYear, QuotationMonth, " +                   //Quotations
                           $"PowerVoltage, Phase, Frequency, NetworkSystem, " +                                                                      //Quotations
                           $"ControlVoltage, TinPlating, NeutralSize, EarthSize EarthingSystem, VAT, Discount, BillOfPriceNote, " +                  //Quotations
                           $"Inquiries.CustomerID, Inquiries.ConsultantID, EstimationID, Inquiries.SalesmanID, " +                                   //Inquiries
                           $"Priority, ProjectName, DuoDate, RegisterDate, DeliveryCondition, " +                                                    //Inquiries
                           $"RegisterCode, RegisterYear, RegisterMonth, RegisterNumber, " +                                                          //Inquiries
                           $"Name as EstimationName, " +                                                                                         //Users
                           $"CustomerName, " +                                                                                                       //Customers
                           $"QuotationCost " +                                                                                                       //QuotationsCost
                           $"From [Quotation].[Quotations] " +
                           $"LEFT OUTER JOIN [Inquiry].[Inquiries] On Quotations.InquiryID = Inquiries.InquiryID " +
                           $"LEFT OUTER JOIN [Customer].[Customers] On Inquiries.CustomerID = Customers.CustomerID " +
                           $"LEFT OUTER JOIN [User].[Users] On Users.Id = Inquiries.EstimationID " +
                           $"LEFT OUTER JOIN [Quotation].[QuotationsCost] On [Quotations].QuotationID = [QuotationsCost].QuotationID " +
                           $"Where QuotationRevise = {revise} And QuotationStatus = 'Revision' And Quotations.InquiryID = {inquiryID} ";

            Quotation quotation = connection.QueryFirstOrDefault<Quotation>(query);
            return quotation;
        }
        public static ObservableCollection<Quotation> GetQuotations(SqlConnection connection, int year, Statuses quotationStatus)
        {
            string query = $"Select * From [Quotation].[Quotations(View)] " +
                           $"Where QuotationYear = {year} ";

            if (quotationStatus == Statuses.All)
            {
                query += $"And QuotationStatus != 'Revision' ";
            }
            else
            {
                query += $"And QuotationStatus = '{quotationStatus}' ";
            }

            query += $"Order By QuotationYear Desc, QuotationNumber Desc";

            ObservableCollection<Quotation> quotations = new(connection.Query<Quotation>(query));
            return quotations;
        }
        public static ObservableCollection<Quotation> UserQuotations(SqlConnection connection, int userID, int year, Statuses quotationStatus)
        {
            string query = $"Select * From [Quotation].[Quotations(View)] " +
                           $"Where QuotationYear = {year} And EstimationID = {userID} ";

            if (quotationStatus == Statuses.All)
            {
                query += $"And QuotationStatus != 'Revision' ";
            }
            else
            {
                query += $"And QuotationStatus = '{quotationStatus}' ";
            }

            query += $"Order By QuotationYear Desc, QuotationNumber Desc";

            ObservableCollection<Quotation> quotations = new(connection.Query<Quotation>(query));
            return quotations;
        }
    }
}
