using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Production
{
    [Table("[Production].[References]")]
    public class Reference : Base
    {
        private string _Code;
        private string _Description;
        private string _Unit = "No";
        private string _Note;
        private double? _Sort1;
        private double? _Sort2;

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
        public string Note
        {
            get => _Note;
            set => SetValue(ref _Note, value);
        }

        public double? Sort1
        {
            get => _Sort1;
            set => SetValue(ref _Sort1, value);
        }

        public double? Sort2
        {
            get => _Sort2;
            set => SetValue(ref _Sort2, value);
        }
    }
}