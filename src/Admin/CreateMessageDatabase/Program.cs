using System;
using MySql.Data.MySqlClient;
using Paramore.Brighter.MessageStore.MySql;


namespace CreateMessageDatabase
{
    internal class Program
    {
        public const string TableNameMessages = "Messages";

        public static void Main(string[] args)
        {
            Console.WriteLine("Updating {0} with message tables", args[0]);
            SetupMessageDb(args[0]);
            Console.WriteLine("Done");
        }

        private static MySqlConnection SetupMessageDb(string connectionStringPath)
        {
            var connectionString = "DataSource=\"" + connectionStringPath + "\"";
            return CreateDatabaseWithTable(connectionString, MySqlMessageStoreBuilder.GetDDL(TableNameMessages));
        }

        private static MySqlConnection CreateDatabaseWithTable(string dataSourceTestDb, string createTableScript)
        {
            var sqlConnection = new MySqlConnection(dataSourceTestDb);

            sqlConnection.Open();
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = createTableScript;
                command.ExecuteNonQuery();
            }

            return sqlConnection;
        }
     }
}

