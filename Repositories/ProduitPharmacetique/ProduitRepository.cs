using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class ProduitRepository : IProduitRepository
    {
        private readonly SqliteDbConnection _connection;

        public ProduitRepository(SqliteDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<TypeProduct>> GetAllProduitTypes()
        {
            var Result = await _connection.QueryAsync<TypeProduct>("SELECT * FROM Type");
            if(Result.Count() > 0)
            {
                return Result;
            }else 
            { 
                return  Enumerable.Empty<TypeProduct>();
            }

        }

        public async Task<DbResponse<ProduitPharma>> InsertProduit(ProduitPharma produit)
        {
            string sql = @"INSERT INTO PharmaceuticalProducts 
                            (Nom_Commercial,Forme,Dosage,Conditionnement) VALUES
                            (@Nom_Commercial,@Forme,@Dosage,@Conditionnement)";

            int s = await _connection.InsertDataAsync(sql, produit);

            if (s > 0)
            {
               
                return new DbResponse<ProduitPharma>
                {
                    Response = new ProduitPharma { Id = s ,Nom_Commercial= produit.Nom_Commercial,
                    Forme=produit.Forme,Dosage=produit.Dosage,Conditionnement=produit.Conditionnement}
                };
            }
            else
            {
                return new DbResponse<ProduitPharma>
                {
                    ErrorMessage = "Erreur de connexion à la base de données"
                };
            }
        }

        public async Task<DbResponse<PharmaceuticalProduct>> InsertProduit(PharmaceuticalProduct produit)
        {
            string sql = @"INSERT INTO PharmaceuticalProducts 
                            (Nom_Commercial,Forme,Dosage,Conditionnement,Img,Type) VALUES
                            (@Nom_Commercial,@Forme,@Dosage,@Conditionnement,@Img,@Type)";

            int s = await _connection.InsertDataAsync(sql, produit);

            if (s > 0)
            {
                produit.Id = s;
                return new DbResponse<PharmaceuticalProduct>
                {
                    Response = produit
                };
            }
            else
            {
                return new DbResponse<PharmaceuticalProduct>
                {
                    ErrorMessage = "Erreur de connexion à la base de données"
                };
            }
        }
    }
}
