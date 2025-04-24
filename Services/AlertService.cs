using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BookstorePointOfSale.Services
{
    public class AlertService : IAlertService
    {
        /// <summary>
        /// JS Runtime to display alerts
        /// </summary>
        private readonly IJSRuntime _js;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="js"></param>
        public AlertService(IJSRuntime js) { _js = js; }

        /// <summary>
        /// Displays an alert
        /// </summary>
        /// <param name="message"> Message</param>
        public async Task JSAlert(string message)
        {
            await _js.InvokeVoidAsync("alert", message);
        }

        /// <summary>
        /// Displays a JavaScript confirm dialog and returns the result.
        /// </summary>
        /// <param name="message">The confirmation message.</param>
        /// <returns>True if confirmed, false otherwise.</returns>
        public async Task<bool> ConfirmAlert(string message)
        {
            return await _js.InvokeAsync<bool>("confirm", message);
        }
    }
}
