using System;

namespace ProjectsNow.SQL.Updates
{
    public class U202201191 : SqlUpdate
    {
        public U202201191()
        {
            Key = nameof(U202201191);
            Command = "Alter Table [Purchase].[Orders] Add " +
                      "Revise int, " +
                      "ReviseDate date, " +
                      "OriginalOrderID int, " +
                      "Revised bit DEFAULT(0) Not Null;";

            Date = DateTime.Now;
            IsDone = true;
        }
    }
}


