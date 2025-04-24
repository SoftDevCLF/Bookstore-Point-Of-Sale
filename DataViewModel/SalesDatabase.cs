using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using BookstorePointOfSale.DataModel;
using Microsoft.Maui.Controls;
using System.Data;



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

        //Method to add sale item //isbn, quantity, total, 
        public static SaleItem AddSaleItem(SaleItem saleItem)
        {
            int saleId = 0;//start sale ID at 0

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO sale_item (sale_id, isbn, quantity_sold) VALUES (@saleId, @isbn, @quantitySold);";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@saleId", saleItem.SaleId);
                    command.Parameters.AddWithValue("@isbn", saleItem.ISBN);
                    command.Parameters.AddWithValue("@quantitySold", saleItem.QuantitySold);
                    command.ExecuteNonQuery();
                }
            }
            saleItem.SaleId = saleId; //set the sale ID to the last inserted ID
            return saleItem; //return the sale item
        }

        //Method to confirm sale, reduces the quantity of the book in stock
        public static bool ConfirmSale(SaleItem saleItem)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                // 1. Reduce the inventory stock
                string updateStockQuery = "UPDATE book SET book_stock = book_stock - @quantitySold WHERE isbn = @isbn;";
                using (MySqlCommand updateCommand = new MySqlCommand(updateStockQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@isbn", saleItem.ISBN);
                    updateCommand.Parameters.AddWithValue("@quantitySold", saleItem.QuantitySold);
                    updateCommand.ExecuteNonQuery();
                }

                // 2. Add sale item to sale_item table
                AddSaleItem(saleItem);

                Console.WriteLine("Sale confirmed successfully.");
                return true;
            }
        }


        //Method to cancel sale, increases the quantity of the book in stock
       public static bool CancelSale(SaleItem saleItem)
{
    using (MySqlConnection connection = GetConnection())
    {
        connection.Open();

        // 1. Increase the inventory stock
        string updateStockQuery = "UPDATE book SET book_stock = book_stock + @quantitySold WHERE isbn = @isbn;";
        using (MySqlCommand updateCommand = new MySqlCommand(updateStockQuery, connection))
        {
            updateCommand.Parameters.AddWithValue("@isbn", saleItem.ISBN);
            updateCommand.Parameters.AddWithValue("@quantitySold", saleItem.QuantitySold);
            updateCommand.ExecuteNonQuery();
        }

        // 2. Remove the sale item from sale_item table
        string deleteQuery = "DELETE FROM sale_item WHERE sale_id = @saleId AND isbn = @isbn;";
        using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
        {
            deleteCommand.Parameters.AddWithValue("@saleId", saleItem.SaleId);
            deleteCommand.Parameters.AddWithValue("@isbn", saleItem.ISBN);
            deleteCommand.ExecuteNonQuery();
        }

        Console.WriteLine("Sale cancelled successfully.");
        return true;
    }
}




        ////Method to retrieve sale item using ISBN
        //public static SaleItem? GetSaleItemByIsbn(string isbn)
        //{
        //    try
        //    {
        //        SaleItem? saleItem = null;

        //        using (MySqlConnection connection = GetConnection())
        //        {
        //            connection.Open();

        //            string query = "SELECT s.isbn, s.book_title, s.quantity_sold, s.item_price FROM sale_item s WHERE s.isbn = @isbn;";

        //            using (MySqlCommand command = new MySqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@isbn", isbn);
        //                using (MySqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        // Create a SaleItem object and populate it with data from the database
        //                        saleItem = new SaleItem(
        //                            reader.GetString(0), // isbn
        //                            reader.GetString(1), // bookTitle
        //                            reader.GetInt32(2), // quantity_sold
        //                            reader.GetDecimal(3) // item_price
        //                        );
        //                    }
        //                }
        //            }
        //        }
        //        return saleItem; // Return the SaleItem object
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in GetSaleItemByIsbn: {ex.Message}");
        //        return null; // Return null to indicate failure
        //    }
        //}

        ////Method to retrieve sale item using TITLE
        //public static SaleItem? GetSaleItemByTitle(string bookTitle)
        //{
        //    try
        //    {
        //        SaleItem? saleItem = null;
        //        using (MySqlConnection connection = GetConnection())
        //        {
        //            connection.Open();
        //            string query = "SELECT s.isbn, s.book_title, s.quantity_sold, s.item_price FROM sale_item s WHERE s.book_title LIKE CONCAT('%', @bookTitle, '%');";
        //            using (MySqlCommand command = new MySqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@bookTitle", bookTitle);
        //                using (MySqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        // Create a SaleItem object and populate it with data from the database
        //                        saleItem = new SaleItem(
        //                            reader.GetString(0), // isbn
        //                            reader.GetString(1), // bookTitle
        //                            reader.GetInt32(2), // quantity_sold
        //                            reader.GetDecimal(3) // item_price
        //                        );
        //                    }
        //                }
        //            }
        //        }
        //        return saleItem; // Return the SaleItem object
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in GetSaleItemByTile: {ex.Message}");
        //        return null; // Return null to indicate failure
        //    }
        //}

        //Method to Generate A Receipt
        public static string GenerateReceipt(int saleId)
        {
            var receiptBuilder = new StringBuilder();

            string query = @"SELECT s.sale_id, s.sale_date, c.first_name, c.last_name, 
                            i.isbn, i.quantity_sold, b.unit_price, b.book_title
                     FROM sales s
                     JOIN customer c ON s.customer_id = c.customer_id
                     JOIN sale_item i ON s.sale_id = i.sale_id
                     JOIN book b ON i.isbn = b.isbn
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
                                // Appending lines with formatting for UI
                                receiptBuilder.AppendLine("========== BOOKSTORE RECEIPT ==========\n");
                                receiptBuilder.AppendLine($"Sale ID      : {reader["sale_id"]}");
                                receiptBuilder.AppendLine($"Customer     : {reader["first_name"]} {reader["last_name"]}");
                                receiptBuilder.AppendLine($"Sale Date    : {Convert.ToDateTime(reader["sale_date"]).ToString("yyyy-MM-dd")}");
                                receiptBuilder.AppendLine(new string('-', 40));
                                receiptBuilder.AppendLine($"ISBN         : {reader["isbn"]}");
                                receiptBuilder.AppendLine($"Book Title   : {reader["book_title"]}");
                                receiptBuilder.AppendLine($"Qty Sold     : {reader["quantity_sold"]}");
                                receiptBuilder.AppendLine($"Unit Price   : {Convert.ToDecimal(reader["unit_price"]).ToString("C")}");
                                receiptBuilder.AppendLine(new string('-', 40));

                                decimal quantity = Convert.ToDecimal(reader["quantity_sold"]);
                                decimal price = Convert.ToDecimal(reader["unit_price"]);
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

            return receiptBuilder.ToString();
        }

        //Report One: getting total sales by date
        public static void GetTotalSalesByDate(DateTime date)
        {
            string query = @"SELECT SUM(b.unit_price * i.quantity_sold) AS TotalSales
                            FROM sales s
                            JOIN sale_item i ON s.sale_id = i.sale_id
                            JOIN book b ON i.isbn = b.isbn
                            WHERE DATE(s.sale_date) = @date";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@date", date);
                    var totalSales = command.ExecuteScalar();
                    Console.WriteLine($"Total sales for {date.ToString("yyyy-MM-dd")}: {totalSales}");
                }
            }
        }

        //Report Two: getting total sales per customer
        public static void GetTotalSalesByCustomer(int customerId)
        {
            string query = @"SELECT SUM(b.unit_price * i.quantity_sold) AS TotalSales
                            FROM sales s
                            JOIN sale_item i ON s.sale_id = i.sale_id
                            JOIN book b ON i.isbn = b.isbn
                            WHERE s.customer_id = @customerId";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customerId", customerId);
                    var totalSales = command.ExecuteScalar();
                    Console.WriteLine($"Total sales for customer ID {customerId}: {totalSales}");
                }
            }
        }

        //Report Three: getting total sales per date period
        public static void GetTotalSalesByDatePeriod(DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT SUM(b.unit_price * i.quantity_sold) AS TotalSales
                            FROM sales s
                            JOIN sale_item i ON s.sale_id = i.sale_id
                            JOIN book b ON i.isbn = b.isbn
                            WHERE DATE(s.sale_date) BETWEEN @startDate AND @endDate";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    var totalSales = command.ExecuteScalar();
                    Console.WriteLine($"Total sales from {startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}: {totalSales}");
                }
            }
        }
    }
}





