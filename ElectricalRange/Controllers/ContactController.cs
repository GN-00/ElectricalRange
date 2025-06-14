using Dapper;

using ProjectsNow.Data.Customers;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Controllers
{
    public static class ContactController
    {
        public static ObservableCollection<Contact> GetContacts(SqlConnection connection)
        {
            string query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
                           $"CustomerName " +
                           $"From [Customer].[Contacts]" +
                           $"LEFT OUTER JOIN [Customer].[Customers] On Customers.CustomerID = Contacts.CustomerID " +
                           $"Order By ContactName";
            ObservableCollection<Contact> records = new(connection.Query<Contact>(query));
            return records;
        }
        public static ObservableCollection<Contact> GetCustomerContacts(SqlConnection connection, int customerID)
        {
            string query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
                        $"CustomerName " +
                        $"From [Customer].[Contacts]" +
                        $"LEFT OUTER JOIN [Customer].[Customers] On Customers.CustomerID = Contacts.CustomerID " +
                        $"Where Contacts.CustomerID = {customerID} " +
                        $"Order By ContactName";
            ObservableCollection<Contact> records = new(connection.Query<Contact>(query));
            return records;
        }
        public static ObservableCollection<Contact> GetCustomerRemainingContacts(SqlConnection connection, int customerID, int[] contactsIDs)
        {
            string query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
                        $"CustomerName " +
                        $"From [Customer].[Contacts] " +
                        $"LEFT OUTER JOIN [Customer].[Customers] On Customers.CustomerID = Contacts.CustomerID " +
                        $"Where Contacts.CustomerID = {customerID} ";


            if (contactsIDs.Count() != 0)
            {
                query += $"And ContactID Not in ";
                for (int i = 0; i < contactsIDs.Count(); i++)
                {
                    query += (i == 0 ? "(" : ", ") + contactsIDs[i];
                }
                query += $") Order By ContactName";
            }
            else
            {
                query += $"Order By ContactName";
            }

            ObservableCollection<Contact> records = new(connection.Query<Contact>(query));
            return records;
        }
        public static ObservableCollection<Contact> GetProjectContacts(SqlConnection connection, int inquiryID)
        {
            string query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
                           $"ProjectsContacts.Attention " +
                           $"FROM [Inquiry].[ProjectsContacts] " +
                           $"LEFT OUTER JOIN [Customer].[Contacts] On Contacts.ContactID = ProjectsContacts.ContactID " +
                           $"Where InquiryID = {inquiryID} " +
                           $"Order By ContactName ";
            ObservableCollection<Contact> records = new(connection.Query<Contact>(query));
            return records;
        }

        public static Contact GetProjectAttention(SqlConnection connection, int inquiryID)
        {
            string query = $"Select Contacts.ContactID, Contacts.CustomerID, Contacts.ContactName, Contacts.Address, Contacts.Mobile, Contacts.Email, Contacts.Job, Contacts.Note, " +
                        $"Attention " +
                        $"FROM [Inquiry].[ProjectsContacts] " +
                        $"LEFT OUTER JOIN [Customer].[Contacts] On Contacts.ContactID = ProjectsContacts.ContactID " +
                        $"Where InquiryID = {inquiryID} And Attention = 'True'";

            Contact record = connection.Query<Contact>(query).FirstOrDefault();

            return record;
        }

        public static bool AbilityToDelete(this Contact contact, SqlConnection connection)
        {
            string query = $"Select ContactID " +
                           $"From [Inquiry].[ProjectsContacts] " +
                           $"Where ContactID = {contact.ContactID} ";
            List<Contact> records = connection.Query<Contact>(query).ToList();
            return records.Count == 0;
        }
    }
}