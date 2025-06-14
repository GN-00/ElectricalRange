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
