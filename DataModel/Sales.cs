using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstorePointOfSale.DataModel;

namespace Library_Manager.Data
{
    public class Sales
    {
        public int SaleId { get; set; }
        public int CustomerId { get; set; } 
        public DateTime SaleDate { get; set; }
        public double TotalAmount { get; set; }
        public int BooksQuantity { get; set; }

        public List<Customer> Customers { get; set; } = new List<Customer>(); //1 -to-many relationship with Customer

        public List<Book> Books { get; set; } = new List<Book>(); //1 -to-many relationship with Book

        //Constructor for the Sales class
        public Sales(int saleId, int customerId, DateTime saleDate, double totalAmount, int booksQuantity)
        {
            this.SaleId = saleId;
            this.CustomerId = customerId;
            this.SaleDate = saleDate;
            this.TotalAmount = totalAmount;
            this.BooksQuantity = booksQuantity;
            Customers = new List<Customer>(); //Create the list inside of the constructor
            Books = new List<Book>(); //Create the list inside of the constructor
        }

    }
}
