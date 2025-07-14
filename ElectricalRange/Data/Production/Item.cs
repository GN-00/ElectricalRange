using Dapper.Contrib.Extensions;

using System.CodeDom;

namespace ProjectsNow.Data.Production
{
    [Table("[Production].[PanelsItems]")]
    public class Item : Base
    {
        private string _Code;
        private string _Description;
        private string _Unit;

        [Key]
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public int PanelId { get; set; }
        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }
        public string Description 
        {
            get => _Description;
            set=> SetValue(ref _Description, value); 
        }
        public double Qty { get; set; }
        public string Type { get; set; }

        [Write(false)]
        public double ReceivedQty { get; set; }

        [Write(false)]
        public double Percentage  => (ReceivedQty / Qty) * 100;

        [Write(false)]
        public double StockQty { get; set; }

        public int? RequestId { get; set; }
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }

    }
}
