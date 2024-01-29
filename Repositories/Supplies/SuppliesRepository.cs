using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class SuppliesRepository : ISuppliesRepository
    {
        private readonly SqliteDbConnection _connection;

        public SuppliesRepository(SqliteDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Supplies>> FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Enumerable.Empty<Supplies>();
            }
            string sql = "SELECT * FROM Supplies WHERE Nom = @word";
            var Result = await _connection.QueryAsync<Supplies>(sql, new {word=name});

            if (Result.Count() > 0)
            {
                return Result;
            }
            else
            {
                return Enumerable.Empty<Supplies>();

            }
        }

        public async Task<IEnumerable<Supplies>> GetAllSupplies()
        {
           
            string sql = "SELECT * FROM Supplies";
            var Result = await _connection.QueryAsync<Supplies>(sql);

            if (Result.Count() > 0)
            {
                return Result;
            }
            else
            {
                return Enumerable.Empty<Supplies>();

            }

        }

        public async Task<DbResponse<Supplies>> InsertSupplieAsync(Supplies supplies)
        {
            var invalidErrorMessage = "Veuillez fournir tous les détails requis pour créer un compte";

            var errorResponse = new DbResponse<Supplies> { ErrorMessage = invalidErrorMessage };

            if (supplies == null)
            {
                return errorResponse;
            }          

            string sql = @"INSERT INTO Supplies (Nom,Adresse,region)
                            VALUES (@Nom,@Adresse,@region)";
            int s = await _connection.InsertDataAsync(sql, supplies);
            if (s > 0)
            {
               

                return new DbResponse<Supplies>
                {
                    Response = new Supplies
                    {
                        Nom = supplies.Nom,
                        Adresse = supplies.Adresse,
                        Id = s,
                        fax = supplies.fax,
                        region = supplies.region
                    }
                };
            }
            else
            {
                return new DbResponse<Supplies>
                {
                    ErrorMessage = "Erreur de connexion à la base de données",
                };
            }

        }
    }
}
