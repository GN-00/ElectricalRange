using Dapper;
using Dapper.Contrib.Extensions;

using ProjectsNow.Data.Application;
using ProjectsNow.Data.JobOrders;

using Microsoft.Data.SqlClient;
using System.Reflection;

namespace ProjectsNow.Data
{
    public static class Database
    {
        public static readonly int CompanyCreationYear = 2015;
        public static readonly long CompanyVAT = 311157442600003;
        public static readonly string CompanyName = "Electrical Range (ERPCAPS)";

        public static JobOrder Store => new() { ID = 0, Code = "Stock", CustomerName = "-", ProjectName = "-", CodeYear = 0 };
        public static string FactoryStoreName => "Stock Transfer";

        public static readonly string PSConnectionString = @"Server=tcp:erserver.database.windows.net,1433;Initial Catalog=ProjectsStore;Persist Security Info=False;User ID=erpcaps2022;Password=Wing00Gundam;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0";

        public static string ConnectionString
        {
            get
            {
                if (AppData.ComputerName == "DESKTOP-RKKR1NS")
                {
                    //return @"Data Source=DESKTOP-RKKR1NS\TRYDB;Initial Catalog=ERIDB;Integrated Security=False;User ID=sa;Password=2468;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
                    return @"Server=tcp:erserver.database.windows.net,1433;Initial Catalog=ProjectsNow;Persist Security Info=False;User ID=erpcaps2022;Password=Wing00Gundam;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0";
                    //return @"Server=mssql-203172-0.cloudclusters.net,10002;Initial Catalog=PN;Persist Security Info=False;User ID=Hassan;Password=H-h-2468-2468;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0";
                }
                //else if (AppData.ComputerName == "GN-00")
                //{
                //    return @"Server=localhost\SQLEXPRESS;Database=ProjectsNow;Trusted_Connection=True;TrustServerCertificate=True;";
                //    //return @"Server=tcp:erserver.database.windows.net,1433;Initial Catalog=ProjectsNow;Persist Security Info=False;User ID=erpcaps2022;Password=Wing00Gundam;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0";
                //}
                else
                {
                    //return @"Server=mssql-203172-0.cloudclusters.net,10002;Initial Catalog=PN;Persist Security Info=False;User ID=Hassan;Password=H-h-2468-2468;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=true;Connection Timeout=0";
                    //return @"Data Source=PCAPSSYSTEM\PROJECTSNOW;Initial Catalog=ProjectsNow;Integrated Security=False;User ID=sa;Password=Wing00Gundam;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
                    return @"Server=tcp:erserver.database.windows.net,1433;Initial Catalog=ProjectsNow;Persist Security Info=False;User ID=erpcaps2022;Password=Wing00Gundam;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0";
                }
            }
        }
        public static string InsertRecordWithID<T>() where T : new()
        {
            int propertiesToUpdateCount = 0;
            string query = $"Insert Into {((TableAttribute)typeof(T).GetCustomAttribute(typeof(TableAttribute))).Name} ";
            string columns = $"";
            string values = $"";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                WriteAttribute checkAttribute = (WriteAttribute)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(WriteAttribute));
                if (checkAttribute == null || checkAttribute.Write == true)
                {
                    columns += $"{(propertiesToUpdateCount == 0 ? " " : ", ")}{properties[i].Name}";
                    values += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}@{properties[i].Name}";
                }
            }
            return $"{query} ({columns}) Values ({values}) ";
        }
        public static void InsertSelect<T, T1>(this SqlConnection connection, string condition) where T : new()
        {
            int propertiesToUpdateCount = 0;
            string query = $"Insert Into {((TableAttribute)typeof(T).GetCustomAttribute(typeof(TableAttribute))).Name} ";
            string columns = $"";
            string values = $"Select ";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                KeyAttribute checkID = (KeyAttribute)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(KeyAttribute));
                WriteAttribute checkAttribute = (WriteAttribute)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(WriteAttribute));
                if ((checkAttribute == null || checkAttribute.Write == true)
                    && checkID == null)
                {
                    columns += $"{(propertiesToUpdateCount == 0 ? " " : ", ")}{properties[i].Name}";
                    values += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name}";
                }
            }

            _ = connection.Execute($"{query} ({columns}) {values} From {((TableAttribute)typeof(T1).GetCustomAttribute(typeof(TableAttribute))).Name} {condition}");
        }
    }
}