using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.Data
{
    /// <summary>
    /// Abstract representation of a book
    /// </summary>
    public abstract class Book
    {

        public string ISBN { get; set; }  // ISBN is the unique identifier
        public string Title { get; set; }
        public string Author { get; set; }
        public int Edition { get; set; }
        public string Editorial { get; set; }
        public string Year { get; set; }
        public string Genre { get; set; }
        public string? Comments { get; set; }
        public double Price { get; set; }
        

        /// <summary>
        /// Constructor for book
        /// </summary>
        /// <param name="isbn">ISBN number</param>
        /// <param name="title">Title</param>
        /// <param name="author">Author</param>
        /// <param name="edition">Edition</param>
        /// <param name="editorial">Editorial</param>
        /// <param name="genre">Category</param>
        /// <param name="comments">Commentaries</param>
        public Book(string isbn, string title, string author, int edition, string editorial, string year, string genre, string? comments = null, double price = 0)
        {
            ISBN = isbn;
            Title = title;
            Author = author;
            Edition = edition;
            Editorial = editorial;
            Year = year;
            Genre = genre;
            Comments = comments;
            Price = price;

        }

        // Abstract method for updating book details
        public abstract void UpdateDetails();








    }
}

