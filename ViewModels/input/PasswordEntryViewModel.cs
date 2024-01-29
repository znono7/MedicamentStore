using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class PasswordEntryViewModel : BaseViewModel
    {
        /// <summary>
        /// The label to identify what this value is for
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// The current non-commit edited password
        /// </summary>
        public SecureString? Password { get; set; }

        
    }
}
