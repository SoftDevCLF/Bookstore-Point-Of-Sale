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

        /// <summary>
        /// Displays books available to purchase from database
        /// </summary>
        //public static List<Book> DisplayBooks()
        //{
        //    //Book books = null; // Initialize books to null
        //    //using (MySqlConnection connection = GetConnection()) //Create connection
        //    //{
        //    //    connection.Open(); //Open connection
        //    //    string sql = "SELECT * FROM books"; //SQL statement
        //    //    using (MySqlCommand command = new MySqlCommand(sql, connection)) //using command, execute query
        //    //    {
        //    //        using (MySqlDataReader reader = command.ExecuteReader()) //Execute reader
        //    //        {
        //    //            while (reader.Read()) //If reader has data
        //    //            {
        //    //                Console.WriteLine($"ISBN {reader.GetString(0)}, Book Title: {reader.GetString(1)}, Author: {reader.GetString(2)}, Edition: {reader.GetInt32(3)}, Editorial: {reader.GetString(4)}, Year: {reader.GetInt32(5)}, Genre: {reader.GetString(6)}, Comments: {reader.GetString(7)}, Unit Price: {reader.GetDecimal(8)}"); //Display book information
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //return books; //Return list of books
        //}
    }
}

