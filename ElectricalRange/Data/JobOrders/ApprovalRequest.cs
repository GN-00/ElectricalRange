﻿using System;

namespace ProjectsNow.Data.JobOrders
{
    public class ApprovalRequest : Base
    {
        private string _Number;
        public string Number
        {
            get => _Number;
            set => SetValue(ref _Number, value);
        }
        public DateTime? Date { get; set; }
        public int? AttachmentId { get; set; }
    }
}
