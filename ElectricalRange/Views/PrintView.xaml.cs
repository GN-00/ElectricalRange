using System;
using System.Windows;
using System.Printing;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProjectsNow.Views
{
    public partial class PrintView : UserControl, IView
    {
        public static readonly double cm = 37.7952755905512;
        public static readonly double A4Width = 29.7 * cm;
        public static readonly double A4Height = 21 * cm;

        private string _documentName { get; set; }

        private FixedDocument _fixedDocument;
        private PageOrientation _pageOrientation = PageOrientation.Portrait;
        public PrintView()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 15;
        }
        public PrintView(FixedDocument fixedDocument, string documentName)
        {
            _documentName = documentName;
            _fixedDocument = fixedDocument;
            InitializeComponent();
            documentViewer.Document = fixedDocument;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 15;
        }
        public PrintView(FixedDocument fixedDocument, string documentName, PageOrientation pageOrientation)
        {
            _pageOrientation = pageOrientation;
            _documentName = documentName;
            _fixedDocument = fixedDocument;
            InitializeComponent();
            documentViewer.Document = fixedDocument;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 15;
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintInfo printInfo = new()
            {
                DocumentName = _documentName,
                DocumentViewer = documentViewer,
                FixedDocument = _fixedDocument,
                PageOrientation = _pageOrientation,
            };
            Print_Document(printInfo);
        }

        public class PrintInfo
        {
            public string DocumentName { get; set; }
            public DocumentViewer DocumentViewer { get; set; }
            public FixedDocument FixedDocument { get; set; }
            public PageOrientation PageOrientation { get; set; }
        }
        public static void Print_Document(PrintInfo printInfo)
        {
            try
            {
                PrintDialog printDialog = new();
                printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;
                printDialog.PrintTicket.PageOrientation = printInfo.PageOrientation;
                printDialog.PrintTicket.PageScalingFactor = 100;
                printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
                printDialog.PrintTicket.PageBorderless = PageBorderless.None;
                printDialog.PageRangeSelection = PageRangeSelection.AllPages;
                printDialog.UserPageRangeEnabled = true;
                printDialog.CurrentPageEnabled = true;

                printDialog.PrintTicket.PageResolution = new PageResolution(PageQualitativeResolution.High);
                if (printDialog.ShowDialog() == true)
                {
                    if (printDialog.PrintQueue.FullName == "Microsoft XPS Document Writer")
                    {
                        _ = MessageView.Show("\"Microsoft XPS Document Writer\" not supported!!");
                        return;
                    }

                    DocumentPaginator paginator = printInfo.FixedDocument.DocumentPaginator;
                    if (printDialog.PageRangeSelection == PageRangeSelection.UserPages)
                    {
                        paginator = new PageRangeDocumentPaginator(
                                         printInfo.FixedDocument.DocumentPaginator,
                                         printDialog.PageRange);
                    }
                    else if (printDialog.PageRangeSelection == PageRangeSelection.CurrentPage)
                    {
                        paginator = new CurrentPageDocumentPaginator(
                                         printInfo.FixedDocument.DocumentPaginator,
                                         printInfo.DocumentViewer.MasterPageNumber);
                    }
                    printInfo.FixedDocument.PrintTicket = printDialog.PrintTicket;
                    printInfo.DocumentName = printInfo.DocumentName.Replace("/", "-");
                    printDialog.PrintDocument(paginator, printInfo.DocumentName);
                }
            }
            catch (Exception e)
            {
                _ = MessageBox.Show(e.Message);
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintInfo printInfo = new()
            {
                DocumentName = _documentName,
                DocumentViewer = documentViewer,
                FixedDocument = _fixedDocument,
                PageOrientation = _pageOrientation,
            };
            Print_Document(printInfo);
        }

        public class PageRangeDocumentPaginator : DocumentPaginator
        {
            private int _startIndex;
            private int _endIndex;
            private DocumentPaginator _paginator;
            public PageRangeDocumentPaginator(DocumentPaginator paginator, PageRange pageRange)
            {
                _startIndex = pageRange.PageFrom - 1;
                _endIndex = pageRange.PageTo - 1;
                _paginator = paginator;

                // Adjust the _endIndex
                _endIndex = Math.Min(_endIndex, _paginator.PageCount - 1);
            }
            public override DocumentPage GetPage(int pageNumber)
            {
                // Just return the page from the original
                // paginator by using the "startIndex"
                return _paginator.GetPage(pageNumber + _startIndex);
            }

            public override bool IsPageCountValid => true;

            public override int PageCount
            {
                get
                {
                    if (_startIndex > _paginator.PageCount - 1)
                    {
                        return 0;
                    }

                    if (_startIndex > _endIndex)
                    {
                        return 0;
                    }

                    return _endIndex - _startIndex + 1;
                }
            }

            public override Size PageSize
            {
                get { return _paginator.PageSize; }
                set { _paginator.PageSize = value; }
            }

            public override IDocumentPaginatorSource Source => _paginator.Source;
        }
        public class CurrentPageDocumentPaginator : DocumentPaginator
        {
            private int _startIndex;
            private int _endIndex;
            private DocumentPaginator _paginator;
            public CurrentPageDocumentPaginator(DocumentPaginator paginator, int masterPageNumber)
            {
                _startIndex = masterPageNumber - 1;
                _endIndex = masterPageNumber - 1;
                _paginator = paginator;

                // Adjust the _endIndex
                _endIndex = Math.Min(_endIndex, _paginator.PageCount - 1);
            }
            public override DocumentPage GetPage(int pageNumber)
            {
                // Just return the page from the original
                // paginator by using the "startIndex"
                return _paginator.GetPage(pageNumber + _startIndex);
            }

            public override bool IsPageCountValid => true;

            public override int PageCount
            {
                get
                {
                    if (_startIndex > _paginator.PageCount - 1)
                    {
                        return 0;
                    }

                    if (_startIndex > _endIndex)
                    {
                        return 0;
                    }

                    return _endIndex - _startIndex + 1;
                }
            }

            public override Size PageSize
            {
                get { return _paginator.PageSize; }
                set { _paginator.PageSize = value; }
            }

            public override IDocumentPaginatorSource Source => _paginator.Source;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Print_Click(null, null);
            }
        }
    }
}
