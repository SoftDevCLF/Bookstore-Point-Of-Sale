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


        /// <summary>
        /// Gets a customer by their ID
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="saleItem">SaleItem object</param>
        /// <param name="totalSale"> Total sale amount</param>
        /// <returns>Customer object or null if not found</returns>
        public static SaleItem AddSaleItem(SaleItem saleItem, int customerId, double totalSale)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open(); //Open connection
                MySqlTransaction transaction = connection.BeginTransaction(); //Start transaction

                string sql = "SELECT COUNT(*) FROM book WHERE isbn = @isbn";
                using (MySqlCommand sqlcommand = new MySqlCommand(sql, connection, transaction))
                {
                    sqlcommand.Parameters.AddWithValue("@isbn", saleItem.ISBN);
                    int count = Convert.ToInt32(sqlcommand.ExecuteScalar()); 
                    
                    //Check if the book exists
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
                        command.Parameters.AddWithValue("@customerId", customerId);  
                        command.Parameters.AddWithValue("@saleDate", DateTime.Now);  
                        command.Parameters.AddWithValue("@totalSale", totalSale);    
                        command.ExecuteNonQuery();
                    }

                    //Get Sale ID  
                    int saleId;
                    string getSaleIdQuery = "SELECT LAST_INSERT_ID();";

                    using (MySqlCommand getSaleIdCommand = new MySqlCommand(getSaleIdQuery, connection, transaction))
                    {
                        saleId = Convert.ToInt32(getSaleIdCommand.ExecuteScalar()); // Get the last inserted sale ID
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

                    //Update the inventory stock (decrease the stock)
                    string updateStockQuery = "UPDATE inventory SET book_stock = book_stock - @quantitySold WHERE isbn = @isbn;";
                    using (MySqlCommand updateCommand = new MySqlCommand(updateStockQuery, connection, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@isbn", saleItem.ISBN);
                        updateCommand.Parameters.AddWithValue("@quantitySold", saleItem.QuantitySold);
                        updateCommand.ExecuteNonQuery();
                    }
                    //Commit the transaction
                    transaction.Commit();
                }
            }
            //Return the sale item with the sale ID
            return saleItem;
        }


        /// <summary>
        /// Cancels a sale by removing the sale item from the database and updating the inventory stock
        /// </summary>
        /// <param name="saleItem">saleItem</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool CancelSale(SaleItem saleItem)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open(); //open connnection

                //Increase the inventory stock (opposite of confirm 
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

        /// <summary>
        /// Generates a receipt for a customer based on their ID
        /// </summary>
        /// <param name="saleItems" ></param>
        /// <param name="customerId"></param>
        /// <returns>Receipt builder item</returns>
        public static string GenerateReceipt(List<SaleItem> saleItems, int customerId)
        {
            //create a StringBuilder to build the receipt
            var receiptBuilder = new StringBuilder();
            double totalSale = 0;

            // Get customer info
            var customer = CustomerDatabase.GetCustomerById(customerId);
            if (customer == null)
            {
                return "Customer not found";
            }

            // Get current date/time
            var saleDate = DateTime.Now;
            // Format the date/time
            receiptBuilder.AppendLine("========== BOOKSTORE RECEIPT ==========\n");
            receiptBuilder.AppendLine($"Sale Date    : {saleDate.ToString("yyyy-MM-dd HH:mm:ss")}");
            receiptBuilder.AppendLine($"Customer     : {customer.FirstName} {customer.LastName}");
            receiptBuilder.AppendLine(new string('-', 40));

            // Process each item in the current sale
            foreach (var item in saleItems)
            {
                var book = InventoryDatabase.SearchByISBN(item.ISBN);
                if (book == null) continue;

                receiptBuilder.AppendLine($"ISBN         : {item.ISBN}");
                receiptBuilder.AppendLine($"Title        : {book.Title}");
                receiptBuilder.AppendLine($"Qty          : {item.QuantitySold}");
                receiptBuilder.AppendLine($"Unit Price   : {book.Price.ToString("C")}");

                double subtotal = item.QuantitySold * book.Price;
                totalSale += subtotal;

                receiptBuilder.AppendLine($"Subtotal     : {subtotal.ToString("C")}");
                receiptBuilder.AppendLine(new string('-', 40));
            }

            if (saleItems.Count > 0)
            {
                receiptBuilder.AppendLine(new string('=', 40));
                receiptBuilder.AppendLine($"GRAND TOTAL  : {totalSale.ToString("C")}");
                receiptBuilder.AppendLine(new string('=', 40));
            }
            else
            {
                // If no items in the sale, show a message
                receiptBuilder.AppendLine("No items in current sale.");
            }

            return receiptBuilder.ToString();
        }


        /// <summary>
        /// Report 1: Gets the total sales for a specific date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Total Sales By Date/returns>
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
                    //Adding parameter to the query
                    command.Parameters.AddWithValue("@date", date);
                    var totalSales = command.ExecuteScalar();
                    //If the total sales is null, set it to 0
                    if (totalSales == DBNull.Value)
                    {
                        totalSales = 0;
                    }
                    else
                    {
                        totalSales = Convert.ToDouble(totalSales);
                    }

                    double totalSalesDate = Convert.ToDouble(totalSales);

                    //Formatting for UI
                    Console.WriteLine($"Total sales for {date.ToString("yyyy-MM-dd")}: {totalSales}");
                    return totalSalesDate;
                }
            }
        }
        /// <summary>
        /// Report 2: Gets the total sales for a specific customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>Total Sales By Customer</returns>
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
                    //Adding parameter to the query
                    command.Parameters.AddWithValue("@customerId", customerId);
                    var totalSales = command.ExecuteScalar();
                    //If the total sales is null, set it to 0
                    if (totalSales == DBNull.Value)
                    {
                        totalSales = 0;
                    }
                    else
                    {
                        totalSales = Convert.ToDouble(totalSales);
                    }

                    double totalSalesCustomer = Convert.ToDouble(totalSales);

                    //Formatting for UI
                    Console.WriteLine($"Total sales for customer ID {customerId}: {totalSales}");
                    return totalSalesCustomer;
                }
            }
        }
        /// <summary>
        /// Report 3: Gets the total sales for a specific date period
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Total Sales with Date Period</returns>
        public static double GetTotalSalesByDatePeriod(DateTime startDate, DateTime endDate)
        {
            //SQL query to get total sales for a specific date period
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
                    //Adding parameters to the query
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    var totalSales = command.ExecuteScalar();

                    //If the total sales is null, set it to 0
                    if (totalSales == DBNull.Value)
                    {
                        totalSales = 0;
                    }
                    else
                    {
                        totalSales = Convert.ToDouble(totalSales);
                    }

                    //Formatting for UI
                    Console.WriteLine($"Total sales from {startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}: {totalSales}");

                    double totalSalesDatePeriod = Convert.ToDouble(totalSales);

                    return totalSalesDatePeriod;
                }
            }
        }
    }
}




