using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class ClientRepository : IClientRepository
    {
        private readonly SqliteDbConnection _connection;

        public ClientRepository(SqliteDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Client>> FindByName(string name)
        {
            return await _connection.QueryAsync<Client>("SELECT * FROM Client WHERE Name = @name", new { name = name });

        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            var result = await _connection.QueryAsync<Client>("SELECT * FROM Client");

            if (result == null)
            {
                return Enumerable.Empty<Client>();
            }
           
            return result;
        }

        public async Task<DbResponse<Client>> InsertClientAsync(Client client)
        {
            var invalidErrorMessage = "Veuillez fournir tous les détails requis pour créer un compte";

            var errorResponse = new DbResponse<Client> { ErrorMessage = invalidErrorMessage };

            if (client == null)
            {
                return errorResponse;
            }

            if (string.IsNullOrWhiteSpace(client.Name) || string.IsNullOrWhiteSpace(client.Adresse))
            {
                return errorResponse;
            }

            bool userExist = (await FindByName(client.Name)).Any();

            if (userExist)
            {
                return new DbResponse<Client>
                {
                    ErrorMessage = "Le Client existe déjà"
                };
            }

            int s = await _connection.ExecuteAsync(@"INSERT INTO Client (Name,Adresse)
                                             VALUES (@Name,@Adresse)", client);


            if (s > 0)
            {
                var clientIdentify = (await FindByName(client.Name)).FirstOrDefault();

                return new DbResponse<Client>
                {
                    Response = new Client
                    {
                       
                        Id = clientIdentify.Id,
                        Name = clientIdentify.Name,
                        Adresse = clientIdentify.Adresse,
                    }
                };
            }
            else
            {
                return new DbResponse<Client>
                {
                    ErrorMessage = "Erreur de connexion à la base de données"
                    
                };
            }

        }
    }
}
