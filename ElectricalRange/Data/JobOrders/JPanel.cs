using Dapper.Contrib.Extensions;

using ProjectsNow.Enums;

using System;
using System.IO;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[Panels]")]
    public class JPanel : Base
    {

        [Key]
        public int PanelID { get; set; }
        public int JobOrderID { get; set; }
        public int PurchaseOrdersID { get; set; }

        private int? _PanelSN;
        public int? PanelSN
        {
            get => _PanelSN;
            set => SetValue(ref _PanelSN, value);
        }

        private string _PanelName;
        public string PanelName
        {
            get => _PanelName;
            set
            {
                if (SetValue(ref _PanelName, value))
                {
                    OnPropertyChanged(nameof(PanelNameInfo));
                }
            }
        }


        [Write(false)]
        public string PanelNameInfo
        {
            get
            {
                using var reader = new StringReader(PanelName);
                return reader.ReadLine();
            }
        }
        public int PanelQty { get; set; }


        private string _Status = Statuses.New.ToString();
        public string Status
        {
            get => _Status;
            set => SetValue(ref _Status, value);
        }

        private DateTime? _DateOfCreation = DateTime.Now;
        public DateTime? DateOfCreation
        {
            get => _DateOfCreation;
            set => SetValue(ref _DateOfCreation, value);
        }


        private DateTime? _DateOfDesign;
        public DateTime? DateOfDesign
        {
            get => _DateOfDesign;
            set => SetValue(ref _DateOfDesign, value);
        }

        private DateTime? _DateOfSendingForApproval;
        public DateTime? DateOfSendingForApproval
        {
            get => _DateOfSendingForApproval;
            set => SetValue(ref _DateOfSendingForApproval, value);
        }


        private DateTime? _DateOfProduction;
        public DateTime? DateOfProduction
        {
            get => _DateOfProduction;
            set => SetValue(ref _DateOfProduction, value)
                  .UpdateProperties(this, nameof(DateOfProductionGroup));
        }

        public string DateOfProductionGroup
        {
            get
            {
                if (_DateOfProduction.GetValueOrDefault() == null)
                {
                    return null;
                }
                else
                {
                    return _DateOfProduction.GetValueOrDefault().ToString("dd/MM/yyyy");
                }
            }
        }

        private DateTime? _DateOfClosing;
        public DateTime? DateOfClosing
        {
            get => _DateOfClosing;
            set => SetValue(ref _DateOfClosing, value);
        }

        private DateTime? _DateOfDelivery;
        public DateTime? DateOfDelivery
        {
            get => _DateOfDelivery;
            set => SetValue(ref _DateOfDelivery, value);
        }

        private DateTime? _DateOfHolding;
        public DateTime? DateOfHolding
        {
            get => _DateOfHolding;
            set => SetValue(ref _DateOfHolding, value);
        }

        private DateTime? _DateOfCancellation;
        public DateTime? DateOfCancellation
        {
            get => _DateOfCancellation;
            set => SetValue(ref _DateOfCancellation, value);
        }


        private int _ClosedQty;
        [Write(false)]
        public int ClosedQty
        {
            get => _ClosedQty;
            set
            {
                if (SetValue(ref _ClosedQty, value))
                {
                    OnPropertyChanged(nameof(NotClosedQty));
                    OnPropertyChanged(nameof(ReadyToCloseQty));
                    OnPropertyChanged(nameof(ClosedToTotalQty));
                    OnPropertyChanged(nameof(NotClosedToTotalQty));
                    OnPropertyChanged(nameof(ReadyToDeliverQty));
                    OnPropertyChanged(nameof(ReadyToHoldQty));
                }
            }
        }
        [Write(false)]
        public int NotClosedQty => PanelQty - ClosedQty;
        [Write(false)]
        public int ReadyToCloseQty => PanelQty - ClosedQty - HoldQty - CanceledQty;
        [Write(false)]
        public string ClosedToTotalQty => $"{ClosedQty} / {PanelQty}";
        [Write(false)]
        public string NotClosedToTotalQty => $"{NotClosedQty} / {PanelQty}";


        private int _InvoicedQty;

        [Write(false)]
        public int InvoicedQty
        {
            get => _InvoicedQty;
            set
            {
                if (SetValue(ref _InvoicedQty, value))
                {
                    OnPropertyChanged(nameof(NotInvoicedQty));
                    OnPropertyChanged(nameof(ReadyToInvoicedQty));
                    OnPropertyChanged(nameof(InvoicedToTotalQty));
                    OnPropertyChanged(nameof(ReadyToHoldQty));
                }
            }
        }

        [Write(false)]
        public int NotInvoicedQty => PanelQty - InvoicedQty;

        [Write(false)]
        public int ReadyToInvoicedQty => PanelQty - InvoicedQty - HoldQty - CanceledQty;

        [Write(false)]
        public string InvoicedToTotalQty => $"{InvoicedQty} / {PanelQty}";


        private int _ProformaInvoicedQty;
        [Write(false)]
        public int ProformaInvoicedQty
        {
            get => _ProformaInvoicedQty;
            set
            {
                if (SetValue(ref _ProformaInvoicedQty, value))
                {
                    OnPropertyChanged(nameof(NotProformaInvoicedQty));
                    OnPropertyChanged(nameof(ReadyToProformaInvoicedQty));
                    OnPropertyChanged(nameof(ProformaInvoicedToTotalQty));
                    OnPropertyChanged(nameof(ReadyToHoldQty));
                }
            }
        }
        [Write(false)]
        public int NotProformaInvoicedQty => PanelQty - ProformaInvoicedQty;
        [Write(false)]
        public int ReadyToProformaInvoicedQty => PanelQty - ProformaInvoicedQty - HoldQty - CanceledQty;
        [Write(false)]
        public string ProformaInvoicedToTotalQty => $"{ProformaInvoicedQty} / {PanelQty}";


        private int _DeliveredQty;
        [Write(false)]
        public int DeliveredQty
        {
            get => _DeliveredQty;
            set
            {
                if (SetValue(ref _DeliveredQty, value))
                {
                    OnPropertyChanged(nameof(ReadyToCloseQty));
                    OnPropertyChanged(nameof(NotDeliveredQty));
                    OnPropertyChanged(nameof(ReadyToDeliverQty));
                    OnPropertyChanged(nameof(DeliveredToTotalQty));
                    OnPropertyChanged(nameof(ReadyToHoldQty));
                }
            }
        }

        [Write(false)]
        public int NotDeliveredQty => PanelQty - DeliveredQty;

        [Write(false)]
        public int ReadyToDeliverQty => NotDeliveredQty;

        [Write(false)]
        public string DeliveredToTotalQty => $"{DeliveredQty} / {PanelQty}";


        private int _HoldQty;
        [Write(false)]
        public int HoldQty
        {
            get => _HoldQty;
            set
            {
                if (SetValue(ref _HoldQty, value))
                {
                    OnPropertyChanged(nameof(NotHoldQty));
                    OnPropertyChanged(nameof(ReadyToCloseQty));
                    OnPropertyChanged(nameof(ReadyToInvoicedQty));
                    OnPropertyChanged(nameof(ReadyToDeliverQty));
                    OnPropertyChanged(nameof(ReadyToHoldQty));
                    OnPropertyChanged(nameof(PanelQtyView));
                }
            }
        }
        [Write(false)]
        public int NotHoldQty => PanelQty - HoldQty;
        [Write(false)]
        public int ReadyToHoldQty => NotClosedQty - HoldQty - CanceledQty;


        private int _CanceledQty;
        [Write(false)]
        public int CanceledQty
        {
            get => _CanceledQty;
            set
            {
                if (SetValue(ref _CanceledQty, value))
                {
                    OnPropertyChanged(nameof(NotCanceledQty));

                    OnPropertyChanged(nameof(ReadyToCloseQty));
                    OnPropertyChanged(nameof(ReadyToInvoicedQty));
                    OnPropertyChanged(nameof(ReadyToDeliverQty));
                    OnPropertyChanged(nameof(PanelQtyView));
                }
            }
        }
        public int NotCanceledQty => PanelQty - CanceledQty;



        private int _ApprovedQty;

        [Write(false)]
        public int ApprovedQty
        {
            get => _ApprovedQty;
            set
            {
                if (SetValue(ref _ApprovedQty, value))
                {
                    OnPropertyChanged(nameof(NotApprovedQty));
                    OnPropertyChanged(nameof(ReadyToApproveQty));
                    OnPropertyChanged(nameof(ApprovedToTotalQty));
                }
            }
        }

        [Write(false)]
        public int NotApprovedQty => PanelQty - ApprovedQty;

        [Write(false)]
        public int ReadyToApproveQty => NotApprovedQty;

        [Write(false)]
        public string ApprovedToTotalQty => $"{ApprovedQty} / {PanelQty}";


        private int _TestedQty;

        [Write(false)]
        public int TestedQty
        {
            get => _TestedQty;
            set
            {
                if (SetValue(ref _TestedQty, value))
                {
                    OnPropertyChanged(nameof(NotTestedQty));
                    OnPropertyChanged(nameof(ReadyToTestQty));
                    OnPropertyChanged(nameof(TestedToTotalQty));
                }
            }
        }

        [Write(false)]
        public int NotTestedQty => PanelQty - TestedQty;

        [Write(false)]
        public int ReadyToTestQty => NotTestedQty;

        [Write(false)]
        public string TestedToTotalQty => $"{TestedQty} / {PanelQty}";


        private int _ProductionQty;

        [Write(false)]
        public int ProductionQty
        {
            get => _ProductionQty;
            set
            {
                if (SetValue(ref _ProductionQty, value))
                {
                    OnPropertyChanged(nameof(NotProductionQty));
                    OnPropertyChanged(nameof(ReadyToTestQty));
                    OnPropertyChanged(nameof(ProductionToTotalQty));
                }
            }
        }

        [Write(false)]
        public int NotProductionQty => PanelQty - ProductionQty;

        [Write(false)]
        public int ReadyToProductionQty => NotProductionQty;

        [Write(false)]
        public string ProductionToTotalQty => $"{ProductionQty} / {PanelQty}";




        [Write(false)]
        public string PanelQtyView
        {
            get
            {
                string qty = (PanelQty - HoldQty - CanceledQty).ToString();
                if (HoldQty != 0)
                {
                    qty += $"/{HoldQty}H";
                }

                if (CanceledQty != 0)
                {
                    qty += $"/{CanceledQty}C";
                }

                return qty;
            }
        }

        private string _EnclosureType;
        public string EnclosureType
        {
            get => _EnclosureType;
            set => SetValue(ref _EnclosureType, value);
        }

        private decimal? _EnclosureHeight;
        public decimal? EnclosureHeight
        {
            get => _EnclosureHeight;
            set => SetValue(ref _EnclosureHeight, value);
        }

        private decimal? _EnclosureWidth;
        public decimal? EnclosureWidth
        {
            get => _EnclosureWidth;
            set => SetValue(ref _EnclosureWidth, value);
        }

        private decimal? _EnclosureDepth;
        public decimal? EnclosureDepth
        {
            get => _EnclosureDepth;
            set => SetValue(ref _EnclosureDepth, value);
        }

        private string _EnclosureIP;
        public string EnclosureIP
        {
            get => _EnclosureIP;
            set => SetValue(ref _EnclosureIP, value);
        }

        public decimal PanelProfit { get; set; }

        public decimal PanelEstimatedPrice { get; set; }
        public decimal PanelsEstimatedPrice { get; set; }

        public decimal PanelFinalPrice { get; set; }
        public decimal PanelsFinalPrice { get; set; }

        public decimal PanelVATValue { get; set; }
        public decimal PanelsVATValue { get; set; }


        private decimal _PanelDesignCost;
        [Write(false)]
        public decimal PanelDesignCost
        {
            get => _PanelDesignCost;
            set
            {
                if (SetValue(ref _PanelDesignCost, value))
                {
                    OnPropertyChanged(nameof(PanelsDesignCost));
                    OnPropertyChanged(nameof(PanelDesignPrice));
                    OnPropertyChanged(nameof(PanelsDesignPrice));
                }
            }
        }

        [Write(false)]
        public decimal PanelsDesignCost => _PanelDesignCost * PanelQty;

        [Write(false)]
        public decimal PanelDesignPrice => _PanelDesignCost / (1 - PanelProfit / 100m);

        [Write(false)]
        public decimal PanelsDesignPrice => _PanelDesignCost * PanelQty / (1 - PanelProfit / 100m);


        public int Revision { get; set; }
        public bool IsSpecial { get; set; }
        public string PanelType { get; set; }
        public string PanelTypeArabic { get; set; }
        public string DrawingManualCode { get; set; }
        public int? DrawingNo { get; set; }

        public bool? IsPercentageInvoice { get; set; }
    }
}
