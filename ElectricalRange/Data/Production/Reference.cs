using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Production
{
    [Table("[Production].[References]")]
    public class Reference : Base
    {
        private string _Code;
        private string _Description;
        private string _Unit = "No";

        [Key]
        public int ReferenceId { get; set; }

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
        public string Unit
        {
            get => _Unit;
            set => SetValue(ref _Unit, value);
        }
    }
}