using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Views;
using ProjectsNow.Views.QuotationsViews;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ProjectsNow.Services
{
    public static class QuotationServices
    {
        public static User UserData => Navigation.UserData;
        internal static string UpdateCode(string quotationCode, string estimatorCode)
        {
            //ER-220/QA/7/2022/R00
            return quotationCode.Remove(7, 2).Insert(7, estimatorCode);
        }

        internal static bool CanAccessInfo(Quotation quotation)
        {
            if (quotation == null)
                return false;

            return true;
        }

        internal static void Terms(Quotation quotation, IView checkPoint)
        {
            Navigation.To(new TermsView(quotation), checkPoint);
        }
        internal static bool CanAccessTerms(Quotation quotation)
        {
            if (quotation == null)
                return false;

            return true;
        }

        internal static void Panels(Quotation quotation, ObservableCollection<Quotation> quotations, IView checkPoint)
        {
            Navigation.To(new PanelsView(quotation), checkPoint);
        }
        internal static bool CanAccessPanels(Quotation quotation)
        {
            if (quotation == null)
                return false;

            return true;
        }

        internal static void Revisions(Quotation quotation, IView checkPoint)
        {
            Navigation.OpenPopup(new RevisionsView(quotation, checkPoint), PlacementMode.Center, false);
        }
        internal static bool CanAccessRevisions(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (quotation.QuotationRevise == 0)
                return false;

            return true;
        }

        internal static void GetItems(Quotation quotation, IView checkPoint)
        {
            //int numberRow = 45;
            ObservableCollection<QItem> items;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Quotation].[QuotationsItemsList(View)] " +
                               $"Where QuotationID = {quotation.QuotationID} " +
                               $"Order By Code";

                items = new ObservableCollection<QItem>(connection.Query<QItem>(query));
            }

            for (int i = 1; i <= items.Count; i++)
            {
                items[i - 1].ItemSort = i;
            }

            double pagesNumber = items.Count / 45d;
            if (pagesNumber - Math.Truncate(pagesNumber) != 0)
            {
                pagesNumber = Math.Truncate(pagesNumber) + 1;
            }

            List<FrameworkElement> elements = new();
            if (pagesNumber != 0)
            {
                for (int i = 1; i <= pagesNumber; i++)
                {
                    Printing.QuotationsItems quotationsItems = new() { QuotationData = quotation, Items = items.Where(p => p.ItemSort > ((i - 1) * 45) && p.ItemSort <= (i * 45)).ToList(), Page = i, Pages = Convert.ToInt32(pagesNumber) };
                    elements.Add(quotationsItems);
                }

                Printing.Print.PrintPreview(elements, $"Quotation {quotation.QuotationCode} Items", checkPoint);
            }
            else
            {
                _ = MessageWindow.Show("Items", "There is no items!!", MessageWindowButton.OK, MessageWindowImage.Warning);
            }
        }
        internal static bool CanAccessGetItems(Quotation quotation)
        {
            if (quotation == null)
                return false;

            return true;
        }

        internal static void CostSheet(Quotation quotation, IView checkPoint)
        {
            Navigation.OpenPopup(new SelectCostSheetView(quotation, checkPoint), PlacementMode.MousePoint, false);
        }
        internal static bool CanAccessCostSheet(Quotation quotation)
        {
            if (quotation == null)
                return false;

            return true;
        }

        internal static void Prices(Quotation quotation)
        {
            Navigation.OpenPopup(new PriceView(quotation), PlacementMode.Center, true);
        }
        internal static bool CanAccessPrices(Quotation quotation)
        {
            //Can't apply discount for price adjusting
            if (quotation == null)
                return false;

            return false;
        }

        internal static void PriceNote(Quotation quotation)
        {
            //BillOfPriceNoteWindow billOfPriceNoteWindow = new BillOfPriceNoteWindow()
            //{
            //    QuotationData = quotation,
            //};

            //billOfPriceNoteWindow.ShowDialog();
            Navigation.OpenPopup(new NoteView(quotation), PlacementMode.Center, true);
        }
        internal static bool CanAccessPriceNote(Quotation quotation)
        {
            if (quotation == null)
                return false;

            return true;
        }

        internal static void Print(Quotation quotation, IView checkPoint)
        {
            Navigation.To(new PrintQuotationView(quotation), checkPoint);
        }
        internal static void Options(Quotation quotation, IView checkPoint)
        {
            Navigation.To(new OptionsView(quotation), checkPoint);
        }

        internal static void Submit(Quotation quotation, ObservableCollection<Quotation> quotations, Statuses status)
        {
            MessageBoxResult result = MessageWindow.Show("Submit", $"Are you sure to submit \nQ.Code: {quotation.QuotationCode}", MessageWindowButton.YesNo, MessageWindowImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    quotation.QuotationStatus = Statuses.Submitted.ToString();
                    quotation.SubmitDate = DateTime.Now;

                    _ = connection.Update(quotation);
                }

                if (status != Statuses.All)
                {
                    _ = quotations.Remove(quotation);
                }
            }
        }
        internal static bool CanAccessSubmit(Quotation quotation)
        {
            if (quotation == null)
                return false;

            if (quotation.QuotationStatus != Statuses.Running.ToString())
                return false;

            if (!UserData.ModifyQuotations)
                return false;

            if (UserData.EmployeeId != quotation.EstimationID)
                return false;

            return true;
        }

        internal static void Restart(Quotation quotation, ObservableCollection<Quotation> quotations, Statuses status)
        {
            MessageBoxResult result = MessageWindow.Show("Submit", $"Are you sure to work \nQ.Code: {quotation.QuotationCode}", MessageWindowButton.YesNo, MessageWindowImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    quotation.QuotationStatus = Statuses.Running.ToString();
                    _ = connection.Update(quotation);
                }

                if (status != Statuses.All)
                {
                    _ = quotations.Remove(quotation);
                }
            }
        }
        internal static void Hold(Quotation quotation, ObservableCollection<Quotation> quotations, Statuses status)
        {
            MessageBoxResult result = MessageWindow.Show("Submit", $"Are you sure to hold \nQ.Code: {quotation.QuotationCode}", MessageWindowButton.YesNo, MessageWindowImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    quotation.QuotationStatus = Statuses.Hold.ToString();
                    _ = connection.Update(quotation);
                }
                if (status != Statuses.All)
                {
                    _ = quotations.Remove(quotation);
                }
            }
        }
        internal static void Cancel(Quotation quotation, ObservableCollection<Quotation> quotations, Statuses status)
        {
            MessageBoxResult result = MessageWindow.Show("Submit", $"Are you sure to cancel \nQ.Code: {quotation.QuotationCode}", MessageWindowButton.YesNo, MessageWindowImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    quotation.QuotationStatus = Statuses.Canceled.ToString();
                    _ = connection.Update(quotation);
                }
                if (status != Statuses.All)
                {
                    _ = quotations.Remove(quotation);
                }
            }
        }
        internal static void Revise(Quotation quotation)
        {
            string query;
            JobOrder checkJobOrder;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[JobOrders] " +
                        $"Where QuotationID = {quotation.QuotationID}";
                checkJobOrder = connection.QueryFirstOrDefault<JobOrder>(query);
            }

            if (checkJobOrder != null)
            {
                MessageWindow.Show(
                    "Job Order",
                    $"Can't revise this quotation!\nBecause it has Job Order:\n{checkJobOrder}",
                    MessageWindowButton.OK,
                    MessageWindowImage.Error);

                return;
            }

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                Quotation reviseQuotationData = new();
                reviseQuotationData.Update(quotation);

                quotation.QuotationStatus = Statuses.Revision.ToString();
                query = $"Update [Quotation].[Quotations] Set QuotationStatus = '{quotation.QuotationStatus}' Where QuotationID = {quotation.QuotationID}";
                _ = connection.Execute(query);

                query = $"Select (Max(QuotationRevise) + 1) as QuotationRevise From [Quotation].[Quotations] " +
                        $"Where QuotationYear = {quotation.QuotationYear} " +
                        $"And QuotationMonth = {quotation.QuotationMonth} " +
                        $"And QuotationNumber = {quotation.QuotationNumber}";

                reviseQuotationData.SubmitDate = null;
                reviseQuotationData.QuotationReviseDate = DateTime.Now;
                reviseQuotationData.QuotationRevise = connection.QueryFirstOrDefault<int>(query);
                reviseQuotationData.QuotationStatus = Statuses.Running.ToString();

                int length = reviseQuotationData.QuotationCode.LastIndexOf("/") + 1;
                reviseQuotationData.QuotationCode = $"{reviseQuotationData.QuotationCode.Substring(0, 17)}R{reviseQuotationData.QuotationRevise:00}";

                _ = connection.Insert(reviseQuotationData);

                query = $"Select * From [Quotation].[Terms&Conditions] Where QuotationID = {quotation.QuotationID} Order By Sort";
                List<Term> terms = connection.Query<Term>(query).ToList();
                foreach (Term term in terms)
                {
                    term.QuotationID = reviseQuotationData.QuotationID;
                }
                _ = connection.Insert(terms);

                List<QPanel> panels = QPanelController.QuotationPanels(connection, quotation.QuotationID).ToList();
                if (panels.Count != 0)
                {
                    foreach (QPanel panelData in panels)
                    {
                        panelData.RevisePanelID = panelData.PanelID;
                        panelData.QuotationID = reviseQuotationData.QuotationID;

                        _ = connection.Insert(panelData);

                        List<QItem> items = QItemController.PanelItems(connection, panelData.RevisePanelID);

                        if (items.Count != 0)
                        {
                            string insert = $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                                         $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort) " +
                                         $"Values " +
                                         $"({panelData.PanelID}, @Article1, @Article2, @Category, @Code, @Description, @Unit, @ItemQty, @Brand, @Remarks, @ItemCost, @ItemDiscount, @ItemTable, @ItemType, @ItemSort)";
                            _ = connection.Execute(insert, items);
                        }
                    }
                }
            }
        }

        internal static void Reset(Quotation quotation)
        {
            MessageBoxResult result = MessageWindow.Show($"Reset",
                                                         $"Are you soure want to reset\nQ.Code {quotation.QuotationCode}?",
                                                         MessageWindowButton.YesNo,
                                                         MessageWindowImage.Question);

            if (result == MessageBoxResult.No)
                return;

            string query;
            JobOrder checkJobOrder;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[JobOrders] " +
                        $"Where QuotationID = {quotation.QuotationID}";
                checkJobOrder = connection.QueryFirstOrDefault<JobOrder>(query);
            }

            if (checkJobOrder != null)
            {
                MessageWindow.Show(
                        "Job Order",
                        $"Can't revise this quotation!\nBecause it has Job Order:\n{checkJobOrder}",
                        MessageWindowButton.OK,
                        MessageWindowImage.Error);

                return;
            }

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID in " +
                        $"(Select PanelID From [Quotation].[QuotationsPanels] Where QuotationID = {quotation.QuotationID})";
                _ = connection.Execute(query);

                query = $"Delete From [Quotation].[QuotationsPanels] Where QuotationID = {quotation.QuotationID}";
                _ = connection.Execute(query);

                query = $"Delete From [Quotation].[Terms&Conditions] Where QuotationID = {quotation.QuotationID}";
                _ = connection.Execute(query);

                query = $"Select * From [Quotation].[Quotations(View)] Where InquiryID = {quotation.InquiryID} And QuotationRevise = {quotation.QuotationRevise - 1} ";
                Quotation revisionQuotationData = connection.QueryFirstOrDefault<Quotation>(query);

                query = $"Select * From [Quotation].[Terms&Conditions] Where QuotationID = {revisionQuotationData.QuotationID} Order By Sort";
                List<Term> terms = connection.Query<Term>(query).ToList();
                foreach (Term term in terms)
                {
                    term.QuotationID = quotation.QuotationID;
                }
                _ = connection.Insert(terms);

                List<QPanel> panels = QPanelController.QuotationPanels(connection, revisionQuotationData.QuotationID).ToList();
                if (panels.Count != 0)
                {
                    foreach (QPanel panelData in panels)
                    {
                        panelData.RevisePanelID = panelData.PanelID;
                        panelData.QuotationID = quotation.QuotationID;

                        _ = connection.Insert(panelData);

                        List<QItem> items = QItemController.PanelItems(connection, panelData.RevisePanelID);

                        if (items.Count != 0)
                        {
                            string insert = $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                                            $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort) " +
                                            $"Values " +
                                            $"({panelData.PanelID}, @Article1, @Article2, @Category, @Code, @Description, @Unit, @ItemQty, @Brand, @Remarks, @ItemCost, @ItemDiscount, @ItemTable, @ItemType, @ItemSort)";
                            _ = connection.Execute(insert, items);
                        }
                    }
                }
            }
        }
    }
}
