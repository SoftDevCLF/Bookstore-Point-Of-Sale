using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.Services
{
    public interface IAlertService
    {
        Task JSAlert(string message);
        Task<bool> ConfirmAlert(string message);
    }
}
