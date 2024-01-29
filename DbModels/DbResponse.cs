using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class DbResponse
    {
        /// <summary>
        /// Indicates if the DB call was successful
        /// </summary>
        public bool Successful => ErrorMessage == null;

        /// <summary>
        /// The error message for a failed DB call
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// The DB response object
        /// </summary>
        public object Response { get; set; }

    }
    public class DbResponse<T>
    {
        /// <summary>
        /// Indicates if the DB call was successful
        /// </summary>
        public bool Successful => ErrorMessage == null;

        /// <summary>
        /// The error message for a failed DB call
        /// </summary>
        public  string? ErrorMessage { get; set; }

        /// <summary>
        /// The DB response object
        /// </summary>
        public  T? Response { get; set; }

    }
}
