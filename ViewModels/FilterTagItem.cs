using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class FilterTagItem : BaseViewModel
    {
        public string TagName { get; set; }

        public ICommand RemoveCommand { get; set; }

        public Func<Task> RemoveAction { get; set; }
        public FilterTagItem() => RemoveCommand = new RelayCommand(async () => await Remove());

        private async Task Remove()
        {
            await RemoveAction();
        }
    }
}
