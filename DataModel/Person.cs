using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.DataModel
{
    /// <summary>
    /// Abstract representation of a person
    /// </summary>
    public abstract class Person
    {
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
        /// Default constructor
        /// </summary>
        public Person() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        public Person( string firstName, string lastName, string email, string phoneNumber)
        {
            
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
