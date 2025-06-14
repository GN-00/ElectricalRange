using Dapper.Contrib.Extensions;

using System;

namespace ProjectsNow.Data.Finance
{
    [Table("[Finance].[MasterTransactions]")]
    public class AccountTransaction : TransactionBase //Level 1
    {
        private DateTime _Date = DateTime.Now;
        private int _Number;
        private string _Code;
        private string _Description;
        private double _Amount;
        private bool _Post;

        [Key]
        public int Id { get; set; }
        public int OldId { get; set; }

        #region Ids
        public int? AccountId { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public int? OtherId { get; set; }
        #endregion

        [Write(false)]
        public string Account { get; set; }

        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value).UpdateProperties(this, nameof(DateInfo));
        }

        public int Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }

        public string Code
        {
            get => _Code;
            set => SetValue(ref _Code, value);
        }

        [Write(false)]
        public string DateInfo
        {
            get => Date.ToString("dd-MM-yyyy");
        }

        public string Description
        {
            get => _Description;
            set => SetValue(ref _Description, value);
        }
        public double Amount
        {
            get => _Amount;
            set => SetValue(ref _Amount, value);
        }

        public string Type { get; set; }

        public bool Post
        {
            get => _Post;
            set => SetValue(ref _Post, value);
        }

        [Write(false)]
        public string Client { get; set; }
    }
}
