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
        public static int CreateSaleSession(int customerId)
        {
            try
            {
                string query = "INSERT INTO sales (customer_id, sale_date, total_sale) VALUES (@customerId, NOW(), 0.00); SELECT LAST_INSERT_ID();";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customerId", customerId);
                        // ExecuteScalar retrieves the LAST_INSERT_ID
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateSaleSession: {ex.Message}");
                return -1; // Return -1 to indicate failure
            }
        }

        //Method to add sale item
        public static bool AddSaleItem(int saleId, string isbn, int quantitySold, decimal itemPrice)
        {
            try
            {
                string query = "INSERT INTO sale_item (sale_id, isbn, quantity_sold, item_price) VALUES (@saleId, @isbn, @quantitySold, @itemPrice);";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        command.Parameters.AddWithValue("@isbn", isbn);
                        command.Parameters.AddWithValue("@quantitySold", quantitySold);
                        command.Parameters.AddWithValue("@itemPrice", itemPrice);

                        // Execute the command and check if it was successful
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; //Return true if rows were affected
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddSaleItem: {ex.Message}");
                return false; // Return false to indicate failure
            }
        }

        //Method to retrieve sale item using ISBN
        public static SaleItem? GetSaleItemByIsbn(string isbn)
        {
            try
            {
                SaleItem? saleItem = null;

                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();

                    string query = "SELECT s.isbn, s.quantity_sold, s.item_price FROM sale_item s WHERE s.isbn = @isbn;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@isbn", isbn);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Create a SaleItem object and populate it with data from the database
                                saleItem = new SaleItem(
                                    reader.GetString(0), // isbn
                                    reader.GetInt32(1), // quantity_sold
                                    reader.GetDecimal(2) // item_price
                                );
                            }
                        }
                    }
                }
                return saleItem; // Return the SaleItem object
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSaleItemByIsbn: {ex.Message}");
                return null; // Return null to indicate failure
            }
        }

        //Method to Confirm Sale
        public static bool ConfirmSale(int saleId)
        {
            try
            {
                string query = @"
            UPDATE sales 
            SET total_sale = (
                SELECT IFNULL(SUM(quantity_sold * item_price), 0) 
                FROM sale_item 
                WHERE sale_id = @saleId
            ) 
            WHERE sale_id = @saleId;";

                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConfirmSale: {ex.Message}");
                return false;
            }
        }


        //Method to Cancel Sale
        public static bool CancelSale(int saleId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    // Delete sale items first due to FK constraints
                    string deleteItems = "DELETE FROM sale_item WHERE sale_id = @saleId;";
                    using (var deleteItemsCommand = new MySqlCommand(deleteItems, connection))
                    {
                        deleteItemsCommand.Parameters.AddWithValue("@saleId", saleId);
                        deleteItemsCommand.ExecuteNonQuery();
                    }

                    // Then delete the sale itself
                    string deleteSale = "DELETE FROM sales WHERE sale_id = @saleId;";
                    using (var deleteSaleCommand = new MySqlCommand(deleteSale, connection))
                    {
                        deleteSaleCommand.Parameters.AddWithValue("@saleId", saleId);
                        int rowsAffected = deleteSaleCommand.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CancelSale: {ex.Message}");
                return false;
            }
        }


        //Method to Generate A Receipt
        public static string GenerateReceipt(int saleId)
        {
            var receiptBuilder = new StringBuilder();
            try
            {
                string query = @"SELECT s.sale_id, s.sale_date, c.first_name, c.last_name, 
                                i.isbn, i.quantity_sold, i.item_price 
                         FROM sales s
                         JOIN customer c ON s.customer_id = c.customer_id
                         JOIN sale_item i ON s.sale_id = i.sale_id
                         WHERE s.sale_id = @saleId";

                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    //Appending lines with formatting for UI
                                    receiptBuilder.AppendLine("========== BOOKSTORE RECEIPT ==========\n");
                                    receiptBuilder.AppendLine($"Sale ID      : {reader["sale_id"]}");
                                    receiptBuilder.AppendLine($"Customer     : {reader["first_name"]} {reader["last_name"]}");
                                    receiptBuilder.AppendLine($"Sale Date    : {Convert.ToDateTime(reader["sale_date"]).ToString("yyyy-MM-dd")}");
                                    receiptBuilder.AppendLine(new string('-', 40));
                                    receiptBuilder.AppendLine($"ISBN         : {reader["isbn"]}");
                                    receiptBuilder.AppendLine($"Qty Sold     : {reader["quantity_sold"]}");
                                    receiptBuilder.AppendLine($"Item Price   : {Convert.ToDecimal(reader["item_price"]).ToString("C")}");
                                    receiptBuilder.AppendLine(new string('-', 40));

                                    decimal quantity = Convert.ToDecimal(reader["quantity_sold"]);
                                    decimal price = Convert.ToDecimal(reader["item_price"]);
                                    receiptBuilder.AppendLine($"TOTAL        : {(quantity * price).ToString("C")}");
                                    receiptBuilder.AppendLine(new string('=', 40));
                                }
                            }
                            else
                            {
                                receiptBuilder.AppendLine("No sale items found for this sale ID.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateReceipt: {ex.Message}");
                receiptBuilder.AppendLine("Error generating receipt. Please try again.");
            }
            return receiptBuilder.ToString();
        }
    }
}




