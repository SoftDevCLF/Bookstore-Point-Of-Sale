using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.Data
{
    public class Customer : ISearch
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<Customer> Customers { get; set; } = new List<Customer>(); // List of customers>

        public void Search(string searchTerm)
        {
            throw new NotImplementedException();
        }
    }
}

