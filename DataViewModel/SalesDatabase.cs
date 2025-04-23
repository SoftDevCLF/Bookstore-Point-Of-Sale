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

        //Method to Calculate Total Quantity For Sale
        public static int CalculateTotalQuantityForSale(int saleId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    // Query to calculate the total quantity of items sold in a sale
                    string query = "SELECT SUM(quantity_sold) FROM sale_item WHERE sale_id = @saleId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculateTotalQuantityForSale: {ex.Message}");
            }
            return 0; // Return 0 if an error occurs or no records are found
        }

        //Method to Calculate Total Sale Amount
        public static int CalculateTotalAmountForSale(int saleId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    // Query to calculate the total sale amount for a sale
                    string query = "SELECT SUM(quantity_sold * item_price) FROM sale_item WHERE sale_id = @saleId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculateTotalSaleAmountForSale: {ex.Message}");
            }
            return 0; // Return 0 if an error occurs or no records are found
        }


        //Method to confirm sale
        public static string ConfirmSale(int saleId)
        {
            try
            {
                //Calculate totals
                int totalQuantity = CalculateTotalQuantityForSale(saleId);
                decimal totalCost = CalculateTotalAmountForSale(saleId);

                //Update the sales table with the total cost
                string updateSaleQuery = "UPDATE sales SET total_sale = @totalCost WHERE sale_id = @saleId;";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(updateSaleQuery, connection))
                    {
                        command.Parameters.AddWithValue("@totalCost", totalCost);
                        command.Parameters.AddWithValue("@saleId", saleId);
                        command.ExecuteNonQuery();
                    }
                }

                //Update inventory
                UpdateInventory(saleId);

                //Generate receipt
                string receipt = GenerateReceipt(saleId);

                // Return confirmation message and receipt
                return $"Sale confirmed successfully!\n\n{receipt}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConfirmSale: {ex.Message}");
                return $"Error confirming sale: {ex.Message}";
            }
        }

        //Method to generate receipt
        public static string GenerateReceipt(int saleId)
        {
            try
            {
                StringBuilder receipt = new StringBuilder();
                receipt.AppendLine($"Receipt for Sale ID: {saleId}");
                receipt.AppendLine("--------------------------------------------------");
                // Fetch sale items
                string query = "SELECT isbn, quantity_sold, item_price FROM sale_item WHERE sale_id = @saleId;";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string isbn = reader.GetString("isbn");
                                int quantitySold = reader.GetInt32("quantity_sold");
                                decimal itemPrice = reader.GetDecimal("item_price");
                                receipt.AppendLine($"ISBN: {isbn}, Quantity Sold: {quantitySold}, Item Price: {itemPrice:C}");
                            }
                        }
                    }
                }
                // Fetch total cost
                decimal totalCost = CalculateTotalAmountForSale(saleId);
                receipt.AppendLine($"--------------------------------------------------");
                receipt.AppendLine($"Total Cost: {totalCost:C}");
                return receipt.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateReceipt: {ex.Message}");
                return $"Error generating receipt: {ex.Message}";
            }
        }


        // Method to process sale items  stock
        public static void UpdateInventory(int saleId)
        {
            try
            {
                // Retrieve sale items for the given sale ID
                string query = "SELECT isbn, quantity_sold FROM sale_item WHERE sale_id = @saleId;";
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@saleId", saleId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string isbn = reader.GetString("isbn");
                                int quantitySold = reader.GetInt32("quantity_sold");

                                // Log or process the sale item without updating inventory
                                Console.WriteLine($"Processing Sale Item - ISBN: {isbn}, Quantity Sold: {quantitySold}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessSaleItems: {ex.Message}");
            }
        }
    }
}






