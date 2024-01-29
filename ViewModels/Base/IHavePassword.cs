using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface IHavePassword
    {
        /// <summary>
        /// The secure password
        /// </summary>
        SecureString SecurePassword { get; }
    }
}
