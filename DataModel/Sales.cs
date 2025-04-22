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
            SaleDate = DateTime.Now;
            Items = new List<SaleItem>();
            Customers = new List<Customer>();
            Books = new List<Book>();
        }

        /// <summary>
        /// Calculates the total amount of the sale by summing the price of each item
        /// </summary>
        public void CalculateTotalAmount()
        {
            TotalAmount = 0;
            foreach (var item in Items)
            {
                TotalAmount = Items.Sum(item => item.ItemPrice * item.QuantitySold);
            }
        }

        /// <summary>
        /// Caclulate the total book quantity of books
        /// </summary>
        public int GetTotalBookQuantity()
        {
            int totalQuantity = 0;
            foreach (var item in Items)
            {
                totalQuantity += item.QuantitySold;
            }
            return totalQuantity;
        }

    }
}