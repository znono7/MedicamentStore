using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MedicamentStore
{
    public class StockRepository : IStockRepository
    {
        private readonly SqliteDbConnection _connection;

        public StockRepository(SqliteDbConnection connection)
        {
            _connection = connection;
        }
         

        public async Task<DbResponse<NewProduitPharmaStock>> AddNewStockAsync(ObservableCollection<NewProduitPharmaStock> newProducts)
        {
            using (var transaction = _connection.Connection().BeginTransaction()) 
            {
                try
                {
                    foreach (var item in newProducts) 
                    {
                        var parameters = new
                        {
                            Id = item.Id,
                            Quantite = item.Quantite,
                            IdSupplie = item.IdSupplie,
                            Prix = item.Prix,
                            Type = item.Type,
                            Date = item.Date,
                            Unit = item.Unit
                        };
                        await _connection.ExecuteAsync(transaction.Connection,@"INSERT INTO Stock (IdMedicament,Quantite,IdSupplie,Prix,Type,Date,Unit)
                                                                            VALUES (@Id,@Quantite,@IdSupplie,@Prix,@Type,@Date,@Unit)", 
                                             transaction: transaction, parameters) ;

                        int LastId = await _connection.ExecuteScalarTransaction<int>(transaction.Connection, "SELECT last_insert_rowid()", transaction: transaction);

                        string QeuryTrans = @"INSERT INTO Transaction (IdStock,TypeTransaction) VALUES (@LastIdStock,1)"; 
                        await _connection.ExecuteAsync(transaction.Connection, QeuryTrans, transaction: transaction, new { LastIdStock = LastId});
                    }

                    transaction.Commit();
                    return new DbResponse<NewProduitPharmaStock>();
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    return new DbResponse<NewProduitPharmaStock>
                    {
                        ErrorMessage = ex.Message,
                    };
                }
            }
        }
         
        public async Task<DbResponse> DeleteStockAsync(Stock stock)
        {
            using (var transaction = _connection.Connection().BeginTransaction())
            {
                try
                {
                    var parameters = new
                    {
                        id = stock.Id,
                       
                    };
                    await _connection.ExecuteAsync(transaction.Connection, @"DELETE FROM Stock WHERE Id = @id",
                                           transaction: transaction, parameters);
                    transaction.Commit();
                    return new DbResponse();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new DbResponse { ErrorMessage = "Échec de la Suppression du Stock" };

                }
            }
               
        }

        public async Task<double> GetAmountTotalStockAsync(ProduitsPharmaceutiquesType type)
        {
            int intValue = (int)type;
            if (intValue > 0)
                return await _connection.ExecuteScalar<double>("select sum(Quantite * Prix) AS AmountTotal from Stock WHERE Type =@Val", new {Val = intValue});
            else
                return await _connection.ExecuteScalar<double>("select sum(Quantite * Prix) AS AmountTotal from Stock");

        }

        public async Task<IEnumerable<MedicamentStock>> GetMedicamentStocksAsync(ProduitsPharmaceutiquesType type)
        {
            int intValue = (int)type;
            var parameters = new { Val = intValue };
            var baseQuery = @"SELECT m.Id , m.Nom_Commercial , m.Dosage , m.Forme ,m.Conditionnement, s.Quantite ,m.Img,s.Prix ,p.Nom ,s.Date,s.Id AS Ids,u.Name AS Unite
                            FROM Stock s INNER JOIN PharmaceuticalProducts m ON s.IdMedicament = m.Id
                                         INNER JOIN Supplies p ON p.Id = s.IdSupplie
                                         INNER JOIN Units u ON u.Id = s.Unit";

            var typeCondition = " WHERE s.Type = @Val";

            var finalQuery = type != ProduitsPharmaceutiquesType.None ? $"{baseQuery}{typeCondition}" : baseQuery;

            var resultFinal = await _connection.QueryAsync<MedicamentStock>(finalQuery, parameters);

            return resultFinal.Any() ? resultFinal : Enumerable.Empty<MedicamentStock>();


        }

        public async Task<IEnumerable<MedicamentStock>> GetPagedStocksAsync(int pageNumber, int pageSize, ProduitsPharmaceutiquesType type)
        {
            int intValue = (int)type;  
            int offset = (pageNumber - 1) * pageSize;

            var parameters = new { PageSize = pageSize, Offset = offset, Val = intValue };

            var baseQuery = @"SELECT m.Id, m.Nom_Commercial, m.Dosage, m.Forme, m.Conditionnement, s.Quantite, m.Img, s.Prix, p.Nom, s.Date, s.Id AS Ids
                        FROM Stock s
                        INNER JOIN PharmaceuticalProducts m ON s.IdMedicament = m.Id
                        INNER JOIN Supplies p ON p.Id = s.IdSupplie";

            var typeCondition = " WHERE s.Type = @Val";

            var finalQuery = type != ProduitsPharmaceutiquesType.None ? $"{baseQuery}{typeCondition}" : baseQuery;

            finalQuery += " ORDER BY s.Id DESC LIMIT @PageSize OFFSET @Offset;";

            var resultFinal = await _connection.QueryAsync<MedicamentStock>(finalQuery, parameters);

            return resultFinal.Any() ? resultFinal : Enumerable.Empty<MedicamentStock>();
        }

        public async Task<IEnumerable<ProductIds>> GetProductIdsAsync()
        {
            return await _connection.QueryAsync<ProductIds>("SELECT IdMedicament FROM Stock");
        }

        public async Task<IEnumerable<ProduitPharma>> GetProduitStocksAsync(string Word , int id)
        {
            
            
                string sql = @"SELECT m.Id , m.Nom_Commercial , m.Dosage , m.Forme ,m.Conditionnement,m.Img
                                FROM PharmaceuticalProducts m WHERE m.Nom_Commercial LIKE @mot OR m.DCI LIKE @mot";
                var ResultFinal = await _connection.QueryAsync<ProduitPharma>(sql, new {mot = $"%{Word}%" });

                if (ResultFinal.Count() > 0)
                {
                    return ResultFinal;
                }
                else
                {
                    return Enumerable.Empty<ProduitPharma>();

                }
            
           

        }

        public async Task<int> GetProduitTotalStockAsync(ProduitsPharmaceutiquesType type)
        {
            int intValue = (int)type;
            if(intValue > 0)
                return await _connection.ExecuteScalar<int>("SELECT COUNT(DISTINCT IdMedicament) AS NumberOfUniqueProducts FROM Stock WHERE Type = @Val ", new {Val = intValue}) ;
            else
                return await _connection.ExecuteScalar<int>("SELECT COUNT(DISTINCT IdMedicament) AS NumberOfUniqueProducts FROM Stock ");

        }

        public IEnumerable<Unite> GetUnitsAsync()
        {
            return  _connection.Query<Unite>("SELECT * FROM Units");

        }

        public async Task<DbResponse> UpdateStockAsync(MedicamentStock stock)
        {
            var s = "UPDATE Stock SET Quantite = @newQ WHERE Id = @I";

            var res = await _connection.ExecuteAsync(s, new {newQ = stock.Quantite,I = stock .Ids});

            if(res > 0)
            {
                return new DbResponse();
            }
            else
            {
                return new DbResponse { ErrorMessage = "ERREUR " };
            }
        }

        #region Stock Enter
       public async Task<DbResponse<StockEnter>> AddStockEnterAsync(StockEnter stockEnter)
        {
            using (var transaction = _connection.Connection().BeginTransaction())
            {
                try
                {
                    string sql = @"INSERT INTO StockEnter (IdStock,QuantiteAdded,Quantite,IdSupplie,Date)
                        VALUES (@IdStock,@QuantiteAdded,@Quantite,@IdSupplie,@Date)";
                    await _connection.ExecuteAsync(transaction.Connection,sql,transaction,stockEnter);
                    int NewQ = stockEnter.Quantite + stockEnter.QuantiteAdded;
                    string s = "UPDATE Stock SET Quantite = @newQ WHERE Id = @I";
                    await _connection.ExecuteAsync(transaction.Connection, s, transaction, new {newQ = NewQ , I = stockEnter.IdStock});
                    transaction.Commit();
                    return new DbResponse<StockEnter>();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new DbResponse<StockEnter>
                    {
                        ErrorMessage = ex.Message,
                    };
                }
            }

        }

        #endregion
    }
}
