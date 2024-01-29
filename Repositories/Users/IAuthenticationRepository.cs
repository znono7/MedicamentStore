using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface IAuthenticationRepository
    {

        Task<DbResponse< LoginUserResult>> GetUserByUsernameAsync(LoginCredentials loginCredentials);
    }
}
