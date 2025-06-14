using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Quotations
{
    [Table("[Quotation].[Terms&Conditions]")]
    public class Term : Base
    {
        private string _Condition;

        [Key]
        public int TermID { get; set; }
        public int QuotationID { get; set; }
        public int Sort { get; set; }
        public string Condition
        {
            get => _Condition;
            set => SetValue(ref _Condition, value);
        }
        public string ConditionType { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDefault { get; set; }
    }
}
