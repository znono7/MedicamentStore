using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface IUserRepository
    {

        Task<List<UserCredentials>> GetUsersAsync();
        Task<DbResponse<RegisterUserResult>> InsertUserAsync(User user);

        Task<IEnumerable<User>> FindByName(string name);

        Task<DbResponse> UpdateUserProfilAsync(UpdateUserProfile user);
        Task<DbResponse> UpdateUserPasswordAsync(UpdateUserPassword user);

        Task<DbResponse> DeleteUserAsync(int id);


    }
}
