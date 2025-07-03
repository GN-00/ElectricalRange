
using Dapper.Contrib.Extensions;
using System;

namespace ProjectsNow.Data.Production
{
    [Table("[Production].[Tasks]")]
    public class Task : Base
    {
        [Key]
        public int Id { get; set; }
        public int PanelId { get; set; }
        public int JobOrderId { get; set; }
        
        private string _TaskName;
        public string TaskName
        {
            get => _TaskName;
            set => SetValue(ref _TaskName, value);
        }
        
        private DateTime? _StartDate;
        public DateTime? StartDate
        {
            get => _StartDate;
            set => SetValue(ref _StartDate, value);
        }
        
        private DateTime? _EndDate;
        public DateTime? EndDate
        {
            get => _EndDate;
            set => SetValue(ref _EndDate, value);
        }
        
        private int _Progress;
        public int Progress
        {
            get => _Progress;
            set => SetValue(ref _Progress, value);
        }
        
        private string _Status = "To Do";
        public string Status
        {
            get => _Status;
            set => SetValue(ref _Status, value);
        }
        
        private string _AssignedEmployees;
        public string AssignedEmployees
        {
            get => _AssignedEmployees;
            set => SetValue(ref _AssignedEmployees, value);
        }
        
        [Write(false)]
        public string PanelName { get; set; }
        
        [Write(false)]
        public string PanelCode { get; set; }
    }
}
