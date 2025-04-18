using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstorePointOfSale.DataModel;
using Microsoft.Maui.Controls;
using MySqlConnector;

namespace BookstorePointOfSale.DataViewModel
{
    /// <summary>
    /// Manager class for Customer Database
    /// </summary>
    public class CustomerDatabase : Database
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomerDatabase() : base() { }

        /// <summary>
        /// Adds a customer to the database
        /// </summary>
        /// <param name="customer">Customer object</param>
        /// <returns>Customer ID</returns>
        public int AddCustomer (Customer customer)
        {
            int customerId = 0; //Start customer ID at 0
            using (MySqlConnection connection = GetConnection()) //Create connection
            {
                connection.Open(); //Open connection
                string sql = "INSERT INTO customer (first_name, last_name, email, phone) VALUES (@firstName, @lastName, @email, @phoneNumber)"; //SQL statement
                using (MySqlCommand command = new MySqlCommand(sql, connection)) //using command, execute query
                {
                    command.Parameters.AddWithValue("@firstName", customer.FirstName);
                    command.Parameters.AddWithValue("@lastName", customer.LastName);
                    command.Parameters.AddWithValue("@email", customer.Email);
                    command.Parameters.AddWithValue("@phoneNumber", customer.PhoneNumber);
                    command.ExecuteNonQuery();

                }
                sql = "SELECT LAST_INSERT_ID()"; //select last inserted ID
                using (MySqlCommand command = new MySqlCommand(sql, connection)) //using command, execute query
                {
                    customerId = Convert.ToInt32(command.ExecuteScalar()); //Convert to int and returns the first value
                }
                
            }

             return customerId;

        }

        /// <summary>
        /// Updates a customer in the database
        /// </summary>
        /// <param name="customer">Customer object</param>
        /// <returns>Customer ID</returns>
        public int UpdateCustomer(Customer customer)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();
                string sql = "UPDATE customer SET first_name = @firstName, last_name = @lastName, email = @email, phone = @phoneNumber WHERE customer_id = @id";
                
                using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@id", customer.CustomerId);
                    command.Parameters.AddWithValue("@firstName", customer.FirstName);
                    command.Parameters.AddWithValue("@lastName", customer.LastName);
                    command.Parameters.AddWithValue("@email", customer.Email);
                    command.Parameters.AddWithValue("@phoneNumber", customer.PhoneNumber);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                
            }

            return customer.CustomerId;
            
        }

        /// <summary>
        /// Deletes a customer from the database
        /// </summary>
        /// <param name="customer">Customer object</param>
        /// <returns>True if successful, false if not</returns>
        public bool DeleteCustomer(Customer customer)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();
                string sql = "DELETE FROM customer WHERE customer_id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@id", customer.CustomerId);
                    int rowsAffected = command.ExecuteNonQuery();
                    
                    if (rowsAffected == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                
            }
            
        }

        /// <summary>
        /// Searches for a customer by phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <returns>Customer object</returns>
        public static Customer Search(string phoneNumber)
        {
            Customer? customer = null;
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                string sql = "SELECT * FROM customer WHERE phone = @phoneNumber";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        customer = new Customer(reader.GetInt32("customer_id"), reader.GetString("first_name"),reader.GetString("last_name"), reader.GetString("email"),reader.GetString("phone"));
                                                
                    }
                    reader.Close();
                }
        
            }

            return customer;
        }
    }
}
