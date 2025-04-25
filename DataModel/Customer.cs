using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstorePointOfSale.Components.Pages;

namespace BookstorePointOfSale.DataModel
{
    /// <summary>
    /// Represents a Customer
    /// </summary>
    public class Customer: Person
    {
        /// <summary>
        /// Property for customer id
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Provides immutable list of sales
        /// </summary>
        public List<Sales> Sales = new List<Sales>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public Customer(): base()
        { }

        /// <summary>
        /// Constructs Customer object
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="firstName">Customer first name</param>
        /// <param name="lastName">Customer last name</param>
        /// <param name="email">Customer email</param>
        /// <param name="phoneNumber">Customer phone number</param>
        public Customer(int customerId, string firstName, string lastName, string email, string phoneNumber): base(firstName, lastName, email, phoneNumber)
        {
            CustomerId = customerId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        /// <summary>
        /// Constructs Customer object to add to database
        /// </summary>
        /// <param name="firstName">Customer first name</param>
        /// <param name="lastName">Customer last name</param>
        /// <param name="email">Customer email</param>
        /// <param name="phoneNumber">Customer phone number</param>
        public Customer(string firstName, string lastName, string email, string phoneNumber): base(firstName, lastName, email, phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}