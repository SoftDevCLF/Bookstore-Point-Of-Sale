using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library_Manager.Data;
using MySqlConnector;
using BookstorePointOfSale.DataModel;
using Microsoft.Maui.Controls;

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


        //Method to create a sale session
        public static string CreateSaleSession(int customerId)
        {
            try
            {
                //Generate new sale ID
                int saleId = GetNextSaleId(); //Method fetches the next sale ID
                if (saleId < 0)
                {
                    return "Error: Unable to fetch the next sale ID.";
                }
                string query = "INSERT INTO sales (sale_id, customer_id, sale_date) VALUES (@saleId, @customerId, @saleDate);";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        command.Parameters.AddWithValue("@customerId", customerId);
                        command.Parameters.AddWithValue("@saleDate", DateTime.Now);
                        command.ExecuteNonQuery();
                    }
                }
                return "Sale session created successfully.";
            }
            catch (Exception ex)
            {
                return $"Error in CreateSaleSession: {ex.Message}";
            }
        }

        //Method to get next SaleId() Autoincrement
        public static int GetNextSaleId()
        {
            try
            {
                //Query to get the maximum sale_id from the sales table
                string query = "SELECT MAX(sale_id) FROM sales;";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();
                        //If result is not null, increment it by 1
                        if (result != DBNull.Value)
                        {
                            return Convert.ToInt32(result) + 1;
                        }
                        else
                        {
                            return 1; // If no records exist, start from 1
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetNextSaleId: {ex.Message}");
                return -1; // Indicate an error
            }
        }

        //Get sale item using ISBN
        public static SaleItem? GetSaleItemByISBN(string isbn)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    // Query to fetch sale item details by ISBN, including sale_id
                    string query = "SELECT sale_item_id, sale_id, quantity_sold, item_price FROM sale_item WHERE isbn = @isbn";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@isbn", isbn.Trim()); // Clean up any whitespace from ISBN

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Use explicit casting to avoid conflicts
                                int saleItemId = reader.GetInt32(0);
                                int saleId = reader.GetInt32(1);
                                int quantitySold = reader.GetInt32(2);
                                decimal itemPrice = reader.GetDecimal(3);

                                return new SaleItem(saleItemId, isbn, quantitySold, itemPrice)
                                {
                                    SaleId = saleId
                                };
                            }

                        }
                    }
                }
                // Return null if no sale item is found
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSaleItemByISBN: {ex.Message}");
                return null; // Return null to indicate failure
            }
        }

        //Method to add books to the cart
        public static string AddBookToCart(int saleId, string isbn, int quantitySold, decimal itemPrice)
        {
            try
            {
                if (saleId <= 0) //Validate saleId
                {
                    return "Error: Invalid sale ID.";
                }

                string query = "INSERT INTO sale_item (sale_id, isbn, quantity_sold, item_price) VALUES (@saleId, @isbn, @quantitySold, @itemPrice);";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        command.Parameters.AddWithValue("@isbn", isbn.Trim()); // Clean up any whitespace from ISBN
                        command.Parameters.AddWithValue("@quantitySold", quantitySold);
                        command.Parameters.AddWithValue("@itemPrice", itemPrice);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Clear success message
                            return "The book was added to the cart successfully.";
                        }
                        else
                        {
                            // Clear failure message
                            return "The book could not be added to the cart. Please try again.";

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddBookToCart: {ex.Message}");
                return $"Error in AddBookToCart: {ex.Message}";
            }
        }








    }
}