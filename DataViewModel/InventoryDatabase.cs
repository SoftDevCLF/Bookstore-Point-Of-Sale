using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstorePointOfSale.DataModel;
using BookstorePointOfSale.Exceptions;
using MySqlConnector;

namespace BookstorePointOfSale.DataViewModel
{
    /// <summary>
    /// Provides database operations for inventory and book records.
    /// </summary>
    public class InventoryDatabase : Database
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryDatabase"/> class.
        /// </summary>
        public InventoryDatabase() : base() { }

        /// <summary>
        /// Searches for a book and its inventory information by ISBN.
        /// </summary>
        /// <param name="isbn">The ISBN of the book to search for.</param>
        /// <returns>An <see cref="Inventory"/> object if found; otherwise, <c>null</c>.</returns>
        public static Inventory? SearchByISBN(string isbn)
        {
            Inventory? book = null;

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string sql = @"
                SELECT
                    b.isbn, b.book_title, b.author, b.edition, b.editorial,
                    b.year, b.genre, b.comments, b.unit_price, i.book_stock
                FROM book b
                JOIN inventory i ON b.isbn = i.isbn
                WHERE b.isbn = @isbn";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@isbn", isbn);
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        book = new Inventory(
                            reader.GetString("isbn"),
                            reader.GetString("book_title"),
                            reader.GetString("author"),
                            reader.GetInt32("edition"),
                            reader.GetString("editorial"),
                            reader.GetString("year"),
                            reader.GetString("genre"),
                            reader.IsDBNull("comments") ? null : reader.GetString("Comments"),
                            reader.GetInt32("book_stock"),
                            reader.GetDouble("unit_price")
                        );
                    }
                    reader.Close();
                }
            }
            return book;
        }

        /// <summary>
        /// Adds a new book and its stock level to the database.
        /// </summary>
        /// <param name="book">The <see cref="Inventory"/> object to add.</param>
        /// <returns>The inserted <see cref="Inventory"/> object.</returns>
        public static Inventory AddBook(Inventory book)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string checkSql = "SELECT COUNT(*) FROM book WHERE isbn = @isbn";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkSql, connection, transaction)) 
                    {
                        checkCommand.Parameters.AddWithValue("@isbn", book.ISBN);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0) 
                        {
                            throw new DuplicateISBNException($"Error Invalid ISBN! A book with ISBN {book.ISBN} already exists.");
                        }
                    }

                    string sql1 = @"
                        INSERT INTO book 
                            (isbn, book_title, author, edition, editorial, year, genre, comments, unit_price)
                        VALUES 
                            (@isbn, @title, @author, @edition, @editorial, @year, 
                            @genre, @comments, @unit_price)";

                    using (MySqlCommand command = new MySqlCommand(sql1, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@isbn", book.ISBN);
                        command.Parameters.AddWithValue("@title", book.Title);
                        command.Parameters.AddWithValue("@author", book.Author);
                        command.Parameters.AddWithValue("@edition", book.Edition);
                        command.Parameters.AddWithValue("@editorial", book.Editorial);
                        command.Parameters.AddWithValue("@year", book.Year);
                        command.Parameters.AddWithValue("@genre", book.Genre);
                        command.Parameters.AddWithValue("@comments", book.Comments ?? "");
                        command.Parameters.AddWithValue("@unit_price", book.Price);
                        command.ExecuteNonQuery();
                    }

                    string sql2 = "INSERT INTO inventory (isbn, book_stock) VALUES (@isbn, @stock)";
                    using (MySqlCommand command = new MySqlCommand(sql2, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@isbn", book.ISBN);
                        command.Parameters.AddWithValue("@stock", book.Quantity);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return book;
        }

        /// <summary>
        /// Updates an existing book and its inventory data in the database.
        /// </summary>
        /// <param name="book">The <see cref="Inventory"/> object containing updated data.</param>
        /// <returns><c>1</c> if successful; <c>0</c> otherwise.</returns>
        public static int UpdateBook(Inventory book)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string sql1 = @"
                    UPDATE book SET
                    book_title = @title,
                    author = @author,
                    edition = @edition,
                    editorial = @editorial,
                    year = @year,
                    genre = @genre,
                    comments = @comments,
                    unit_price = @price
                    WHERE isbn = @isbn";

                    using (MySqlCommand command = new MySqlCommand(sql1, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@isbn", book.ISBN);
                        command.Parameters.AddWithValue("@title", book.Title);
                        command.Parameters.AddWithValue("@author", book.Author);
                        command.Parameters.AddWithValue("@edition", book.Edition);
                        command.Parameters.AddWithValue("@editorial", book.Editorial);
                        command.Parameters.AddWithValue("@year", book.Year);
                        command.Parameters.AddWithValue("@genre", book.Genre);
                        command.Parameters.AddWithValue("@comments", book.Comments ?? "");
                        command.Parameters.AddWithValue("@price", book.Price);
                        command.ExecuteNonQuery();
                    }

                    string sql2 = "UPDATE inventory SET book_stock = @stock WHERE isbn = @isbn";
                    using (MySqlCommand command = new MySqlCommand(sql2, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@isbn", book.ISBN);
                        command.Parameters.AddWithValue("@stock", book.Quantity);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return 1;
                }
                catch
                {
                    transaction.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Deletes a book and its inventory record from the database using the given ISBN.
        /// </summary>
        /// <param name="isbn">The ISBN of the book to delete.</param>
        /// <returns><c>true</c> if deletion was successful; otherwise, <c>false</c>.</returns>
        public static bool DeleteBook(string isbn)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string deleteInventory = "DELETE FROM inventory WHERE isbn = @isbn";
                    using (MySqlCommand command = new MySqlCommand(deleteInventory, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@isbn", isbn);
                        command.ExecuteNonQuery();
                    }

                    string deleteBook = "DELETE FROM book WHERE isbn = @isbn";
                    using (MySqlCommand command = new MySqlCommand(deleteBook, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@isbn", isbn);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Retrieves all books with their inventory details from the database.
        /// </summary>
        /// <returns>A list of <see cref="Inventory"/> records.</returns>
        public static List<Inventory> GetAllBooks()
        {
            List<Inventory> inventoryList = new List<Inventory>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                string sql = @"
        SELECT 
            b.isbn, b.book_title, b.author, b.edition, b.editorial,
            b.year, b.genre, b.comments, b.unit_price, i.book_stock
        FROM book b
        JOIN inventory i ON b.isbn = i.isbn";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Inventory item = new Inventory(
                            reader.GetString("isbn"),
                            reader.GetString("book_title"),
                            reader.GetString("author"),
                            reader.GetInt32("edition"),
                            reader.GetString("editorial"),
                            reader.GetString("year"),
                            reader.GetString("genre"),
                            reader.IsDBNull("comments") ? null : reader.GetString("Comments"),
                            reader.GetInt32("book_stock"),
                            (double)reader.GetDecimal("unit_price")
                        );

                        inventoryList.Add(item);
                    }
                }
            }
            return inventoryList;
        }

        /// <summary>
        /// Searches for inventory books whose titles contain the specified keyword (case-insensitive).
        /// </summary>
        /// <param name="title">The partial or full title of the book to search for.</param>
        /// <returns>
        /// A list of <see cref="Inventory"/> objects that match the search criteria. 
        /// Returns an empty list if no matches are found.
        /// </returns>
        public static List<Inventory> SearchByTitle(string title)
        {
            List<Inventory> matchingBooks = new List<Inventory>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                // SQL query to search for books with matching title (case-insensitive)
                string sql = @"
            SELECT 
                b.isbn, b.book_title, b.author, b.edition, b.editorial,
                b.year, b.genre, b.comments, b.unit_price, i.book_stock
            FROM book b
            JOIN inventory i ON b.isbn = i.isbn
            WHERE LOWER(b.book_title) LIKE CONCAT('%', LOWER(@title), '%')";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    // Add the title as a parameter to prevent SQL injection
                    command.Parameters.AddWithValue("@title", title);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Read each result row and construct Inventory objects
                        while (reader.Read())
                        {
                            Inventory item = new Inventory(
                                reader.GetString("isbn"),
                                reader.GetString("book_title"),
                                reader.GetString("author"),
                                reader.GetInt32("edition"),
                                reader.GetString("editorial"),
                                reader.GetString("year"),
                                reader.GetString("genre"),
                                reader.IsDBNull("comments") ? null : reader.GetString("Comments"),
                                reader.GetInt32("book_stock"),
                                (double)reader.GetDecimal("unit_price")
                            );

                            matchingBooks.Add(item);
                        }
                    }
                }
            }

            return matchingBooks;
        }
    }
}
