using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.DataModel
{
    public class Inventory : Book, IInventoryManager
    {
        public int Quantity { get; set; }
        public List<Book> Books { get; set; }

        public Inventory(string isbn, string title, string author, int edition, string editorial, string genre, string? commentaries, int quantity, double price) : base(isbn, title, author, edition, editorial, genre, commentaries)
        {
            Quantity = quantity;
            Price = price;
        }

        public void Add()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Search(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public void Sum()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public override void UpdateDetails()
        {
            throw new NotImplementedException();
        }
    }
}
