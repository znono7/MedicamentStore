using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class TypeProduitViewModel : BaseViewModel
    {
        public Func<object, Task> CommitAction { get; set; }

        public ICommand SubmitCommand { get; set; }

        public TypeProduitViewModel()
        {
            SubmitCommand = new RelayParameterizedCommand(async (p) => await Submit(p));

        }

        public async Task Submit(object p)
        {
            await CommitAction(p);
        }

    }
}
