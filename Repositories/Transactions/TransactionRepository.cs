using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SqliteDbConnection _connection;

        public TransactionRepository(SqliteDbConnection connection)
        {
            _connection = connection; 
        }
       

        public async Task<IEnumerable<EnterTransaction>> GetAllEnter(string IdProduct) 
        {
            

            var baseQuery = @"SELECT  m.Nom_Commercial, m.Dosage, m.Forme, m.Conditionnement, 
                                      s.Quantite, m.Img, 
                                    s.Prix, p.Nom, t.Date  , t.TypeTransaction ,t.QuantiteTransaction ,
                                    m.Type , u.Name AS Unite,t.PreviousQuantity
                                FROM  [Transaction] t 
                                INNER JOIN Stock s ON s.Id = t.IdStock
                                INNER JOIN PharmaceuticalProducts m ON t.IdProduct = m.IdProduct
                                INNER JOIN Supplies p ON p.Id = t.IdSupplie 
                                INNER JOIN Units u ON u.Id = s.Unit 
                                WHERE t.TypeTransaction = 1 AND t.IdProduct = @IdProduct
                                ";

            try
            {
                var resultFinal = await _connection.QueryAsync<EnterTransaction>
                    (
                    baseQuery, 
                    new {  IdProduct }
                    );
                return resultFinal.Any() ? resultFinal : Enumerable.Empty<EnterTransaction>();

            }
            catch (Exception ex)
            {
                //display error message
                Console.WriteLine(ex.Message);


                throw;
            }

            
        }

      

        public async Task<IEnumerable<MouvementStocks>> GetAllMovement(string IdProduct)
        { 

            var baseQuery = @"SELECT t.Id, m.Nom_Commercial, m.Dosage, m.Forme, 
                                    m.Conditionnement, 
                                    m.Img, s.Prix, t.Date, s.Id AS IdStock , 
                                    t.TypeTransaction ,t.QuantiteTransaction , m.Type , 
                                    u.Name AS Unite ,t.PreviousQuantity , p.Nom AS Supplie
                                FROM  [Transaction] t
                                INNER JOIN Stock s ON s.Id = t.IdStock
                                INNER JOIN PharmaceuticalProducts m ON t.IdProduct = m.IdProduct
                                INNER JOIN Supplies p ON p.Id = t.IdSupplie 
                                INNER JOIN Units u ON u.Id = s.Unit 
                                WHERE t.IdProduct = @IdProduct ";

            var resultFinal = await _connection.QueryAsync<MouvementStocks>(baseQuery, new { IdProduct });

            return resultFinal.Any() ? resultFinal : Enumerable.Empty<MouvementStocks>();
        }

        public async Task<IEnumerable<EnterTransaction>> GetAllSorte(string IdProduct)
        {
            

            var baseQuery = @"SELECT t.Id, m.Nom_Commercial, m.Dosage, m.Forme, m.Conditionnement, s.Quantite, m.Img, 
                                    s.Prix, t.Date, s.Id AS IdStock , t.TypeTransaction ,t.QuantiteTransaction , 
                                    m.Type , u.Name AS Unite,u.Id AS IdUnite,t.PreviousQuantity
                                FROM  [Transaction] t  
                                INNER JOIN Stock s ON s.Id = t.IdStock
                                INNER JOIN PharmaceuticalProducts m ON t.IdProduct = m.IdProduct
                                INNER JOIN Units u ON u.Id = s.Unit 
                                WHERE t.TypeTransaction = 2 AND m.IdProduct = @IdProduct
                                ";


            var resultFinal = await _connection.QueryAsync<EnterTransaction>(baseQuery, new {  IdProduct });

            return resultFinal.Any() ? resultFinal : Enumerable.Empty<EnterTransaction>();
        }

        public async Task<int> GetTotalEnterStockAsync(string IdProduct)
        {
            return await _connection.ExecuteScalar<int>(@"SELECT COUNT(Id) AS NumberOfUniqueProducts 
                                                          FROM  [Transaction] WHERE IdProduct = @I AND TypeTransaction = 1"                              
                                                          , new {I = IdProduct });

        }

        public async Task<int> GetTotalMovmentStockAsync(string IdProduct)
        {
            return await _connection.ExecuteScalar<int>(@"SELECT COUNT(Id) AS NumberOfUniqueProducts 
                                                          FROM  [Transaction] WHERE IdProduct = @I", new { I = IdProduct });

        }

        public async Task<int> GetTotalSortieStockAsync(string IdProduct)
        {
            return await _connection.ExecuteScalar<int>(@"SELECT COUNT(Id) AS NumberOfUniqueProducts 
                                                          FROM  [Transaction] WHERE IdProduct = @I AND TypeTransaction = 2", new { I = IdProduct });
        }
    }
}
