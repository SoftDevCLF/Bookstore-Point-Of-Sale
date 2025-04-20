using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library_Manager.Data;
using MySqlConnector;
using BookstorePointOfSale.DataModel;

namespace BookstorePointOfSale.DataViewModel
{
    /// <summary>
    /// Manager class for Sales Database
    /// </summary>
    public class SalesDatabase : Database
    {
        /// <summary>
        /// Parameterless Constructor
        /// </summary>
        public SalesDatabase() : base() { }


        /// <summary>
        /// Searches for a customer by customer ID
        /// </summary>
        /// <param name="customerId">Customer Idr</param>
        /// <returns>customer object</returns>

        public static Customer SearchId(int customerId)
        {
            Customer customer = null; //Initialize customer to null
            using (MySqlConnection connection = GetConnection()) //Create connection
            {
                connection.Open(); //Open connection
                string sql = "SELECT * FROM customer WHERE customer_id = @customerId"; //SQL statement
                using (MySqlCommand command = new MySqlCommand(sql, connection)) //using command, execute query
                {
                    command.Parameters.AddWithValue("@customerId", customerId);
                    using (MySqlDataReader reader = command.ExecuteReader()) //Execute reader
                    {
                        if (reader.Read()) //If reader has data
                        {
                            customer = new Customer(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)); //Create customer object
                        }
                    }
                }
            }
            return customer; //Return customer object


        }
    }
}

