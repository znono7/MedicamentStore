using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class NotificationManager : INotificationManager
    {
        public Task ShowMessage(NotificationBoxViewModel viewModel)
        {
            return new NotificationControl().ShowDialog(viewModel);
        }
    }
}
