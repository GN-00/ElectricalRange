using Dapper.Contrib.Extensions;

using System.IO;

namespace ProjectsNow.Data.Quotations
{
    [Table("[Quotation].[QuotationsOptionsPanels]")]
    public class QuotationOptionPanel: Base
    {
        private string _PanelName;

        [Key]
        public int ID { get; set; }
        public int OptionID { get; set; }
        public int PanelID { get; set; }

        [Write(false)]
        public int PanelSN { get; set; }
        [Write(false)]
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

        [Write(false)]
        public int PanelQty { get; set; }

        [Write(false)]
        public string EnclosureName { get; set; }

        [Write(false)]
        public decimal EstimatedPrice{ get; set; }
    }
}
