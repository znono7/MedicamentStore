using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class UserRepository : IUserRepository
    {
        private readonly SqliteDbConnection _connection;

        public UserRepository(SqliteDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<User>> FindByName(string name)
        {
            return await _connection.QueryAsync<User>("SELECT * FROM User WHERE UserName = @Username", new { Username = name });
                
        }

        public async Task<List<UserCredentials>> GetUsersAsync()
        {
            return (await _connection.QueryAsync<UserCredentials>("SELECT Id , Nom , Prenom , UserName , Type FROM User")).ToList();
        }

    
        public async Task<DbResponse<RegisterUserResult>> InsertUserAsync(User user)
        {
            var invalidErrorMessage = "Veuillez fournir tous les détails requis pour créer un compte";

            var errorResponse = new DbResponse<RegisterUserResult> { ErrorMessage = invalidErrorMessage };

            if (user == null)
            {
                return errorResponse;
            }

            if(string.IsNullOrWhiteSpace(user.UserName)|| string.IsNullOrWhiteSpace(user.Password))
            {
                return errorResponse;
            }
           
            bool userExist = (await FindByName(user.UserName)).Any();

            if (userExist)
            {
                return new DbResponse<RegisterUserResult>
                {
                    ErrorMessage = "L'utilisateur existe déjà"
                };
            }

            int s = await _connection.ExecuteAsync(@"INSERT INTO User (Nom,Prenom,Password,UserName,Type)
                                             VALUES (@Nom,@Prenom,@Password,@UserName,@Type)", user);

            if (s > 0)
            {
                var userIdentify = (await FindByName(user.UserName)).FirstOrDefault();
                
                return new DbResponse<RegisterUserResult>
                {
                    Response = new RegisterUserResult
                    {
                        Nom = userIdentify.Nom,
                        Prenom = userIdentify.Prenom,
                        UserName = userIdentify.UserName,
                        Type = userIdentify.Type,
                        Id = userIdentify.Id
                    }
                };
            }
            else
            {
                return new DbResponse<RegisterUserResult>
                {
                    ErrorMessage = "Erreur de connexion à la base de données",
                };
            }
        }

        public async Task<DbResponse> UpdateUserProfilAsync(UpdateUserProfile user)
        {
            string column = "";
            string _var = "";

            if (user.Nom != null)
            {
               
                column = "Nom";
                _var = user.Nom;
            }
            if (user.Prenom != null)
            {
                column = "Prenom";
                _var = user.Prenom;
            }
            if (user.UserName != null)
            {
                bool userExist = (await FindByName(user.UserName)).Any();
                if (userExist)
                {
                    return new DbResponse
                    {
                        ErrorMessage = "L'utilisateur existe déjà"
                    };
                }
                column = "UserName";
                _var = user.UserName;
            }

            string sql = "UPDATE User SET " + column + " = @Value WHERE Id = @UserId";
            int affectedRows = await _connection.ExecuteAsync(sql, new { Value = _var, UserId = user.Id });

            if (affectedRows > 0)
            {
                return new DbResponse();
            }
            else
            {
                return new DbResponse { ErrorMessage = "Erreur de modification" };
            }


        }

        public async Task<DbResponse> UpdateUserPasswordAsync(UpdateUserPassword user)
        {
            string sql = "UPDATE User SET Password = @Value WHERE Id = @UserId AND Password = @CurrentPass";
            int affectedRows = await _connection.ExecuteAsync(sql, new { Value = user.NewPassword, UserId = user.Id , CurrentPass = user.CurrentPassword});

            if (affectedRows > 0)
            {
                return new DbResponse();
            }
            else
            {
                return new DbResponse { ErrorMessage = "Échec de la modification du mot de passe" };
            }

        }

        public async Task<DbResponse> DeleteUserAsync(int id)
        {
            string sql = "DELETE FROM User WHERE Id = @IdUser";

            int x = await _connection.ExecuteAsync(sql,new { IdUser = id });

            if (x > 0)
            {
                return new DbResponse();
            }
            else
            {
                return new DbResponse { ErrorMessage = "Échec de la modification du mot de passe" };
            }
        }
    }
}
