using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace BookstorePointOfSale.DataViewModel
{
    public class Database
    {
        private readonly string _connectionString = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "12345678",
            Database = "bookstoredb"
        }.ConnectionString;

        protected MySqlConnection GetConnection() => new MySqlConnection(_connectionString);
    }
}
