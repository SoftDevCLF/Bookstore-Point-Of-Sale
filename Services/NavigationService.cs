using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BookstorePointOfSale.Services
{
    public class NavigationService : INavigationService
    {
        private NavigationManager _navigationManager;


        public NavigationService(NavigationManager navigationManager) { _navigationManager = navigationManager; }

        /// <summary>
        /// Navigates back to the customer management page 
        /// </summary>
        public void GoToCustomerManagementPage()
        {
            _navigationManager.NavigateTo("/customerManagement");
        }

        /// <summary>
        /// Navigates to the add new customer page when clicking a button
        /// </summary>
        public void GoToAddNewCustomerPage()
        {
            _navigationManager.NavigateTo("/addNewCustomer"); 

        }

        /// <summary>
        /// Navigates to the update customer page and holds the customer id
        /// </summary>
        /// <param name="customerId"> Customer id</param>
        public async void GoToUpdateCustomerPage(int customerId)
        {
            _navigationManager.NavigateTo($"/updatecustomer/{customerId}");
        }

        /// <summary>
        /// Navigates to the sales page and holds the customer id
        /// </summary>
        /// <param name="customerId"></param>
        public async void GoToSalesPage(int customerId)
        {
            _navigationManager.NavigateTo($"/sales/{customerId}");
        }
    }
}
