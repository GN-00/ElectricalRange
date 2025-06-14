using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[CustomersInvoicesEdits]")]
    public class InvoiceEdit : Base
    {
        private int _PageSerial;
        private int _PagePanels;

        [Key]
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int PageSerial
        {
            get => _PageSerial;
            set => SetValue(ref _PageSerial, value);
        }

        public int PagePanels
        {
            get => _PagePanels;
            set => SetValue(ref _PagePanels, value);
        }
    }
}
