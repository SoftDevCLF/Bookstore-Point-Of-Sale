using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstorePointOfSale.DataModel
{
    public interface IInventoryManager
    {
        void Add();
        void Update();
        void Delete();
        void Sum();
    }
}
