using Dapper;

using Microsoft.Data.SqlClient;

using ProjectsNow.AttachedProperties;
using ProjectsNow.Commands;
using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.Application;
using ProjectsNow.Data.Quotations;
using ProjectsNow.Data.Users;
using ProjectsNow.Enums;
using ProjectsNow.Printing;
using ProjectsNow.Printing.QuotationPages;

using System.Printing;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

using static ProjectsNow.Views.PrintView;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class PrintQuotationViewModel : ViewModelBase
    {
        private bool _IsCover;
        private bool _IsTerms;
        private bool _IsBill;
        private bool _IsDetails;
        private bool _IsVAT;
        private bool _IsBackground;
        private ImageSource _BackgroundImage;
        private string _TotalPages = "-";

        private double _Zoom;
        PageOrientation _PageOrientationData;
        private FixedDocument _DocumentData;
        private DocumentViewer _DocumentViewerData;

        public PrintQuotationViewModel(Quotation quotation, List<BillPanel> panels)
        {
            UserData = Navigation.UserData;
            QuotationData = quotation;
            PanelsData = panels;

            ReportCommand = new RelayCommand(Report, CanAccessReport);
            ReloadCommand = new RelayCommand(Reload, CanReload);

            Group.Add(CoverPages);
            Group.Add(TermsPages);
            Group.Add(BillPages);
            Group.Add(DetailsPages);
        }

        public double MaxPageHeight => 750;
        public User UserData { get; }
        public Quotation QuotationData { get; }
        public List<BillPanel> PanelsData { get; set; }
        public List<BillItem> ItemsData { get; set; }
        public List<FrameworkElement> Pages { get; } = new List<FrameworkElement>();
        public List<List<FrameworkElement>> Group { get; } = new List<List<FrameworkElement>>();
        public List<FrameworkElement> CoverPages { get; } = new List<FrameworkElement>();
        public List<FrameworkElement> TermsPages { get; } = new List<FrameworkElement>();
        public List<FrameworkElement> BillPages { get; } = new List<FrameworkElement>();
        public List<FrameworkElement> DetailsPages { get; } = new List<FrameworkElement>();

        public bool IsCover
        {
            get => _IsCover;
            set => SetValue(ref _IsCover, value);
        }
        public bool IsTerms
        {
            get => _IsTerms;
            set => SetValue(ref _IsTerms, value);
        }
        public bool IsBill
        {
            get => _IsBill;
            set => SetValue(ref _IsBill, value);
        }
        public bool IsDetails
        {
            get => _IsDetails;
            set => SetValue(ref _IsDetails, value);
        }
        public bool IsVAT
        {
            get => _IsVAT;
            set => SetValue(ref _IsVAT, value);
        }
        public bool IsBackground
        {
            get => _IsBackground;
            set => SetValue(ref _IsBackground, value);
        }
        public ImageSource BackgroundImage
        {
            get => _BackgroundImage;
            set => SetValue(ref _BackgroundImage, value);
        }
        public string TotalPages
        {
            get => _TotalPages;
            set => SetValue(ref _TotalPages, value);
        }


        public double Zoom
        {
            get => _Zoom;
            set => SetValue(ref _Zoom, value);
        }
        public PageOrientation PageOrientationData
        {
            get => _PageOrientationData;
            set => SetValue(ref _PageOrientationData, value);
        }
        public FixedDocument DocumentData
        {
            get => _DocumentData;
            set => SetValue(ref _DocumentData, value);
        }
        public DocumentViewer DocumentViewerData
        {
            get => _DocumentViewerData;
            set => SetValue(ref _DocumentViewerData, value);
        }

        public RelayCommand ReportCommand { get; }
        public RelayCommand ReloadCommand { get; }

        private void Report()
        {
            PrintInfo printInfo = new()
            {
                DocumentName = $"{QuotationData.QuotationCode}",
                DocumentViewer = DocumentViewerData,
                PageOrientation = PageOrientationData,
                FixedDocument = DocumentData,
            };
            PrintView.Print_Document(printInfo);
        }
        private bool CanAccessReport()
        {
            if (DocumentData == null)
                return false;

            return true;
        }

        private void Reload()
        {
            Navigation.OpenLoading(Visibility.Visible, "Loading...");

            Zoom = 100;
            PageOrientationData = PageOrientation.Portrait;
            if (IsBackground)
            {
                if (AppData.UserWatermark == null)
                {
                    BackgroundImage = AppData.CompanyWatermark;
                }
                else
                {
                    BackgroundImage = AppData.UserWatermark;
                }
            }


            if (IsTerms)
            {
                GetTerms();
            }

            if (IsBill)
            {
                GetBill();
            }

            if (IsDetails)
            {
                GetDetails();
            }

            if (IsCover)
            {
                GetCover();
            }

            Pages.Clear();
            int pageNumber = 1;
            int totalPages = CoverPages.Count + TermsPages.Count + BillPages.Count + DetailsPages.Count;
            TotalPages = totalPages.ToString();

            foreach (List<FrameworkElement> list in Group)
            {
                if (list.Count == 0)
                    continue;

                foreach (FrameworkElement element in list)
                {
                    if (IsBackground)
                    {
                        ((PageBase)element).SetBackground(BackgroundImage);
                    }

                    ((PageBase)element).SetPage(pageNumber++, totalPages);
                    Pages.Add(element);
                }
            }

            DocumentData = Print.GetFixedDocument(Pages);
            Navigation.CloseLoading();
        }
        private bool CanReload()
        {
            if (!IsCover && !IsTerms && !IsBill && !IsDetails)
                return false;

            return true;
        }

        private void GetTerms()
        {
            TermsPages.Clear();
            List<Term> terms;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                string query = $"Select * From [Quotation].[Terms&Conditions] " +
                               $"Where IsUsed = 'True' " +
                               $"And QuotationID = {QuotationData.QuotationID} " +
                               $"Order By Sort";
                terms = connection.Query<Term>(query).ToList();
            }

            if (terms.Count == 0)
                return;

            int topicNumber = 0;
            double totalHeight = 0;
            Line line;
            TextBlock textBlock;
            StackPanel content;
            StackPanel stackPanel;

            List<IEnumerable<Term>> termsList = new();

            IEnumerable<Term> ScopeOfSupply = terms.Where(item => item.ConditionType == ConditionTypes.ScopeOfSupply.ToString());
            termsList.Add(ScopeOfSupply);

            IEnumerable<Term> TotalPrice = terms.Where(item => item.ConditionType == ConditionTypes.TotalPrice.ToString());
            termsList.Add(TotalPrice);

            IEnumerable<Term> PaymentConditions = terms.Where(item => item.ConditionType == ConditionTypes.PaymentConditions.ToString());
            termsList.Add(PaymentConditions);

            IEnumerable<Term> ValidityPeriod = terms.Where(item => item.ConditionType == ConditionTypes.ValidityPeriod.ToString());
            termsList.Add(ValidityPeriod);

            IEnumerable<Term> ShopDrawingSubmittals = terms.Where(item => item.ConditionType == ConditionTypes.ShopDrawingSubmittals.ToString());
            termsList.Add(ShopDrawingSubmittals);

            IEnumerable<Term> Delivery = terms.Where(item => item.ConditionType == ConditionTypes.Delivery.ToString());
            termsList.Add(Delivery);

            IEnumerable<Term> Guarantee = terms.Where(item => item.ConditionType == ConditionTypes.Guarantee.ToString());
            termsList.Add(Guarantee);

            IEnumerable<Term> Remarks = terms.Where(item => item.ConditionType == ConditionTypes.Remarks.ToString());
            termsList.Add(Remarks);

            content = new StackPanel() { Orientation = Orientation.Vertical };
            textBlock = new TextBlock()
            {
                Text = "COMMERCIAL TERMS & CONDITIONS",
                FontSize = 29.5,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Calibri (Body)"),
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));
            _ = content.Children.Add(textBlock);
            totalHeight += textBlock.ActualHeight;

            content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            content.Arrange(new Rect(content.DesiredSize));

            line = new Line()
            {
                X1 = 0,
                X2 = 678.11,
                Stroke = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                StrokeThickness = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            line.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            line.Arrange(new Rect(line.DesiredSize));
            _ = content.Children.Add(line);
            totalHeight += line.ActualHeight;

            foreach (IEnumerable<Term> group in termsList)
            {
                if (group.Count() == 0)
                    continue;

                textBlock = new TextBlock()
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    TextWrapping = TextWrapping.Wrap,
                    TextDecorations = TextDecorations.Underline,
                    Margin = new Thickness(0, 0.5 * cm, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };

                if (termsList.IndexOf(group) == 0)
                    textBlock.Text = $"{++topicNumber}. Scope Of Supply:";
                if (termsList.IndexOf(group) == 1)
                    textBlock.Text = $"{++topicNumber}. Total Price:";
                if (termsList.IndexOf(group) == 2)
                    textBlock.Text = $"{++topicNumber}. Payment Conditions:";
                if (termsList.IndexOf(group) == 3)
                    textBlock.Text = $"{++topicNumber}. Validity Period:";
                if (termsList.IndexOf(group) == 4)
                    textBlock.Text = $"{++topicNumber}. Shop Drawing Submittals:";
                if (termsList.IndexOf(group) == 5)
                    textBlock.Text = $"{++topicNumber}. Delivery:";
                if (termsList.IndexOf(group) == 6)
                    textBlock.Text = $"{++topicNumber}. Guarantee:";
                if (termsList.IndexOf(group) == 7)
                    textBlock.Text = $"{++topicNumber}. Remarks:";

                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textBlock.Arrange(new Rect(textBlock.DesiredSize));
                totalHeight += textBlock.ActualHeight + 0.5 * cm;


                if (totalHeight > MaxPageHeight)
                {
                    TermsPages.Add(new QuotationPageView(QuotationData, content));
                    content = new StackPanel() { Orientation = Orientation.Vertical };
                    totalHeight = 0;
                    _ = content.Children.Add(textBlock);
                    totalHeight += textBlock.ActualHeight;
                }
                else if (totalHeight + 50 > MaxPageHeight)
                {
                    TermsPages.Add(new QuotationPageView(QuotationData, content));
                    content = new StackPanel() { Orientation = Orientation.Vertical };
                    totalHeight = 0;
                    _ = content.Children.Add(textBlock);
                    totalHeight += textBlock.ActualHeight;
                }
                else
                {
                    _ = content.Children.Add(textBlock);
                }

                foreach (Term termData in group)
                {
                    stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                    textBlock = new TextBlock()
                    {
                        Text = $"» ",
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Calibri (Body)"),
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    _ = stackPanel.Children.Add(textBlock);
                    textBlock = new TextBlock()
                    {
                        Text = termData.Condition,
                        FontSize = 18,
                        Width = 670,
                        FontFamily = new FontFamily("Calibri (Body)"),
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    _ = stackPanel.Children.Add(textBlock);

                    stackPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    stackPanel.Arrange(new Rect(stackPanel.DesiredSize));
                    totalHeight += stackPanel.ActualHeight;

                    if (totalHeight > MaxPageHeight)
                    {
                        TermsPages.Add(new QuotationPageView(QuotationData, content));
                        content = new StackPanel() { Orientation = Orientation.Vertical };
                        totalHeight = 0;
                        _ = content.Children.Add(stackPanel);
                        totalHeight += stackPanel.ActualHeight;
                    }
                    else
                    {
                        _ = content.Children.Add(stackPanel);
                    }
                }
            }

            var page = new QuotationPageView(QuotationData, content);
            TermsPages.Add(page);
        }


        private void GetBill()
        {
            if (PanelsData == null)
            {
                using SqlConnection connection = new(Database.ConnectionString);
                PanelsData = BillPanelController.GetBillPanels(connection, QuotationData.QuotationID);
            }

            if (PanelsData.Count == 0)
                return;

            BillPages.Clear();

            int lastIndex = 0;
            double totalHeight = 0;

            Line line;
            Grid grid;
            Border border;
            TextBlock textBlock;
            StackPanel content;

            content = new StackPanel() { Orientation = Orientation.Vertical };

            textBlock = new TextBlock()
            {
                Text = "BILL OF PRICES",
                FontSize = 29.5,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Calibri (Body)"),
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));

            _ = content.Children.Add(textBlock);
            totalHeight += textBlock.ActualHeight;

            line = new Line()
            {
                X1 = 0,
                X2 = 678.11,
                Stroke = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                StrokeThickness = 2,
                Margin = new Thickness(0, 0, 0, 10),
            };
            line.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            line.Arrange(new Rect(line.DesiredSize));
            _ = content.Children.Add(line);

            totalHeight += line.ActualHeight;

            if (QuotationData.OptionCode != null)
            {
                textBlock = new TextBlock()
                {
                    Text = $"Option {QuotationData.OptionCode}: {QuotationData.OptionName}",
                    FontSize = 20,
                    Width = 670,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0.5 * cm, 0, 0),
                };
                _ = content.Children.Add(textBlock);

                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textBlock.Arrange(new Rect(textBlock.DesiredSize));
                totalHeight += textBlock.ActualHeight;
            }

            #region Header
            grid = GetNewBillGrid();
            totalHeight += grid.ActualHeight;
            _ = content.Children.Add(grid);

            grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
            #endregion

            #region Grid
            for (int i = 0; i < PanelsData.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
                lastIndex++;

                textBlock = new TextBlock()
                {
                    Text = PanelsData[i].PanelName,
                    FontSize = 18,
                    Width = 322.7716535433072,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    Margin = new Thickness(5, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };
                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textBlock.Arrange(new Rect(textBlock.DesiredSize));

                var lines = textBlock.GetLines().ToList();
                bool isMultiLine = lines.Count > 1;

            Panel:

                //2
                textBlock = new TextBlock()
                {
                    Text = PanelsData[i].PanelName,
                    FontSize = 18,
                    Width = 322.7716535433072,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    Margin = new Thickness(5, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock,
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 1);

                border.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                border.Arrange(new Rect(border.DesiredSize));

                int nameLines = lines.Count;

                if (nameLines >= 1 && isMultiLine)
                {
                    textBlock.Text = "";
                    textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    textBlock.Arrange(new Rect(textBlock.DesiredSize));
                    border.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    border.Arrange(new Rect(border.DesiredSize));

                    double nameHeight = totalHeight + border.ActualHeight;
                    while (nameHeight + 25 /* Value of new row*/  < MaxPageHeight)
                    {
                        if (lines.Count == 0)
                            break;

                        if (textBlock.Text == "")
                            textBlock.Text += lines[0];
                        else
                            textBlock.Text += "\n" + lines[0];


                        textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        textBlock.Arrange(new Rect(textBlock.DesiredSize));
                        border.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        border.Arrange(new Rect(border.DesiredSize));

                        nameHeight = totalHeight + border.ActualHeight;
                        lines.Remove(lines[0]);
                    }

                    textBlock.Text = textBlock.Text.TrimEnd('\r', '\n');
                }

                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                textBlock.Arrange(new Rect(textBlock.DesiredSize));
                border.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                border.Arrange(new Rect(border.DesiredSize));

                totalHeight += border.ActualHeight;

                if (totalHeight + (isMultiLine ? 0 : 25) > MaxPageHeight)
                {
                    BillPages.Add(new QuotationPageView(QuotationData, content));
                    lastIndex = 1;
                    content = new StackPanel() { Orientation = Orientation.Vertical };
                    grid = GetNewBillGrid();
                    grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
                    _ = content.Children.Add(grid);

                    content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    content.Arrange(new Rect(content.DesiredSize));
                    totalHeight = content.ActualHeight;

                    grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
                    Grid.SetRow(border, lastIndex);
                    Grid.SetColumn(border, 1);
                    _ = grid.Children.Add(border);
                }
                else
                {
                    _ = grid.Children.Add(border);
                }

                //1
                textBlock = new TextBlock()
                {
                    Text = PanelsData[i].PanelSN.ToString(),
                    FontSize = 18,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    Background = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 0);
                _ = grid.Children.Add(border);

                //3
                textBlock = new TextBlock()
                {
                    Text = PanelsData[i].PanelQty.ToString(),
                    FontSize = 18,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 2);
                _ = grid.Children.Add(border);

                //4
                textBlock = new TextBlock()
                {
                    Text = PanelsData[i].PanelPriceInfo,
                    FontSize = 18,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 3);
                _ = grid.Children.Add(border);

                //5
                textBlock = new TextBlock()
                {
                    Text = PanelsData[i].PanelsPriceInfo,
                    FontSize = 18,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 4);
                _ = grid.Children.Add(border);

                if (lines.Count != 0 && isMultiLine)
                {
                    BillPages.Add(new QuotationPageView(QuotationData, content));
                    lastIndex = 1;
                    content = new StackPanel() { Orientation = Orientation.Vertical };
                    grid = GetNewBillGrid();
                    grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
                    _ = content.Children.Add(grid);

                    content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    content.Arrange(new Rect(content.DesiredSize));
                    totalHeight = content.ActualHeight;

                    grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
                    goto Panel;
                }
            }
            #endregion

            #region Totals
            lastIndex = 0;
            grid = GetNewTotalGrid();

            if (IsVAT || QuotationData.Discount != 0)
            {
                grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
                //1
                textBlock = new TextBlock()
                {
                    Text = "Total",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 5, 0),
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 0);
                Grid.SetColumnSpan(border, 4);
                _ = grid.Children.Add(border);

                //2
                textBlock = new TextBlock()
                {
                    Text = QuotationData.QuotationPrice.ToString("N2"),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 4);
                _ = grid.Children.Add(border);

                lastIndex++;
            }

            if (QuotationData.Discount != 0)
            {
                grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
                //1
                textBlock = new TextBlock()
                {
                    Text = $"Discount {QuotationData.Discount:N2}%",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 5, 0),
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 0);
                Grid.SetColumnSpan(border, 4);
                _ = grid.Children.Add(border);

                //2
                textBlock = new TextBlock()
                {
                    Text = QuotationData.QuotationDiscountValue.ToString("N2"),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 4);
                _ = grid.Children.Add(border);

                lastIndex++;
            }

            if (IsVAT)
            {
                grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
                //1
                textBlock = new TextBlock()
                {
                    Text = $"VAT {QuotationData.VATPercentage:N2}%",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 5, 0),
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 0);
                Grid.SetColumnSpan(border, 4);
                _ = grid.Children.Add(border);

                //2
                textBlock = new TextBlock()
                {
                    Text = QuotationData.QuotationVATValue.ToString("N2"),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = textBlock
                };
                Grid.SetRow(border, lastIndex);
                Grid.SetColumn(border, 4);
                _ = grid.Children.Add(border);

                ++lastIndex;
            }

            grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
            //1
            textBlock = new TextBlock()
            {
                Text = "Total (Net Price)",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 5, 0),
                FontFamily = new FontFamily("Calibri (Body)"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            border = new Border()
            {
                Child = textBlock,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1, 0, 1, 1),
            };
            Grid.SetRow(border, lastIndex);
            Grid.SetColumn(border, 0);
            Grid.SetColumnSpan(border, 4);
            _ = grid.Children.Add(border);

            //2
            if (IsVAT)
            {
                textBlock = new TextBlock()
                {
                    Text = QuotationData.QuotationFinalPrice.ToString("N2"),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            else
            {
                textBlock = new TextBlock()
                {
                    Text = QuotationData.QuotationEstimatedPrice.ToString("N2"),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            border = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = textBlock
            };

            Grid.SetRow(border, lastIndex);
            Grid.SetColumn(border, 4);
            _ = grid.Children.Add(border);
            ++lastIndex;


            grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });
            string priceText;
            if (IsVAT)
            {
                priceText = DataInput.Input.NumberToSRWords(QuotationData.QuotationFinalPrice);
            }
            else
            {
                priceText = DataInput.Input.NumberToSRWords(QuotationData.QuotationEstimatedPrice);
            }

            textBlock = new TextBlock()
            {
                Text = priceText,
                FontSize = 18,
                Width = 670,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new FontFamily("Calibri (Body)"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(textBlock, lastIndex);
            Grid.SetColumn(textBlock, 0);
            Grid.SetColumnSpan(textBlock, 5);
            _ = grid.Children.Add(textBlock);
            ++lastIndex;

            if (!IsVAT)
            {
                grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });

                textBlock = new TextBlock()
                {
                    Text = $"\nNote: \nVAT excluded from price.",
                    FontSize = 18,
                    Width = 670,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Brushes.Red,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                if (QuotationData.BillOfPriceNoteLanguage == "Arabic")
                {
                    textBlock.Text = "\nملاحظات: ";
                    textBlock.Text += "\n- لا يشمل السعر ضريبة القيمة المضافة.";
                    textBlock.FlowDirection = FlowDirection.RightToLeft;
                }

                Grid.SetRow(textBlock, lastIndex);
                Grid.SetColumn(textBlock, 0);
                Grid.SetColumnSpan(textBlock, 5);
                _ = grid.Children.Add(textBlock);
                ++lastIndex;
            }

            if (!string.IsNullOrWhiteSpace(QuotationData.BillOfPriceNote))
            {
                grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });

                if (IsVAT)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { MinHeight = 24 });

                    textBlock = new TextBlock()
                    {
                        Text = $"\nNote:",
                        FontSize = 18,
                        Width = 670,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Brushes.Red,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Calibri (Body)"),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    if (QuotationData.BillOfPriceNoteLanguage == "Arabic")
                    {
                        textBlock.Text = "\nملاحظات: ";
                        textBlock.FlowDirection = FlowDirection.RightToLeft;
                    }

                    Grid.SetRow(textBlock, lastIndex);
                    Grid.SetColumn(textBlock, 0);
                    Grid.SetColumnSpan(textBlock, 5);
                    _ = grid.Children.Add(textBlock);
                    ++lastIndex;
                }

                textBlock = new TextBlock()
                {
                    Text = $"{QuotationData.BillOfPriceNote}",
                    FontSize = 18,
                    Width = 670,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Brushes.Red,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Calibri (Body)"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                if (QuotationData.BillOfPriceNoteLanguage == "Arabic")
                {
                    textBlock.FlowDirection = FlowDirection.RightToLeft;
                }

                Grid.SetRow(textBlock, lastIndex);
                Grid.SetColumn(textBlock, 0);
                Grid.SetColumnSpan(textBlock, 5);
                _ = grid.Children.Add(textBlock);
                ++lastIndex;
            }

            grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            grid.Arrange(new Rect(grid.DesiredSize));
            totalHeight += grid.ActualHeight;

            if (totalHeight + 25 > MaxPageHeight)
            {
                BillPages.Add(new QuotationPageView(QuotationData, content));
                content = new StackPanel() { Orientation = Orientation.Vertical };
                _ = content.Children.Add(grid);
            }
            else
            {
                _ = content.Children.Add(grid);
            }

            #endregion

            BillPages.Add(new QuotationPageView(QuotationData, content));
        }
        private Grid GetNewBillGrid()
        {
            Border border;
            TextBlock textBlock;
            Grid grid = new();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.20 * cm) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8.54 * cm) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.20 * cm) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3.50 * cm) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3.50 * cm) });

            textBlock = new TextBlock()
            {
                Text = "Item",
                FontSize = 18,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Calibri (Body)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            border = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1, 1, 1, 1),
                Background = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                Child = textBlock,
            };
            Grid.SetColumn(border, 0);
            _ = grid.Children.Add(border);

            //2
            textBlock = new TextBlock()
            {
                Text = "Description",
                FontSize = 18,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Calibri (Body)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            border = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 1, 1, 1),
                Background = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                Child = textBlock
            };
            Grid.SetColumn(border, 1);
            _ = grid.Children.Add(border);

            //3
            textBlock = new TextBlock()
            {
                Text = "QTY",
                FontSize = 18,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Calibri (Body)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            border = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 1, 1, 1),
                Background = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                Child = textBlock
            };
            Grid.SetColumn(border, 2);
            _ = grid.Children.Add(border);

            //4
            textBlock = new TextBlock()
            {
                Text = "U/Price (SR)",
                FontSize = 18,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Calibri (Body)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            border = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 1, 1, 1),
                Background = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                Child = textBlock
            };
            Grid.SetColumn(border, 3);
            _ = grid.Children.Add(border);


            //5
            textBlock = new TextBlock()
            {
                Text = "T/Price (SR)",
                FontSize = 18,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Calibri (Body)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            border = new Border()
            {
                Width = 3.5 * cm,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 1, 1, 1),
                Background = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                Child = textBlock
            };
            Grid.SetColumn(border, 4);
            _ = grid.Children.Add(border);

            grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            grid.Arrange(new Rect(grid.DesiredSize));

            return grid;
        }
        private Grid GetNewTotalGrid()
        {
            Grid grid = new();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.20 * cm) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8.54 * cm) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.20 * cm) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3.50 * cm) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3.50 * cm) });

            grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            grid.Arrange(new Rect(grid.DesiredSize));

            return grid;
        }

        private void GetDetails()
        {
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                if (PanelsData == null)
                {
                    PanelsData = BillPanelController.GetBillPanels(connection, QuotationData.QuotationID);
                }

                string panelsID = null;
                foreach (BillPanel panel in PanelsData.Where(p => !p.IsSpecial))
                {
                    if (panelsID == null)
                    {
                        panelsID = panel.PanelID.ToString();
                    }
                    else
                    {
                        panelsID += $", {panel.PanelID}";
                    }
                }

                ItemsData = new List<BillItem>();
                if (panelsID != null)
                {
                    ItemsData = BillItemController.PanelsDetails(connection, panelsID);
                }
            }

            if (PanelsData.Count == 0)
                return;

            if (ItemsData.Count == 0)
                return;


            DetailsPages.Clear();

            Line line;
            TextBlock textBlock;
            StackPanel content = new() { Margin = new Thickness(-cm, 0, -cm, 0) };

            double totalHeight = 0;

            textBlock = new TextBlock()
            {
                Text = "TECHNICAL DETAILS",
                FontSize = 29.5,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Calibri (Body)"),
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));
            totalHeight += textBlock.ActualHeight;
            _ = content.Children.Add(textBlock);

            line = new Line()
            {
                X1 = 0,
                X2 = 678,
                Stroke = (Brush)new BrushConverter().ConvertFromString("#4f81bd"),
                StrokeThickness = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            line.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            line.Arrange(new Rect(line.DesiredSize));
            totalHeight += line.ActualHeight;
            _ = content.Children.Add(line);

            foreach (BillPanel panel in PanelsData.Where(p => !p.IsSpecial))
            {
                int panelSN = PanelsData.IndexOf(panel);
                List<BillItem> panelItems = ItemsData.Where(item => item.PanelID == panel.PanelID).ToList();

                if (panelItems.Count == 0)
                    continue;


            NewPage:
                QuotationPanelInfo quotationPanelInfo = new(panel);
                quotationPanelInfo.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                quotationPanelInfo.Arrange(new Rect(quotationPanelInfo.DesiredSize));
                totalHeight += quotationPanelInfo.ActualHeight;

                if (totalHeight > (MaxPageHeight * 0.90))
                {
                    DetailsPages.Add(new QuotationPageView(QuotationData, content));
                    content = new StackPanel() { Margin = new Thickness(-cm, 0, -cm, 0) };

                    content.Children.Add(quotationPanelInfo);

                    quotationPanelInfo.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    quotationPanelInfo.Arrange(new Rect(quotationPanelInfo.DesiredSize));

                    totalHeight = quotationPanelInfo.ActualHeight;
                }
                else
                {
                    content.Children.Add(quotationPanelInfo);
                }

            NewDescription:
                if (panelItems.Count == 0)
                    continue;
                ////////////////////////////////////////////////////////
                NewDescriptionCell(panelItems[0]);
                var lines = DescriptionTextBlock.GetLines().ToList();
                bool isMultiLine = lines.Count > 1;
                //////////////////////////////////////////////////////////
                if (isMultiLine)
                {
                    if (totalHeight + DescriptionTextBlock.ActualHeight < MaxPageHeight)
                    {
                        quotationPanelInfo.AddItem = panelItems[0];
                        panelItems.Remove(panelItems[0]);
                        totalHeight += DescriptionTextBlock.ActualHeight;
                        goto NewDescription;
                    }
                    else
                    {
                        if (quotationPanelInfo.ItemsCount == 0)
                        {
                            content.Children.Remove(quotationPanelInfo);
                        }

                        totalHeight = 0;
                        DetailsPages.Add(new QuotationPageView(QuotationData, content, Visibility.Visible));
                        content = new StackPanel() { Margin = new Thickness(-cm, 0, -cm, 0) };

                        goto NewPage;
                    }
                }
                else
                {
                    if (totalHeight + 0.7 * cm < MaxPageHeight)
                    {
                        quotationPanelInfo.AddItem = panelItems[0];
                        panelItems.Remove(panelItems[0]);
                        totalHeight += 0.6 * cm;

                        goto NewDescription;
                    }
                    else
                    {
                        totalHeight = 0;
                        DetailsPages.Add(new QuotationPageView(QuotationData, content, Visibility.Visible));
                        content = new StackPanel() { Margin = new Thickness(-cm, 0, -cm, 0) };

                        goto NewPage;
                    }
                }



                //    while (totalHeight + 0.7 * cm < MaxPageHeight)
                //    {
                //        if (panelItems.Count == 0)
                //            break;

                //        if (lines.Count == 0)
                //            break;

                //        if (isMultiLine)
                //        {
                //            if (description == "")
                //                description += lines[0];
                //            else
                //                description += $"\n{lines[0]}";

                //            totalHeight += 17.09;
                //            lines.Remove(lines[0]);
                //        }
                //        else
                //        {
                //            quotationPanelInfo.AddItem = panelItems[0];
                //            panelItems.Remove(panelItems[0]);
                //            totalHeight += 0.6 * cm;
                //        }
                //    }

                //if (isMultiLine)
                //{
                //    if (lines.Count == 0)
                //    {
                //        panelItems[0].Description = description;
                //        quotationPanelInfo.AddItem = panelItems[0];
                //        panelItems.Remove(panelItems[0]);
                //    }
                //    if (lines.Count != 0)
                //    {
                //        BillItem item = new();
                //        item.Update(panelItems[0]);
                //        item.Description = description;
                //        quotationPanelInfo.AddItem = item;
                //        panelItems[0].Description = string.Join("\n", lines);

                //        totalHeight = 0;
                //        DetailsPages.Add(new QuotationPageView(QuotationData, content, Visibility.Visible));
                //        content = new StackPanel() { Margin = new Thickness(-cm, 0, -cm, 0) };

                //        goto NewPage;
                //    }
                //}



                //if (panelItems.Count != 0)
                //{
                //    totalHeight = 0;
                //    DetailsPages.Add(new QuotationPageView(QuotationData, content, Visibility.Visible));
                //    content = new StackPanel() { Margin = new Thickness(-cm, 0, -cm, 0) };

                //    goto NewPage;
                //}
            }

            DetailsPages.Add(new QuotationPageView(QuotationData, content));
        }

        private static Grid NameCell { get; set; }
        private static Border NameBorder { get; set; }
        private static TextBlock DescriptionTextBlock { get; set; }

        private static void NewDescriptionCell(BillItem item)
        {
            DescriptionTextBlock = new TextBlock()
            {
                FontWeight = FontWeights.Bold,
                Text = item.Description,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new FontFamily("Calibri (Body)"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 5, 0)
            };
            NameBorder = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = DescriptionTextBlock,
            };

            NameCell = new Grid()
            {
                Width = 348,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            NameCell.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(348) });
            Grid.SetColumn(NameBorder, 0);

            NameCell.Children.Add(NameBorder);

            UpdateUI();
        }
        static void UpdateUI()
        {
            List<UIElement> elements = new()
            {
                DescriptionTextBlock,
                NameCell,
                NameBorder,
            };

            foreach (UIElement element in elements)
            {
                if (element == null)
                    continue;

                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                element.Arrange(new Rect(element.DesiredSize));
            }
        }
        private void GetCover()
        {
            CoverPages.Clear();

            int startPage = 1;
            QuotationContent content;
            List<QuotationContent> contents = new();

            if (IsTerms && TermsPages.Count != 0)
            {
                content = new QuotationContent();
                content.Description = "Commercial Terms & Conditions.";
                if (TermsPages.Count == 1)
                {
                    content.Pages = (++startPage).ToString();
                }
                else
                {
                    content.Pages = $"{++startPage} - {startPage + TermsPages.Count - 1}";
                }

                startPage = startPage + TermsPages.Count - 1;
                contents.Add(content);
            }

            if (IsBill && BillPages.Count != 0)
            {
                content = new QuotationContent();
                content.Description = "Bill of Quantity & Price.";
                if (BillPages.Count == 1)
                {
                    content.Pages = (++startPage).ToString();
                }
                else
                {
                    content.Pages = $"{++startPage} - {startPage + BillPages.Count - 1}";
                }

                startPage = startPage + BillPages.Count - 1;
                contents.Add(content);
            }

            if (IsDetails && DetailsPages.Count != 0)
            {
                content = new QuotationContent();
                content.Description = "Technical Details.";
                if (DetailsPages.Count == 1)
                {
                    content.Pages = (++startPage).ToString();
                }
                else
                {
                    content.Pages = $"{++startPage} - {startPage + DetailsPages.Count - 1}";
                }

                contents.Add(content);
            }

            CoverPages.Add(new QuotationCoverView(QuotationData, contents));
        }

    }
}