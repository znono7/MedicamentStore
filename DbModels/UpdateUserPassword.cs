using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class UpdateUserPassword
    {
        #region Public Properties
        public int Id { get; set; }
        /// <summary>
        /// The users current password
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// The users new password
        /// </summary>
        public string NewPassword { get; set; }

        #endregion
    }
}
