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

        /// <summary>
        /// Retrieves sale details for a book using its ISBN.
        /// </summary>
        /// <param name="isbn">The ISBN of the book.</param>
        /// <returns>Returns a SaleItem object if found, or null if no sale record exists.</returns>
        public static SaleItem? GetSaleItemByISBN(string isbn)
        {
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT isbn, quantity_sold, item_price FROM sale_item WHERE isbn = @isbn";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@isbn", isbn);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SaleItem(
                                    reader.GetString("isbn"),
                                    reader.GetInt32("quantity_sold"),
                                    reader.GetDecimal("item_price")
                                );
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSaleItemByISBN: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Removes a book from the sales records.
        /// </summary>
        /// <param name="customerId">The customer ID associated with the sale.</param>
        /// <param name="isbn">The ISBN of the book to be removed.</param>
        /// <returns>A string message indicating the success or failure of the operation.</returns>
        public static string RemoveBookFromSales(int customerId, string isbn)
        {
            using (MySqlConnection connection = GetConnection()) // Create a connection to the database
            {
                connection.Open(); // Open the connection

                // Check if the sale exists before deleting
                string checkQuery = "SELECT COUNT(*) FROM sales s JOIN sale_item si ON s.sale_id = si.sale_id WHERE s.customer_id = @customerId AND si.isbn = @isbn;";
                using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@customerId", customerId);
                    checkCommand.Parameters.AddWithValue("@isbn", isbn);

                    long saleExists = (long)checkCommand.ExecuteScalar();
                    if (saleExists == 0)
                    {
                        return $"No sale found for Customer ID {customerId} with ISBN {isbn}.";
                    }
                }

                // Delete the book from the sales record
                string deleteQuery = "DELETE FROM sales WHERE sale_id IN (SELECT sale_id FROM sale_item WHERE isbn = @isbn AND sale_id IN (SELECT sale_id FROM sales WHERE customer_id = @customerId));";
                ;
                using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@customerId", customerId);
                    deleteCommand.Parameters.AddWithValue("@isbn", isbn);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Book removed from sales successfully!" : "Failed to remove book from sales.";
                }
            }
        }

        /// <summary>
        /// Retrieves the next sale ID by finding the highest current sale ID in the sales table.
        /// </summary>
        /// <returns>The next available sale ID.</returns>
        public static int GetNextSaleId()
        {
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    string query = "SELECT MAX(sale_id) FROM sales"; // Get highest sale ID

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();

                        // Ensure the result is a valid integer
                        if (result == DBNull.Value || result == null)
                        {
                            return 1; // Start at 1 if no sales exist
                        }

                        return Convert.ToInt32(result) + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Sale ID: {ex.Message}");
                return -1; // Return -1 to indicate failure
            }
        }

        /// <summary>
        /// Adds books to a customer's sale by inserting a new record into the sales table.
        /// </summary>
        /// <param name="customerId">The ID of the customer making the purchase.</param>
        /// <param name="isbn">The ISBN of the book being purchased.</param>
        /// <param name="booksQuantity">The number of copies being purchased.</param>
        /// <param name="saleId">The sale ID for the transaction.</param>
        /// <returns>A string message indicating the success or failure of the operation.</returns>
        public static string AddBooksToCustomerSale(int customerId, string isbn, int booksQuantity)
        {
            using (MySqlConnection connection = GetConnection()) // Create connection
            {
                connection.Open(); // Open connection

                int saleId = GetNextSaleId(); // Fetch the next available sale ID from the database

                // Check if the customer exists
                string customerQuery = "SELECT COUNT(*) FROM customer WHERE customer_id = @customerId";
                using (MySqlCommand customerCommand = new MySqlCommand(customerQuery, connection))
                {
                    customerCommand.Parameters.AddWithValue("@customerId", customerId);
                    long customerExists = (long)customerCommand.ExecuteScalar();
                    if (customerExists == 0)
                    {
                        return $"Customer ID {customerId} not found.";
                    }
                }

                // Check if the book exists and get unit price
                double unitPrice = 0;
                string bookQuery = "SELECT unit_price FROM book WHERE isbn = @isbn";
                using (MySqlCommand bookCommand = new MySqlCommand(bookQuery, connection))
                {
                    bookCommand.Parameters.AddWithValue("@isbn", isbn);
                    object result = bookCommand.ExecuteScalar();

                    if (result == null)
                    {
                        return $"Book with ISBN {isbn} not found.";
                    }
                    unitPrice = Convert.ToDouble(result);
                }

                // Calculate total sale amount
                double totalAmount = unitPrice * booksQuantity;

                // Insert the sale with generated sale ID
                string insertQuery = "INSERT INTO sale_item(sale_id, isbn, quantity_sold, item_price) VALUES(@saleId, @isbn, @quantitySold, @itemPrice)";
                ;

                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@saleId", saleId); // Use dynamically generated sale ID
                    command.Parameters.AddWithValue("@customerId", customerId);
                    command.Parameters.AddWithValue("@isbn", isbn);
                    command.Parameters.AddWithValue("@saleDate", DateTime.Now);
                    command.Parameters.AddWithValue("@quantitySold", booksQuantity);
                    command.Parameters.AddWithValue("@totalSale", totalAmount);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0 ? "Sale added successfully!" : $"Failed to add sale for ISBN {isbn}.";
                }
            }
        }


    }
}



