using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.Services
{
    public interface INavigationService
    {
        public interface INavigationService
        {
            void GoToCustomerManagementPage();
            void GoToAddNewCustomerPage();
            void GoToUpdateCustomerPage(int customerId);
            void GoToAddNewBookPage();
            void GoToEditBook();
            void GoToViewInventoryPage();
            void GoToSalesPage(int customerId);
        }
    }
}
