using Microsoft.Win32;

using ProjectsNow.Data.References;
using ProjectsNow.Data.Suppliers;
using ProjectsNow.Windows.MessageWindows;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace ProjectsNow.ProgrammingTools
{
    public static class Data
    {
        //public static string InsertReferences()
        //{
        //    OpenFileDialog path = new OpenFileDialog() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
        //    _ = path.ShowDialog();

        //    string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
        //                      $@"Extended Properties='Excel 8.0;HDR=Yes;'";

        //    try
        //    {
        //        DataTable excelData = new DataTable();
        //        using (OleDbConnection connection = new OleDbConnection(filePath))
        //        {
        //            connection.Open();
        //            OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [References$]", connection); //here we read data from sheet1  
        //            _ = oleAdpt.Fill(excelData);
        //        }

        //        if (excelData.Rows.Count == 0)
        //        {
        //            _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
        //            return null;
        //        }


        //        List<Reference> excelList = new List<Reference>();
        //        for (int i = 0; i < excelData.Rows.Count; i++)
        //        {
        //            Reference excelRow = new Reference();
        //            excelRow.Category = excelData.Rows[i]["Category"].ToString();
        //            excelRow.Code = excelData.Rows[i]["Code"].ToString();
        //            excelRow.Description = excelData.Rows[i]["Description"].ToString();
        //            excelRow.Article1 = excelData.Rows[i]["Article1"].ToString();
        //            excelRow.Article2 = excelData.Rows[i]["Article2"].ToString();
        //            excelRow.Brand = excelData.Rows[i]["Brand"].ToString();
        //            excelRow.Remarks = excelData.Rows[i]["Remarks"].ToString();
        //            excelRow.Cost = Convert.ToDecimal(excelData.Rows[i]["Cost"]);
        //            excelRow.Discount = Convert.ToDecimal(excelData.Rows[i]["Discount"]);
        //            excelRow.Unit = excelData.Rows[i]["Unit"].ToString();
        //            excelRow.Type = excelData.Rows[i]["Type"].ToString();
        //            //excelRow.Hide = excelData.Rows[i]["Hide"].ToString();
        //            excelRow.ItemType = excelData.Rows[i]["ItemType"].ToString();
        //            excelRow.SearchKeys = excelData.Rows[i]["SearchKeys"].ToString();
        //            excelRow.EditableCost = excelData.Rows[i]["EditableCost"].ToString() == "True" ? true : false;
        //            excelList.Add(excelRow);
        //        }

        //        string query = "INSERT INTO [Reference].[References] " +
        //                       "(" +
        //                       " [Category] " +
        //                       ",[Code] " +
        //                       ",[Description] " +
        //                       ",[Article1] " +
        //                       ",[Article2] " +
        //                       ",[Brand] " +
        //                       ",[Remarks] " +
        //                       ",[Cost] " +
        //                       ",[Discount] " +
        //                       ",[Unit] " +
        //                       ",[Type] " +
        //                       ",[ItemType] " +
        //                       ",[SearchKeys] " +
        //                       ",[EditableCost]" +
        //                       ") " +
        //                       "VALUES ";
        //        foreach (Reference row in excelList)
        //        {
        //            query += "(" +
        //                      $" '{row.Category}' " +
        //                      $",'{row.Code}' " +
        //                      $",'{row.Description}' " +
        //                      $",{(row.Article1 == "NULL" ? "NULL" : $"'{row.Article1}'")} " +
        //                      $",{(row.Article2 == "NULL" ? "NULL" : $"'{row.Article2}'")} " +
        //                      $",{(row.Brand == "NULL" ? "NULL" : $"'{row.Brand}'")} " +
        //                      $",{(row.Remarks == "NULL" ? "NULL" : $"'{row.Remarks}'")} " +
        //                      $", {row.Cost} " +
        //                      $", {row.Discount} " +
        //                      $",{(row.Unit == "NULL" ? "NULL" : $"'{row.Unit}'")} " +
        //                      $",{(row.Type == "NULL" ? "NULL" : $"'{row.Type}'")} " +
        //                      $",{(row.ItemType == "NULL" ? "NULL" : $"'{row.ItemType}'")} " +
        //                      $",{(row.SearchKeys == "NULL" ? "NULL" : $"'{row.SearchKeys}'")} " +
        //                      $",{(row.EditableCost == true ? '1' : '0')}" +
        //                      ") ,";
        //        }

        //        return query;

        //    }
        //    catch (Exception exception)
        //    {
        //        _ = MessageWindow.Show("Error", exception.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
        //        return null;
        //    }
        //}
        //public static string InsertSuppliers()
        //{
        //    OpenFileDialog path = new OpenFileDialog() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
        //    _ = path.ShowDialog();

        //    string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
        //                      $@"Extended Properties='Excel 8.0;HDR=Yes;'";

        //    try
        //    {
        //        DataTable excelData = new DataTable();
        //        using (OleDbConnection connection = new OleDbConnection(filePath))
        //        {
        //            connection.Open();
        //            OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Suppliers$]", connection); //here we read data from sheet1  
        //            _ = oleAdpt.Fill(excelData);
        //        }

        //        if (excelData.Rows.Count == 0)
        //        {
        //            _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
        //            return null;
        //        }


        //        List<Supplier> excelList = new List<Supplier>();
        //        for (int i = 0; i < excelData.Rows.Count; i++)
        //        {
        //            Supplier excelRow = new Supplier();
        //            excelRow.Name = excelData.Rows[i]["Name"].ToString();
        //            excelRow.Code = excelData.Rows[i]["Code"].ToString();
        //            excelList.Add(excelRow);
        //        }

        //        string query = "INSERT INTO [Store].[Suppliers] " +
        //                       "([Name] " +
        //                       ",[Code] " +
        //                       ",[BankID]) " +
        //                       "VALUES ";
        //        foreach (Supplier row in excelList)
        //        {
        //            query += "(" +
        //                      $" '{row.Name}' " +
        //                      $",'{row.Code}' " +
        //                      ") ,";
        //        }

        //        return query;
        //    }
        //    catch (Exception exception)
        //    {
        //        _ = MessageWindow.Show("Error", exception.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
        //        return null;
        //    }
        //}
        //public static string InsertInvoices()
        //{
        //    OpenFileDialog path = new OpenFileDialog() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
        //    _ = path.ShowDialog();

        //    string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
        //                      $@"Extended Properties='Excel 8.0;HDR=Yes;'";

        //    try
        //    {
        //        DataTable excelData = new DataTable();
        //        using (OleDbConnection connection = new OleDbConnection(filePath))
        //        {
        //            connection.Open();
        //            OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Invoices$]", connection); //here we read data from sheet1  
        //            _ = oleAdpt.Fill(excelData);
        //        }

        //        if (excelData.Rows.Count == 0)
        //        {
        //            _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
        //            return null;
        //        }

        //        string query = "INSERT INTO [Store].[Invoices] " +
        //                       "([SupplierID] " +
        //                       ",[JobOrderID] " +
        //                       ",[Date] " +
        //                       ",[Number]) " +
        //                       "VALUES ";

        //        for (int i = 0; i < excelData.Rows.Count; i++)
        //        {
        //            query += $"(" +
        //                     $"(Select SupplierID From [Store].[Suppliers] Where Name ='{excelData.Rows[i]["Supplier"]}') " +
        //                     $",0 " +
        //                     $",'2022-01-01' " +
        //                     $",'{excelData.Rows[i]["Invoice"]}' " +
        //                     $") ,";
        //        }

        //        return query;
        //    }
        //    catch (Exception exception)
        //    {
        //        _ = MessageWindow.Show("Error", exception.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
        //        return null;
        //    }
        //}
        //public static string InsertStock()
        //{
        //    OpenFileDialog path = new OpenFileDialog() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm" };
        //    _ = path.ShowDialog();

        //    string filePath = $@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={path.FileName};" +
        //                      $@"Extended Properties='Excel 8.0;HDR=Yes;'";

        //    try
        //    {
        //        DataTable excelData = new DataTable();
        //        using (OleDbConnection connection = new OleDbConnection(filePath))
        //        {
        //            connection.Open();
        //            OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Stock$]", connection); //here we read data from sheet1  
        //            _ = oleAdpt.Fill(excelData);
        //        }

        //        if (excelData.Rows.Count == 0)
        //        {
        //            _ = MessageWindow.Show("Data Error", "No data!", MessageWindowButton.OK, MessageWindowImage.Warning);
        //            return null;
        //        }

        //        string query = "INSERT INTO [Store].[Transactions] " +
        //                       "(" +
        //                       " [JobOrderID] " +
        //                       ",[InvoiceID] " +
        //                       ",[Code] " +
        //                       ",[Description] " +
        //                       ",[Unit] " +
        //                       ",[Qty] " +
        //                       ",[Cost] " +
        //                       ",[Date] " +
        //                       ",[Type] " +
        //                       ",[Source] " +
        //                       ",[VAT] " +
        //                       ",[OriginalInvoiceID]" +
        //                       ") " +
        //                       "VALUES";

        //        for (int i = 0; i < excelData.Rows.Count; i++)
        //        {
        //            query += $"(" +
        //                     $" 0" +
        //                     $",(Select ID From [Store].[Invoices] Where Number ='{excelData.Rows[i]["Invoice"]}') " +
        //                     $",'{excelData.Rows[i]["Code"]}' " +
        //                     $",'{excelData.Rows[i]["Description"]}' " +
        //                     $",'{excelData.Rows[i]["Unit"]}' " +
        //                     $",{Convert.ToDouble(excelData.Rows[i]["Qty"])} " +
        //                     $",{Convert.ToDouble(excelData.Rows[i]["Cost"])} " +
        //                     $",'2022-01-01' " +
        //                     $",'Stock' " +
        //                     $",'New' " +
        //                     $",{Convert.ToDouble(excelData.Rows[i]["VAT"])} " +
        //                     $",(Select ID From [Store].[Invoices] Where Number ='{excelData.Rows[i]["Invoice"]}') " +
        //                     $") ,";
        //        }

        //        return query;
        //    }
        //    catch (Exception exception)
        //    {
        //        _ = MessageWindow.Show("Error", exception.Message, MessageWindowButton.OK, MessageWindowImage.Warning);
        //        return null;
        //    }
        //}
    }
}
