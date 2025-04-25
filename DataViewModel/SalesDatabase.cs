using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using BookstorePointOfSale.DataModel;
using Microsoft.Maui.Controls;
using System.Data;
using System.Transactions;




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
        public static SaleItem AddSaleItem(SaleItem saleItem, int customerId, double totalSale)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();


                string sql = "SELECT COUNT(*) FROM book WHERE isbn = @isbn";
                using (MySqlCommand sqlcommand = new MySqlCommand(sql, connection, transaction))
                {
                    sqlcommand.Parameters.AddWithValue("@isbn", saleItem.ISBN);
                    int count = Convert.ToInt32(sqlcommand.ExecuteScalar());
                    if (count == 0)
                    {
                        // Book not found, handle accordingly
                        Console.WriteLine("Book not found in the inventory.");
                        return null;
                    }

                    //Insert the sale into the sales table
                    string insertSaleQuery = "INSERT INTO sales (customer_id, sale_date, total_sale) VALUES (@customerId, @saleDate, @totalSale);";
                    using (MySqlCommand command = new MySqlCommand(insertSaleQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@customerId", customerId);  // Passed from UI or other code
                        command.Parameters.AddWithValue("@saleDate", DateTime.Now);  // Or use a specific date
                        command.Parameters.AddWithValue("@totalSale", totalSale);    // Calculate this based on items
                        command.ExecuteNonQuery();
                    }

                    //Get sale ID  
                    int saleId;
                    string getSaleIdQuery = "SELECT LAST_INSERT_ID();";
                    using (MySqlCommand getSaleIdCommand = new MySqlCommand(getSaleIdQuery, connection, transaction))
                    {
                        saleId = Convert.ToInt32(getSaleIdCommand.ExecuteScalar());
                    }
                    saleItem.SaleId = saleId; // Set the sale ID in the SaleItem object

                    //Insert into sale_item table
                    string sql2 = "INSERT INTO sale_item (sale_id, isbn, quantity_sold) VALUES (@sale_id, @isbn, @quantitySold)";
                    using (MySqlCommand sqlcommand2 = new MySqlCommand(sql2, connection, transaction))
                    {
                        sqlcommand2.Parameters.AddWithValue("@sale_id", saleItem.SaleId);
                        sqlcommand2.Parameters.AddWithValue("@isbn", saleItem.ISBN);
                        sqlcommand2.Parameters.AddWithValue("@quantitySold", saleItem.QuantitySold);
                        sqlcommand2.ExecuteNonQuery();
                    }

                    //Update the inventory stock
                    string updateStockQuery = "UPDATE inventory SET book_stock = book_stock - @quantitySold WHERE isbn = @isbn;";
                    using (MySqlCommand updateCommand = new MySqlCommand(updateStockQuery, connection, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@isbn", saleItem.ISBN);
                        updateCommand.Parameters.AddWithValue("@quantitySold", saleItem.QuantitySold);
                        updateCommand.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
            return saleItem;
        }



        //Method to cancel sale, increases the quantity of the book in stock
        public static bool CancelSale(SaleItem saleItem)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                //Increase the inventory stock
                string updateStockQuery = "UPDATE book SET book_stock = book_stock + @quantitySold WHERE isbn = @isbn;";
                using (MySqlCommand updateCommand = new MySqlCommand(updateStockQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@isbn", saleItem.ISBN);
                    updateCommand.Parameters.AddWithValue("@quantitySold", saleItem.QuantitySold);
                    updateCommand.ExecuteNonQuery();
                }

                //Remove the sale item from sale_item table
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
  

        //Method to Generate A Receipt
        public static string GenerateReceipt(int saleId)
        {
            var receiptBuilder = new StringBuilder();
            double totalSale = 0;

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
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@saleId", saleId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            receiptBuilder.AppendLine("========== BOOKSTORE RECEIPT ==========\n");
                            while (reader.Read())
                            {
                                // Appending lines with formatting for UI
                                 receiptBuilder.AppendLine($"Sale ID      :{reader["sale_id"]}");
                                receiptBuilder.AppendLine($"Customer     : {reader["first_name"]} {reader["last_name"]}");
                                receiptBuilder.AppendLine($"Sale Date    : {Convert.ToDateTime(reader["sale_date"]).ToString("yyyy-MM-dd")}");
                                receiptBuilder.AppendLine(new string('-', 40));
                                receiptBuilder.AppendLine($"ISBN         : {reader["isbn"]}");
                                receiptBuilder.AppendLine($"Book Title   : {reader["book_title"]}");
                                receiptBuilder.AppendLine($"Qty Sold     : {reader["quantity_sold"]}");
                                receiptBuilder.AppendLine($"Unit Price   : {Convert.ToDouble(reader["unit_price"]).ToString("C")}");
                                receiptBuilder.AppendLine(new string('-', 40));

                                double quantity = Convert.ToDouble(reader["quantity_sold"]);
                                double price = Convert.ToDouble(reader["unit_price"]);
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
        public static double GetTotalSalesByDate(DateTime date)
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

                    if (totalSales == DBNull.Value)
                    {
                        totalSales = 0;
                    }
                    else
                    {
                        totalSales = Convert.ToDouble(totalSales);
                    }
                    double totalSalesDate = Convert.ToDouble(totalSales);
                    Console.WriteLine($"Total sales for {date.ToString("yyyy-MM-dd")}: {totalSales}");
                    return totalSalesDate;
                }
            }
        }

        //Report Two: getting total sales per customer
        public static double GetTotalSalesByCustomer(int customerId)
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
                    if (totalSales == DBNull.Value)
                    {
                        totalSales = 0;
                    }
                    else
                    {
                        totalSales = Convert.ToDouble(totalSales);
                    }
                    double totalSalesCustomer = Convert.ToDouble(totalSales);
                    return totalSalesCustomer;
                }
            }
        }

        //Report Three: getting total sales per date period
        public static double GetTotalSalesByDatePeriod(DateTime startDate, DateTime endDate)
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
                    if (totalSales == DBNull.Value)
                    {
                        totalSales = 0;
                    }
                    else
                    {
                        totalSales = Convert.ToDouble(totalSales);
                    }
                    double totalSalesDatePeriod = Convert.ToDouble(totalSales);
                    return totalSalesDatePeriod;
                }
            }
        }
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



