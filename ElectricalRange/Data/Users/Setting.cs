using Dapper.Contrib.Extensions;

namespace ProjectsNow.Data.Users
{
    [Table("[User].[Computers]")]
    public class Setting
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } //To Delete after finish new Log In Update

        [Write(false)]
        public string ComputerName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
