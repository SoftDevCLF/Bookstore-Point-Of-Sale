using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstorePointOfSale.DataModel;

using MySqlConnector;

namespace BookstorePointOfSale.DataViewModel
{
    internal class InventoryDatabase : Database
    {
        public InventoryDatabase() : base() { } 

        public static Inventory AddBook(Inventory book) 
        {
            using (MySqlConnection connection = GetConnection()) 
            {
                connection.Open();
                string sql = @"INSERT INTO books 
                                (ISBN, Title, Author, Edition, Editorial, Genre, Comments, Quantity, Price) 
                                VALUES 
                                (@ISBN, @Title, @Author, @Edition, @Editorial, @Genre, @Comments, @Quantity, @Price)";

                using (MySqlCommand command = new MySqlCommand(sql, connection)) 
                {
                    command.Parameters.AddWithValue("@ISBN", book.ISBN);
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@Edition", book.Edition);
                    command.Parameters.AddWithValue("@Editorial", book.Editorial);
                    command.Parameters.AddWithValue("@Genre", book.Genre);
                    command.Parameters.AddWithValue("@Comments", book.Comments ?? string.Empty);
                    command.Parameters.AddWithValue("@Quantity", book.Quantity);
                    command.Parameters.AddWithValue("@Price", book.Price);

                    command.ExecuteNonQuery();
                }
            }
            return book;
        }

        public int UpdateBook(Inventory book) 
        {
            using (MySqlConnection connection = GetConnection()) 
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                string sql = @"UPDATE books SET
                                Title = @Title,
                                Author = @Author, 
                                Edition = @Edition,
                                Genre = @Genre,
                                Comments = @Comments,
                                Quantity = @Quantity,
                                Price = @Price
                                WHERE ISBN = @ISBN";

                using (MySqlCommand command = new MySqlCommand(sql, connection, transaction)) 
                {
                    command.Parameters.AddWithValue("@ISBN", book.ISBN);
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@Edition", book.Edition);
                    command.Parameters.AddWithValue("@Editorial", book.Editorial);
                    command.Parameters.AddWithValue("@Genre", book.Genre);
                    command.Parameters.AddWithValue("@Comments", book.Comments ?? string.Empty);
                    command.Parameters.AddWithValue("@Quantity", book.Quantity);
                    command.Parameters.AddWithValue("@Price", book.Price);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 1) 
                    {
                        transaction.Commit();
                    }
                    else 
                    {
                        transaction.Rollback();
                    }
                    return rowsAffected;
                }
            }
        }

        public bool DeleteBook(string isbn) 
        {
            using (MySqlConnection connection = GetConnection()) 
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                string sql = "DELETE FROM books WHERE ISBN = @ISBN";

                using (MySqlCommand command = new MySqlCommand(sql, connection, transaction)) 
                {
                    command.Parameters.AddWithValue("@ISBN", isbn);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 1) 
                    {
                        transaction.Commit();
                        return true;
                    }
                    else 
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public static Inventory? SearchByISBN(string isbn) 
        {
            Inventory? book = null;

            using (MySqlConnection connection = GetConnection()) 
            {
                connection.Open();

                string sql = "SELECT * FROM books WHERE ISBN = @ISBN";

                using (MySqlCommand command = new MySqlCommand(sql, connection)) 
                {
                    command.Parameters.AddWithValue("@ISBN", isbn);
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read()) 
                    {
                        book = new Inventory(
                            reader.GetString("ISBN"),
                            reader.GetString("Title"),
                            reader.GetString("Author"),
                            reader.GetInt32("Edition"),
                            reader.GetString("Editorial"),
                            reader.GetString("Genre"),
                            reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                            reader.GetInt32("Quantity"),
                            reader.GetDouble("Price")
                        );
                    }
                    reader.Close();
                }
            }
            return book;
        }

        public static List<Inventory> GetAllBooks() 
        {
            List<Inventory> books = new List<Inventory>();

            using (MySqlConnection connection = GetConnection()) 
            {
                connection.Open();
                string sql = "SELECT * FROM books";

                using (MySqlCommand command = new MySqlCommand(sql, connection)) 
                {
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read()) 
                    {
                        Inventory book = new Inventory(
                            reader.GetString("ISBN"),
                            reader.GetString("Title"),
                            reader.GetString("Author"),
                            reader.GetInt32("Edition"),
                            reader.GetString("Editorial"),
                            reader.GetString("Genre"),
                            reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                            reader.GetInt32("Quantity"),
                            reader.GetDouble("Price")
                        ); 
                        books.Add(book);
                    }
                    reader.Close();
                }
                return books;
            }
        }
    }
}
