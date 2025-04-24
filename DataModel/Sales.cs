using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.DataModel
{
    public class Sales
    {
        public int SaleId { get; set; }
        public int CustomerId { get; set; }
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Property for the total amount of the sale
        /// </summary>
        public decimal TotalAmount { get; set; }

         /// <summary>
        /// One-to-many relationship: List of customers associated with the sale
        /// </summary>
        public List<Customer> Customers { get; set; } = new List<Customer>();

        /// <summary>
        /// One-to-many relationship: List of books included in the sale
        /// </summary>
        public List<Book> Books { get; set; } = new List<Book>();

        /// <summary>
        /// One-to-many relationship: List of books sold in the sale
        /// </summary>
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();

        /// <summary>
        /// Constructs a Sales object
        /// </summary>
        /// <param name="saleId">Sale ID</param>
        /// <param name="customerId">Customer ID</param>
        /// <param name="saleDate">Date of sale</param>

        public Sales(int saleId, int customerId, DateTime saleDate, decimal totalAmount)
        {
            SaleId = saleId;
            CustomerId = customerId;
            SaleDate = saleDate;
            Items = new List<SaleItem>();
            Customers = new List<Customer>();
            Books = new List<Book>();
        }
    }
}