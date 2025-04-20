using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstorePointOfSale.DataModel;

namespace Library_Manager.Data
{
    /// <summary>
    /// Represents a Sales record
    /// </summary>
    public class Sales
    {
        /// <summary>
        /// Property for sale ID
        /// </summary>
        public int SaleId { get; set; }

        /// <summary>
        /// Property for customer ID associated with the sale
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Property for the date of the sale
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Property for the total amount of the sale
        /// </summary>
        public double TotalAmount { get; set; }

        /// <summary>
        /// Property for the quantity of books sold
        /// </summary>
        public int BooksQuantity { get; set; }

        /// <summary>
        /// One-to-many relationship: List of customers associated with the sale
        /// </summary>
        public List<Customer> Customers { get; set; } = new List<Customer>();

        /// <summary>
        /// One-to-many relationship: List of books included in the sale
        /// </summary>
        public List<Book> Books { get; set; } = new List<Book>();

        /// <summary>
        /// Constructs a Sales object
        /// </summary>
        /// <param name="saleId">Sale ID</param>
        /// <param name="customerId">Customer ID</param>
        /// <param name="saleDate">Date of sale</param>
        /// <param name="totalAmount">Total amount of sale</param>
        /// <param name="booksQuantity">Number of books sold</param>
        public Sales(int saleId, int customerId, DateTime saleDate, double totalAmount, int booksQuantity)
        {
            SaleId = saleId;
            CustomerId = customerId;
            SaleDate = saleDate;
            TotalAmount = totalAmount;
            BooksQuantity = booksQuantity;
            Customers = new List<Customer>();
            Books = new List<Book>();
        }
    }
}