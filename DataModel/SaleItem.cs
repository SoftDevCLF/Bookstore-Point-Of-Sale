using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.DataModel
{
    /// <summary>
    /// Represents an item sold in a sale
    /// </summary>
    public class SaleItem
    {
        /// <summary>
        /// Property for ISBN of the sold item
        /// </summary>
        public string ISBN { get; set; }

        /// <summary>
        /// Property for the quantity of the item sold
        /// </summary>
        public int QuantitySold { get; set; }

        /// <summary>
        /// Property for the unit price of the item
        /// </summary>
        public decimal ItemPrice { get; set; }

        /// <summary>
        /// Constructs a SaleItem object
        /// </summary>
        /// <param name="isbn">ISBN of the book</param>
        /// <param name="quantitySold">Quantity of the book sold</param>
        /// <param name="price">Unit price of the book</param>
        public SaleItem(string isbn, int quantitySold, decimal itemPrice)
        {
            ISBN = isbn;
            QuantitySold = quantitySold;
            ItemPrice = itemPrice;
        }
    }
}
