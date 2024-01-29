using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class ClientAddedEventArgs : EventArgs
    {
        public Client NewClient { get; }

        public ClientAddedEventArgs(Client newClient)
        {
            NewClient = newClient;
        }
    }
}
