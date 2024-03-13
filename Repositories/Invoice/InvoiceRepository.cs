using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Collections.ObjectModel;

namespace MedicamentStore
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly SqliteDbConnection _connection;

        public InvoiceRepository(SqliteDbConnection connection)
        {
            _connection = connection;
        }
         
      
       

        public async Task<IEnumerable<Invoice>> GetAllInvoices(int pageNumber, int pageSize, int type) 
        { 
            int intValue = type;
            int offset = pageNumber * pageSize;
            var param = new { PageSize = pageSize, Offset = offset, Val = intValue };
            var Qeury = @"SELECT i.Id , i.Number , i.Date,i.MontantTotal,i.ProduitTotal,i.InvoiceType , s.Nom AS NomSupplie ,i.IdSupplie
                            FROM Invoice i LEFT JOIN Supplies s ON s.Id = i.IdSupplie";
            var typeCondition = " WHERE  i.InvoiceType = @Val  ";
            var finalQuery = type == 0 ? Qeury : Qeury + typeCondition;
            finalQuery += " ORDER BY i.Id DESC LIMIT @PageSize OFFSET @Offset;";

            var ResultFinal = await _connection.QueryAsync<Invoice>(finalQuery,param);
            return ResultFinal.Any() ? ResultFinal : Enumerable.Empty<Invoice>();

           
        }

       

        public async Task<IEnumerable<Invoice>> GetAllInvoicesByDate(int pageNumber, int pageSize, DateTime startDate, DateTime endDate, int type, int idSupplie = 0)
        {
            int offset = pageNumber * pageSize;

            var query = @"SELECT i.Id, i.Number, i.Date, i.MontantTotal, i.ProduitTotal, i.InvoiceType, s.Nom AS NomSupplie, i.IdSupplie
                FROM Invoice i
                INNER JOIN Supplies s ON s.Id = i.IdSupplie 
                WHERE i.Date BETWEEN @StartDate AND @EndDate";

            if (type != 0)
            {
                query += " AND i.InvoiceType = @Type";
            }

            if (idSupplie != 0)
            {
                query += " AND i.IdSupplie IS NOT NULL AND i.IdSupplie = @IdSupplie";
            }

            query += " ORDER BY i.Id DESC LIMIT @PageSize OFFSET @Offset;";

            var param = new { StartDate = startDate, EndDate = endDate, Type = type, IdSupplie = idSupplie, PageSize = pageSize, Offset = offset };

            var result = await _connection.QueryAsync<Invoice>(query, param);

            return result.Any() ? result : Enumerable.Empty<Invoice>();

        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesBySupplie(int pageNumber, int pageSize, int type, int idSupplie)
        {
            int intValue = type;
            int offset = pageNumber * pageSize;
            var param = new { PageSize = pageSize, Offset = offset, Val = intValue ,i = idSupplie};
            var Qeury = @"SELECT  i.Id , i.Number , i.Date,i.MontantTotal,i.ProduitTotal,i.InvoiceType , s.Nom AS NomSupplie ,i.IdSupplie
                            FROM Invoice i INNER JOIN Supplies s ON s.Id = i.IdSupplie 
                            WHERE i.IdSupplie IS NOT NULL AND i.IdSupplie = @i"; 
            var typeCondition = " WHERE  i.InvoiceType = @Val  ";
            var finalQuery = type == 0 ? Qeury : Qeury + typeCondition;
            finalQuery += " ORDER BY i.Id DESC LIMIT @PageSize OFFSET @Offset;";

            var ResultFinal = await _connection.QueryAsync<Invoice>(finalQuery, param);
            return ResultFinal.Any() ? ResultFinal : Enumerable.Empty<Invoice>();

        }

        public async Task<IEnumerable<InvoiceItemDto>> GetInvoiceItems(string num)
        {
            var Qeury = @"SELECT p.Nom_Commercial,p.Forme,p.Dosage,p.Conditionnement,p.Id AS IdMedicament ,s.Quantite ,
                            s.Prix , u.Name AS Unite , t.type AS TypeProduct  
                            FROM PharmaceuticalProducts p INNER JOIN InvoiceItem s ON s.IdProduct = p.IdProduct
                            INNER JOIN Units u ON u.Id = s.IdUnite
                            INNER JOIN Type t ON t.Id = s.IdTypeProduct  
                            WHERE s.InvoiceNumber = @nbr";  

            var ResultFinal = await _connection.QueryAsync<InvoiceItemDto>(Qeury, new { nbr = num }); 
            return ResultFinal.Any() ? ResultFinal : Enumerable.Empty<InvoiceItemDto>();
        }

        public async Task<int> GetLastInvoiceNumber() 
        {
                // Query to get the maximum invoice number, or default to 1 if the table is empty
                string query = "SELECT COALESCE(MAX(Id), 1) FROM Invoice";  
                return await _connection.ExecuteScalar<int>(query);  
        }

      
        public async Task<int> GetTotalInvoices(int type = 0)
        {
            if (type == 0)
                return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice  ");
            else
                return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice WHERE InvoiceType = @Val ", new { Val = type });

        }

        public async Task<int> GetTotalInvoicesByDate(DateTime startDate, DateTime endDate, int type, int idSupp)
        {
            return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice WHERE Date BETWEEN @StartDate AND @EndDate ", new { StartDate = startDate, EndDate = endDate });

            //if (idSupp == 0)
            //{
            //    if (type == 0)
            //        return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice WHERE Date BETWEEN @StartDate AND @EndDate ", new { StartDate = startDate, EndDate = endDate });
            //    else
            //        return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice WHERE Date BETWEEN @StartDate AND @EndDate AND InvoiceType = @Val  ", new { StartDate = startDate, EndDate = endDate, Val = type });

            //} 
            //else
            //{
            //    if (type == 0 )
            //        return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice WHERE Date BETWEEN @StartDate AND @EndDate AND IdSupplie = @id", new { StartDate = startDate, EndDate = endDate, id = idSupp });
            //    else
            //        return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice WHERE Date BETWEEN @StartDate AND @EndDate AND InvoiceType = @Val AND IdSupplie = @id ", new { StartDate = startDate, EndDate = endDate, Val = type, id = idSupp });

            //}

        }

        public async Task<int> GetTotalInvoicesBySupplie(int type, int id)
        {
            if (type == 0)
                return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice WHERE IdSupplie = @Val ", new { Val = id });
            else
                return await _connection.ExecuteScalar<int>("SELECT COUNT(Id) AS NumberOfInvoices FROM Invoice WHERE InvoiceType = @Val AND IdSupplie = @id ", new { Val = type, id });
           
        }

      

        public async Task<DbResponse> InsertInvoice(Invoice invoice, ObservableCollection<InvoiceItem> invoiceDetails)
        {
            using (var transaction = _connection.Connection().BeginTransaction()) 
            {
                try  
                {
                    string sql = @"INSERT INTO Invoice (Date,Number,MontantTotal,ProduitTotal,IdSupplie,InvoiceType) 
                                    VALUES (@Date,@Number,@MontantTotal,@ProduitTotal,@IdSupplie,@InvoiceType)";
                    await _connection.ExecuteAsync(transaction.Connection,sql,transaction, invoice);

                    int LastId = await _connection.ExecuteScalarTransaction<int>(transaction.Connection, "SELECT last_insert_rowid()", transaction: transaction);

                    //Insert Invoice Items
                    foreach (var itemInvoice in invoiceDetails)
                    {
                        itemInvoice.IdInvoice = LastId;
                        await _connection.ExecuteAsync(transaction.Connection,
                            @"INSERT INTO InvoiceItem (IdInvoice,InvoiceNumber,IdMedicament,IdTypeProduct,IdUnite,Quantite,Prix)                                             
                              VALUES (@IdInvoice,@InvoiceNumber,@IdMedicament,@IdTypeProduct,@IdUnite,@Quantite,@Prix);",
                                           transaction: transaction, itemInvoice);
                    }
                    foreach (var item in invoiceDetails)
                    {
                        var ProductStock = (await _connection.QueryAsync<Stock>("SELECT * FROM Stock WHERE Id=@idS", new { idS = item.IdStock })).SingleOrDefault() ;

                        if(ProductStock != null)
                        {
                            int PrevQte = ProductStock.Quantite ;
                            ProductStock.Quantite -= item.Quantite;

                            await _connection.ExecuteAsync(transaction.Connection, "UPDATE Stock SET Quantite=@Quantite WHERE Id=@Id", transaction,ProductStock);

                           
                            
                            string QeuryTrans = @"INSERT INTO [Transaction] (IdStock,TypeTransaction,QuantiteTransaction,Date,PreviousQuantity)
                                                                            VALUES (@LastIdStock,2,@q,@d,@PreviousQuantity)";
                            await _connection.ExecuteAsync(transaction.Connection, QeuryTrans, transaction: transaction, 
                                new { LastIdStock = ProductStock.Id , q = item.Quantite ,d =invoice.Date, PreviousQuantity = PrevQte.ToString() });

                        }


                    }
                    transaction.Commit();
                    return new DbResponse();
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    return new DbResponse
                    {
                        ErrorMessage = ex.Message,
                    };
                }
            }
        }

       
    }
}
