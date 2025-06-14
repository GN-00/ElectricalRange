using ProjectsNow.Views;
using ProjectsNow.Windows.PrintWindows;

using System.Collections.Generic;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace ProjectsNow.Printing
{
    public class Print
    {
        public static FixedDocument GetFixedDocument(List<FrameworkElement> Pages)
        {
            if (System.IO.File.Exists("PrintPreview.xps"))
            { System.IO.File.Delete("PrintPreview.xps"); }

            XpsDocument xpsDocument = new("PrintPreview.xps", System.IO.FileAccess.ReadWrite);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            FixedDocument fixedDocument = new();
            PageContent pageContent;
            FixedPage fixedPage;

            if (Pages.Count == 0)
            {
                xpsDocument.Close();
                MessageView.Show("Pages", "No data!", MessageViewButton.OK, MessageViewImage.Information);
                return null;
            }

            for (int i = 0; i < Pages.Count; i++)
            {
                fixedPage = new FixedPage
                { Width = Pages[i].Width, Height = Pages[i].Height, Margin = new Thickness(-10) };
                _ = fixedPage.Children.Add(Pages[i]);
                pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                _ = fixedDocument.Pages.Add(pageContent);
            }

            writer.Write(fixedDocument);
            xpsDocument.Close();
            return fixedDocument;
        }

        public static void PrintPreview(FrameworkElement Page, string documentName, IView checkPoint)
        {
            if (System.IO.File.Exists("PrintPreview.xps"))
            { System.IO.File.Delete("PrintPreview.xps"); }

            XpsDocument xpsDocument = new("PrintPreview.xps", System.IO.FileAccess.ReadWrite);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            FixedDocument fixedDocument = new();
            PageContent pageContent;
            FixedPage fixedPage;

            fixedPage = new FixedPage
            { Width = Page.Width, Height = Page.Height };
            _ = fixedPage.Children.Add(Page);

            pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);

            _ = fixedDocument.Pages.Add(pageContent);

            writer.Write(fixedDocument);

            if (checkPoint == null)
            {
                PrintWindow printWindow = new(fixedDocument, documentName);
                _ = printWindow.ShowDialog();
            }
            else
            {
                Navigation.To(new PrintView(fixedDocument, documentName), checkPoint);
            }

            xpsDocument.Close();
        }
        public static void PrintPreview(FrameworkElement Page, string documentName, PageOrientation pageOrientation, IView checkPoint)
        {
            if (System.IO.File.Exists("PrintPreview.xps"))
            { System.IO.File.Delete("PrintPreview.xps"); }

            XpsDocument xpsDocument = new("PrintPreview.xps", System.IO.FileAccess.ReadWrite);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            FixedDocument fixedDocument = new();
            PageContent pageContent;
            FixedPage fixedPage;

            fixedPage = new FixedPage
            { Width = Page.Width, Height = Page.Height };
            _ = fixedPage.Children.Add(Page);

            pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);

            _ = fixedDocument.Pages.Add(pageContent);

            writer.Write(fixedDocument);

            if (checkPoint == null)
            {
                PrintWindow printWindow = new(fixedDocument, documentName, pageOrientation);
                _ = printWindow.ShowDialog();
            }
            else
            {
                Navigation.To(new PrintView(fixedDocument, documentName, pageOrientation), checkPoint);
            }

            xpsDocument.Close();
        }
        public static void PrintPreview(List<FrameworkElement> Pages, string documentName, IView checkPoint = null)
        {
            if (System.IO.File.Exists("PrintPreview.xps"))
            { System.IO.File.Delete("PrintPreview.xps"); }

            XpsDocument xpsDocument = new("PrintPreview.xps", System.IO.FileAccess.ReadWrite);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            FixedDocument fixedDocument = new();
            PageContent pageContent;
            FixedPage fixedPage;

            for (int i = 0; i < Pages.Count; i++)
            {
                fixedPage = new FixedPage
                { Width = Pages[i].Width, Height = Pages[i].Height, Margin = new Thickness(-10) };
                _ = fixedPage.Children.Add(Pages[i]);
                pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                _ = fixedDocument.Pages.Add(pageContent);
            }

            writer.Write(fixedDocument);

            if (checkPoint == null)
            {
                PrintWindow printWindow = new(fixedDocument, documentName);
                _ = printWindow.ShowDialog();
            }
            else
            {
                Navigation.To(new PrintView(fixedDocument, documentName), checkPoint);
            }

            xpsDocument.Close();
        }
        public static void PrintPreview(List<FrameworkElement> Pages, string documentName, PageOrientation pageOrientation, IView checkPoint = null)
        {
            if (System.IO.File.Exists("PrintPreview.xps"))
            { System.IO.File.Delete("PrintPreview.xps"); }

            XpsDocument xpsDocument = new("PrintPreview.xps", System.IO.FileAccess.ReadWrite);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            FixedDocument fixedDocument = new();
            PageContent pageContent;
            FixedPage fixedPage;

            for (int i = 0; i < Pages.Count; i++)
            {
                fixedPage = new FixedPage
                { Width = Pages[i].Width, Height = Pages[i].Height, Margin = new Thickness(-10) };
                _ = fixedPage.Children.Add(Pages[i]);
                pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                _ = fixedDocument.Pages.Add(pageContent);
            }

            writer.Write(fixedDocument);

            if (checkPoint == null)
            {
                PrintWindow printWindow = new(fixedDocument, documentName, pageOrientation);
                _ = printWindow.ShowDialog();
            }
            else
            {
                Navigation.To(new PrintView(fixedDocument, documentName, pageOrientation), checkPoint);
            }
            xpsDocument.Close();
        }
    }
}

