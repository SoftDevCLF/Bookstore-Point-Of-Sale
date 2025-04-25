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
    }
}
