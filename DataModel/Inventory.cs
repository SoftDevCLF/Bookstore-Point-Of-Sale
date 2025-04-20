using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.DataModel
{
    public class Inventory : Book
    {
        public int Quantity { get; set; }

        public Inventory(string isbn,string title,string author,int edition,string editorial,string year,string genre,string? comments,double price,int quantity) : base(isbn, title, author, edition, editorial, year, genre, comments, price)
        {
            Quantity = quantity;
        }

        public override string DisplayInfo()
        {
            return $"{ISBN}\n{Title}\n{Author}\n{Edition}\n{Editorial}\n{Year}\n{Genre}\n{Comments}\n{Price}\n{Quantity}";
        }
    }
}
