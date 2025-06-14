using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Inquiries
{
    [Table("[Inquiry].[Communications]")]
    public class Communication : Base
    {
        private DateTime _Date = DateTime.Now;
        private string _Description;
        private string _Status = "On Going";
        private string _Type = "Phone";

        [Key]
        public int Id { get; set; }
        public int InquiryID { get; set; }
        public int SalesmanID { get; set; }

        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }

        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }

        public string Status
        {
            get => _Status;
            set => SetValue(ref _Status, value);
        }

        public string Type
        {
            get => _Type;
            set => SetValue(ref _Type, value);
        }

        [Write(false)]
        public string Salesman { get; set; }
    }
}
