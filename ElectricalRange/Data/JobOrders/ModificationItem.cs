using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Collections.ObjectModel;
using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[PanelsItems]")]
    public class ModificationItem : Base 
    {
        private string _Code;
        private string _Description;
        private decimal _ItemQty;
        private string _Brand;
        private string _ItemType;
        private bool _IsGhostRecord;

        [Key]
        public int ItemID { get; set; }
        public DateTime Date { get; set; }
        public int ModificationID { get; set; }
        public int PanelID { get; set; }
        public int PanelSN { get; set; }
        public string PanelName { get; set; }
        public int PanelQty { get; set; }
        public string Category { get; set; }
        public string Code
        {
            get => _Code; 
            set => SetValue(ref _Code, value);
        }
        public string Description
        {
            get => _Description; 
            set => SetValue(ref _Description, value);
        }
        public string Unit { get; set; }
        public decimal ItemQty
        {
            get => _ItemQty;
            set
            {
                if (SetValue(ref _ItemQty, value))
                {
                   OnPropertyChanged(nameof(TotalQty));
                }; 
            }
        }
        public decimal TotalQty => PanelQty * ItemQty;
        public string Brand
        {
            get => _Brand; 
            set => SetValue(ref _Brand, value);
        }
        public decimal ItemCost { get; set; }
        public decimal ItemDiscount { get; set; }
        [Write(false)]
        public decimal ItemPrice => ItemCost * (1 - (ItemDiscount / 100m));
        [Write(false)]
        public decimal ItemTotalCost => ItemCost * ItemQty;
        [Write(false)]
        public decimal ItemTotalPrice => ItemCost * ItemQty * (1 - (ItemDiscount / 100m));
        public string ItemTable { get; set; }
        public string ItemType
        {
            get => _ItemType; 
            set => SetValue(ref _ItemType, value);
        }
        public int ItemSort { get; set; }
        public string Source { get; set; }
        [Write(false)]
        public Visibility EditVisibility
        {
            get
            {
                if (DateTime.Today == Date)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        [Write(false)]
        public bool IsGhostRecord
        {
            get => _IsGhostRecord;
            set
            {
                if (SetValue(ref _IsGhostRecord, value))
                {
                    OnPropertyChanged(nameof(GhostRecord));
                }
            }
        }
        [Write(false)]
        public Visibility GhostRecord
        {
            get => IsGhostRecord? Visibility.Collapsed : Visibility.Visible;
        }

    }
}
