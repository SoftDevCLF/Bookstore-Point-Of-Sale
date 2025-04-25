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

        /// <summary>
        /// Validates general book details such as title, author, quantity, and price.
        /// </summary>
        /// <param name="isbn">The ISBN of the book (handled separately)</param>
        /// <param name="title">The title of the book</param>
        /// <param name="author">The author of the book</param>
        /// <param name="edition">The edition number (not validated here)</param>
        /// <param name="editorial">The editorial name (not validated here)</param>
        /// <param name="year">The year of publication (not validated here)</param>
        /// <param name="genre">The genre of the book (not validated here)</param>
        /// <param name="price">The unit price of the book</param>
        /// <param name="quantity">The quantity in stock</param>
        /// <returns>True if all fields are valid; otherwise, false</returns>
        public async Task<bool> ValidateBookInputs(string isbn, string title, string author, int edition, string editorial, string year, string genre, double price, int quantity)
        {
            // Check if the title is provided
            if (string.IsNullOrWhiteSpace(title))
            {
                await _alertService.JSAlert("Title is required.");
                return false;
            }

            // Check if the author is provided
            if (string.IsNullOrWhiteSpace(author))
            {
                await _alertService.JSAlert("Author is required.");
                return false;
            }

            // Ensure edition is a positive number
            if (edition <= 0)
            {
                await _alertService.JSAlert("Edition is required and must be greater than 0.");
                return false;
            }

            // Editorial must not be empty
            if (string.IsNullOrWhiteSpace(editorial))
            {
                await _alertService.JSAlert("Editorial is required.");
                return false;
            }

            // Genre is required
            if (string.IsNullOrWhiteSpace(genre))
            {
                await _alertService.JSAlert("Genre is required.");
                return false;
            }

            // Price must not be negative
            if (price < 0)
            {
                await _alertService.JSAlert("Price cannot be negative.");
                return false;
            }

            // Quantity must not be negative
            if (quantity < 0)
            {
                await _alertService.JSAlert("Quantity cannot be negative.");
                return false;
            }

            return true; // All validations passed
        }


        /// <summary>
        /// Validates the ISBN to ensure it is numeric and exactly 13 characters long.
        /// </summary>
        /// <param name="isbn">The ISBN string to validate</param>
        /// <returns>True if the ISBN is valid; otherwise, false</returns>
        public async Task<bool> ValidateIsbn(string isbn)
        {
            // Check if the ISBN is empty or null
            if (string.IsNullOrWhiteSpace(isbn))
            {
                await _alertService.JSAlert("ISBN is required.");
                return false;
            }

            // Ensure all characters in ISBN are numeric
            if (!isbn.All(char.IsDigit))
            {
                await _alertService.JSAlert("ISBN must only contain numeric digits.");
                return false;
            }

            // Ensure ISBN has exactly 13 characters
            if (isbn.Length != 13)
            {
                await _alertService.JSAlert("ISBN must be exactly 13 characters.");
                return false;
            }

            return true; // ISBN passed all checks
        }

        /// <summary>
        /// Validates that the year is a 4-digit numeric string.
        /// </summary>
        /// <param name="year">The year input.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public async Task<bool> ValidateYear(string year)
        {
            // Check if the year field is empty or whitespace
            if (string.IsNullOrWhiteSpace(year))
            {
                await _alertService.JSAlert("Year is required.");
                return false;
            }

            // Check if all characters in the year are numeric
            if (!year.All(char.IsDigit))
            {
                await _alertService.JSAlert("Year must only contain digits.");
                return false;
            }

            // Check if the year is exactly 4 characters long
            if (year.Length != 4)
            {
                await _alertService.JSAlert("Year must be exactly 4 digits.");
                return false;
            }

            // All checks passed
            return true;
        }
    }
}
