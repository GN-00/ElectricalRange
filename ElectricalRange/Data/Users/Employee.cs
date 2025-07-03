using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Users
{
    [Table("[User].[Employees]")]
    internal class Employee 
    {
        [Key]
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeJob { get; set; }
        public int BankID { get; set; }
    }
}
using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Users
{
    [Table("[Users].[Employees]")]
    public class Employee : Base
    {
        [Key]
        public int Id { get; set; }
        
        private string _name;
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }
        
        private string _position;
        public string Position
        {
            get => _position;
            set => SetValue(ref _position, value);
        }
        
        private string _department;
        public string Department
        {
            get => _department;
            set => SetValue(ref _department, value);
        }
        
        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set => SetValue(ref _isActive, value);
        }
    }
}
