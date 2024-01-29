using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MedicamentStore
{
    public class AuthenticationRepository : IAuthenticationRepository
    {

        private readonly SqliteDbConnection _dbConnection;

        public AuthenticationRepository(SqliteDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<DbResponse<LoginUserResult>> GetUserByUsernameAsync(LoginCredentials loginCredentials)
        {
            string invalidErrorMessage = "Veuillez fournir tous les détails ";

            var errorResponse = new DbResponse<LoginUserResult> { ErrorMessage = invalidErrorMessage };

            if (loginCredentials == null)
            {
                return errorResponse;
            }

            if (string.IsNullOrWhiteSpace(loginCredentials.UserName) || string.IsNullOrWhiteSpace(loginCredentials.Password))
            {
                return errorResponse;
            }

            string query = "SELECT * FROM User WHERE UserName = @Username";
            var result = (await _dbConnection.QueryAsync<User>(query, new
            {
                Username = loginCredentials.UserName

            })).FirstOrDefault();

           
            if (result == null)
            {
                return new DbResponse<LoginUserResult> { ErrorMessage = "Aucun utilisateur trouvé" };

            }

            if (!string.Equals( loginCredentials.Password,result.Password))
            {
                return new DbResponse<LoginUserResult> { ErrorMessage = "Erreur de mot de passe" };

            }

            return new DbResponse<LoginUserResult>
            {
                Response = new LoginUserResult
                {
                    Id = result.Id,
                    Nom = result.Nom,
                    Prenom = result.Prenom,
                    UserName = result.UserName,
                    Type = result.Type,
                }
            };

        }
    }
}
