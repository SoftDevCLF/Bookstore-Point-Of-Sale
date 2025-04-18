using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.Data
{
    public class Sales
    {
        public int SaleId { get; set; }
        public int CustomerId { get; set; } 
        public DateTime SaleDate { get; set; }
        public double TotalAmount { get; set; }
        public int BooksQuantity { get; set; }   

    }
}
