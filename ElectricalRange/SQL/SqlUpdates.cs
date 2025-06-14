using Dapper;
using Dapper.Contrib.Extensions;

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;


namespace ProjectsNow.SQL
{
    public static class SqlUpdates
    {
        public static List<SqlUpdate> Updates()
        {
            List<SqlUpdate> updates = new();

            IEnumerable<Type> types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "ProjectsNow.SQL.Updates" && t.BaseType == typeof(SqlUpdate));

            foreach (Type type in types)
            {
                SqlUpdate update = (SqlUpdate)Activator.CreateInstance(type);

                updates.Add(update);
            }

            return updates;
        }
        public static void CheckForUpdate()
        {
            using SqlConnection connection = new(Data.Database.ConnectionString);
            string query = "Select * From [SQL].[Updates]; ";
            List<SqlUpdate> newUpdates;
            List<SqlUpdate> updatesList = Updates();
            List<SqlUpdate> oldUpdates;

            try
            {
                oldUpdates = connection.Query<SqlUpdate>(query).ToList();
            }
            catch
            {
                oldUpdates = new List<SqlUpdate>();
            }

            if (updatesList.Count != 0)
            {
                updatesList = updatesList.OrderBy(u => u.Key).ToList();

                if (oldUpdates.Count == 0)
                {
                    newUpdates = updatesList;
                }
                else
                {
                    foreach (SqlUpdate update in oldUpdates)
                    {
                        SqlUpdate check = updatesList.FirstOrDefault(u => u.Key == update.Key);

                        if (check != null)
                        {
                            _ = updatesList.Remove(check);
                        }
                    }
                    newUpdates = updatesList;
                }

                foreach (SqlUpdate update in newUpdates)
                {
                    _ = connection.Execute(update.Command);
                    _ = connection.Insert(update);
                }
            }
        }
    }
}
