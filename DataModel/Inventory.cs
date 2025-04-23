using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstorePointOfSale.DataViewModel;

namespace BookstorePointOfSale.DataModel
{

    /// <summary>
    /// Represents a book item in the inventory.
    /// Inherits from the abstract Book class.
    /// </summary>
    public class Inventory : Book
    {
        /// <summary>
        /// Quantity of the book available in stock.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Constructs a full Inventory object using all required details.
        /// </summary>
        /// <param name="isbn">ISBN of the book</param>
        /// <param name="title">Title of the book</param>
        /// <param name="author">Author of the book</param>
        /// <param name="edition">Edition number</param>
        /// <param name="editorial">Publishing house</param>
        /// <param name="year">Year of publication</param>
        /// <param name="genre">Genre or category</param>
        /// <param name="comments">Optional comments or notes</param>
        /// <param name="quantity">Quantity in stock</param>
        /// <param name="price">Price of the book</param>
        public Inventory(string isbn, string title, string author, int edition, string editorial, string year, string genre, string? comments, int quantity, double price)
            : base(isbn, title, author, edition, editorial, year, genre, comments, price)
        {
            Quantity = quantity;
            Price = price;
        }

        /// <summary>
        /// Constructs a simplified Inventory object (e.g., for search or deletion).
        /// </summary>
        /// <param name="isbn">ISBN of the book</param>
        public Inventory(string isbn)
            : base(isbn, "", "", 0, "", "", "", null, 0)
        {
            Quantity = 0;
            Price = 0;
        }

        /// <summary>
        /// Returns a formatted string with the inventory information.
        /// </summary>
        /// <returns>Multi-line string with book and stock details</returns>
        public override string DisplayInfo()
        {
            return $"{ISBN}\n{Title}\n{Author}\n{Edition}\n{Editorial}\n{Year}\n{Genre}\n{Comments}\nPrice: {Price:C}\nQuantity: {Quantity}";
        }
    }
}
