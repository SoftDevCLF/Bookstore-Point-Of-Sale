using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.Exceptions
{
    internal class DuplicateISBNException : ApplicationException
    {
        public DuplicateISBNException() { }
        public DuplicateISBNException(string message) : base(message) { }
    }
}
