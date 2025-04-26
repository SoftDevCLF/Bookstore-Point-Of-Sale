using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.Services
{
    public interface IValidationService
    {
        Task<bool> ValidateName(string firstName, string lastName);
        Task<bool> ValidatePhoneNumber(string phoneNumber);
        void ValidateEmail(string email);
        Task<bool> ValidateBookInputs(string isbn, string title, string author, int edition, string editorial, string year, string genre, double price, int quantity);
        Task<bool> ValidateIsbn(string isbn);
        Task<bool> ValidateYear(string year);
    }
}
