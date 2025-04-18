using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace BookstorePointOfSale.DataViewModel
{
    /// <summary>
    /// Manager class for Database
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Connection string
        /// </summary>
        private static readonly string _connectionString = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            UserID = "root",
            Password = "12345678",
            Database = "bookstoredb"
        }.ConnectionString;

        /// <summary>
        /// Returns a connection to the database
        /// </summary>
        public static MySqlConnection GetConnection() => new MySqlConnection(_connectionString);
    }
}
