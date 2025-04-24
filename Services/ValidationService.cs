using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookstorePointOfSale.Services
{
    /// <summary>
    /// Class to manage customer validation
    /// </summary>
    public class ValidationService : IValidationService
    {
        /// <summary>
        /// Alert service to display alerts
        /// </summary>
        private readonly AlertService _alertService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="alertService">Alert service</param>
        public ValidationService( AlertService alertService)
        {
            
            _alertService = alertService;
        }

        /// <summary>
        /// Validates the phone number
        /// </summary>
        /// <returns>True if valid, false if not</returns>
        public async Task<bool> ValidatePhoneNumber(string phoneNumber)
        {

            if (String.IsNullOrEmpty(phoneNumber)) //Check if the phone number is empty
            {
                _alertService.JSAlert("Please enter a phone number.");
                return false;
            }
            else if (!phoneNumber.All(char.IsDigit)) //Check if the phone number is numeric
            {
                _alertService.JSAlert("Only numeric values are allowed for phone number.");
                return false;
            }
            else if (phoneNumber.Length != 10) //Check if the phone number is 10 digits
            {
                _alertService.JSAlert("Phone number must be 10 digits.");
                return false;
            }

            return true;

        }

        /// <summary>
        /// Validates the name
        /// </summary>
        /// <returns>True if valid, false if not</returns>
        public async Task<bool> ValidateName(string firstName, string lastName)
        {

            if (!firstName.All(char.IsLetter)) //Check if the first name contains only letters
            {
                await _alertService.JSAlert("First name should only contain letters.");
                return false;
            }

            if (!lastName.All(char.IsLetter)) //Check if the last name contains only letters
            {
                await _alertService.JSAlert("Last name should only contain letters.");
                return false;
            }

            return true; //Return true if the name is valid

        }
    }
}
