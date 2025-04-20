using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.DataModel
{
    /// <summary>
    /// Represents a Customer
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Property for customer id
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Property for customer first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Property for customer last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Property for customer email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Property for customer phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Provides immutable list of sales
        /// </summary>
        public List<Sales> Sales = new List<Sales>();

        /// <summary>
        /// Constructs Customer object
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="firstName">Customer first name</param>
        /// <param name="lastName">Customer last name</param>
        /// <param name="email">Customer email</param>
        /// <param name="phoneNumber">Customer phone number</param>
        public Customer(int customerId, string firstName, string lastName, string email, string phoneNumber)
        {
            CustomerId = customerId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        /// <summary>
        /// Constructs Customer object
        /// </summary>
        /// <param name="firstName">Customer first name</param>
        /// <param name="lastName">Customer last name</param>
        /// <param name="email">Customer email</param>
        /// <param name="phoneNumber">Customer phone number</param>
        public Customer(string firstName, string lastName, string email, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}

