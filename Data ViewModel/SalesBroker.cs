using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library_Manager.Data;
using MySqlConnector;

namespace BookstorePointOfSale.Data_ViewModel
{
    public class SalesBroker : Database
    {
        public static void ReadSalesData()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Database = "bookstore_DB",
                UserID = "root",
                Password = "password"
            };

            //Create a connection to the database
            MySqlConnection connection = new MySqlConnection(builder.ConnectionString);

            //Open the connection
            connection.Open();

            //Create a command to execute the query
            string salesQuery = "SELECT * FROM sales";

            ////Create a command to execute the query
            //MySqlConnection command = new MySqlCommand(salesQuery, connection);

            //List<Sale> sales


        }
    }
}
