using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MedicamentStore
{
    public class StockRepository : IStockRepository
    {
        private readonly SqliteDbConnection _connection;

        public StockRepository(SqliteDbConnection connection)
        {
            _connection = connection;
        }
         

         
        public async Task<DbResponse> DeleteStockAsync(int id)
        {
            using (var transaction = _connection.Connection().BeginTransaction())
            {
                try 
                {
                    var parameters = new
                    {
                        id,
                       
                    };
                    await _connection.ExecuteAsync(transaction.Connection, @"DELETE FROM Stock WHERE Id = @id",
                                           transaction: transaction, parameters);
                    await _connection.ExecuteAsync(transaction.Connection, @"DELETE FROM [Transaction] WHERE IdStock = @id",
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

        public async Task<IEnumerable<MedicamentStock>> GetPagedStocksAsync(int pageNumber, int pageSize, ProduitsPharmaceutiquesType type)
        {
            int intValue = (int)type;  
            int offset = pageNumber * pageSize;    
              
            var parameters = new { PageSize = pageSize, Offset = offset, Val = intValue };

            var baseQuery = @"SELECT  m.IdProduct, m.Nom_Commercial, m.Dosage, m.Forme, m.Conditionnement, s.Quantite, m.Img, 
                                s.Prix, p.Nom, s.Date, s.Id AS Ids ,u.Name AS Unite,u.Id AS IdUnite,m.Type
                        FROM Stock s
                        INNER JOIN PharmaceuticalProducts m ON s.IdProduct = m.IdProduct 
                        INNER JOIN Supplies p ON p.Id = s.IdSupplie
                        INNER JOIN Units u ON u.Id = s.Unit";

            var typeCondition = " WHERE m.Type = @Val";

            var finalQuery = type != ProduitsPharmaceutiquesType.None ? $"{baseQuery}{typeCondition}" : baseQuery;

            finalQuery += " ORDER BY s.Id DESC LIMIT @PageSize OFFSET @Offset;";

            var resultFinal = await _connection.QueryAsync<MedicamentStock>(finalQuery, parameters);

            return resultFinal.Any() ? resultFinal : Enumerable.Empty<MedicamentStock>();
        }

        public async Task<int> GetProduitTotalStockAsync(ProduitsPharmaceutiquesType type)
        {
            int intValue = (int)type; 
            if(intValue > 0)
                return await _connection.ExecuteScalar<int>("SELECT COUNT(DISTINCT IdProduct) AS NumberOfUniqueProducts FROM Stock WHERE Type = @Val ", new {Val = intValue}) ;
            else
                return await _connection.ExecuteScalar<int>("SELECT COUNT(DISTINCT IdProduct) AS NumberOfUniqueProducts FROM Stock ");

        } 

        public IEnumerable<Unite> GetUnitsAsync()
        {
            return  _connection.Query<Unite>("SELECT * FROM Units");

        }


        public async Task<DbResponse> UpdateStock(List<MedicamentUpdateStock> newProduits, Invoice invoice, List<InvoiceItem> invoiceItems)
        {
            using (var transaction = _connection.Connection().BeginTransaction()) 
            {
                try 
                {
                    // Insert Invoice
                    string sql = @"INSERT INTO Invoice (Date,Number,MontantTotal,ProduitTotal,IdSupplie,InvoiceType) 
                                    VALUES (@Date,@Number,@MontantTotal,@ProduitTotal,@IdSupplie,@InvoiceType)"; 

                    await _connection.ExecuteAsync(transaction.Connection, sql, transaction, invoice);

                    int LastId = await _connection.ExecuteScalarTransaction<int>(transaction.Connection, "SELECT last_insert_rowid()", transaction: transaction);

                    //Insert Invoice Items
                    foreach (var itemInvoice in invoiceItems)
                    {
                        itemInvoice.IdInvoice = LastId;
                        await _connection.ExecuteAsync(transaction.Connection,
                            @"INSERT INTO InvoiceItem (IdInvoice,InvoiceNumber,IdProduct,IdTypeProduct,IdUnite,Quantite,Prix)                                             
                              VALUES (@IdInvoice,@InvoiceNumber,@IdProduct,@IdTypeProduct,@IdUnite,@Quantite,@Prix);",
                                           transaction: transaction, itemInvoice);
                    }
                    foreach (var item in newProduits)
                    {

                        //Save Transactions
                        string QeuryTrans = @"INSERT INTO [Transaction] (IdStock,TypeTransaction,QuantiteTransaction,Date,PreviousQuantity,IdProduct)
                                            VALUES (@LastIdStock,1,@q,@d,@quant,@IdProduct)";
                        await _connection.ExecuteAsync(transaction.Connection, QeuryTrans, transaction: transaction, 
                            new { 
                                LastIdStock = item.IdStock,               
                                q = item.QuantiteAdded,              
                                d = invoice.Date,    
                                quant = item.Quantite.ToString(),
                                item.IdProduct
                            });

                        // Update Quantity
                        int NewQ = item.Quantite + item.QuantiteAdded;
                        string s = "UPDATE Stock SET Quantite = @newQ WHERE IdProduct = @I";
                        await _connection.ExecuteAsync(transaction.Connection, s, transaction, new { newQ = NewQ, I = item.IdProduct });

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

        public async Task<DbResponse> NewStock(List<NewProduitPharmaStock> newProduits, Invoice invoice, List<InvoiceItem> invoiceItems)
        {
            using (var transaction = _connection.Connection().BeginTransaction())
            {
                try
                {  //Insert PharmaceuticalProduct
                    foreach (var item in newProduits)
                    {
                        var parameters = new
                        {
                            item.IdProduct,
                            item.Nom_Commercial,
                            item.Dosage,
                            item.Forme,
                            item.Conditionnement,
                            item.Img,
                            item.Type,

                        };
                        await _connection.ExecuteAsync(transaction.Connection, @"INSERT INTO PharmaceuticalProducts (IdProduct,Nom_Commercial,Dosage,Forme,Conditionnement,Img,Type)
                                                                            VALUES (@IdProduct,@Nom_Commercial,@Dosage,@Forme,@Conditionnement,@Img,@Type)",
                                                                                                                        transaction: transaction, parameters);
                        if(item.imageSource != null && item.Img != null)
                            SaveImageToDisk(item.imageSource, item.Img);
                    }
                    // Insert Invoice
                    string sql = @"INSERT INTO Invoice (Date,Number,MontantTotal,ProduitTotal,IdSupplie,InvoiceType) 
                                    VALUES (@Date,@Number,@MontantTotal,@ProduitTotal,@IdSupplie,@InvoiceType)";

                    await _connection.ExecuteAsync(transaction.Connection, sql, transaction, invoice);

                    int LastInvoiceId = await _connection.ExecuteScalarTransaction<int>(transaction.Connection, "SELECT last_insert_rowid()", transaction: transaction);
                    //Insert Invoice Items
                    foreach (var itemInvoice in invoiceItems)
                    {
                        itemInvoice.IdInvoice = LastInvoiceId;
                        await _connection.ExecuteAsync(transaction.Connection,
                            @"INSERT INTO InvoiceItem (IdInvoice,InvoiceNumber,IdProduct,IdTypeProduct,IdUnite,Quantite,Prix)                                             
                              VALUES (@IdInvoice,@InvoiceNumber,@IdProduct,@IdTypeProduct,@IdUnite,@Quantite,@Prix);",
                                           transaction: transaction, itemInvoice);
                    }
                    // Insert Stocks
                    foreach (var item in newProduits)
                    {
                        var parameters = new
                        {
                            item.Id,
                            item.Quantite,
                            item.IdSupplie,
                            item.Prix,
                            item.Type,
                            item.Date,
                            Unit = item.SelectedUnite.Name == null ? 1 : item.SelectedUnite.Id ,
                            item.IdProduct
                        };
                        await _connection.ExecuteAsync(transaction.Connection, @"INSERT INTO Stock (IdProduct,Quantite,IdSupplie,Prix,Type,Date,Unit)
                                                                            VALUES (@IdProduct,@Quantite,@IdSupplie,@Prix,@Type,@Date,@Unit)",
                                             transaction: transaction, parameters);

                        int LastStockId = await _connection.ExecuteScalarTransaction<int>(transaction.Connection, "SELECT last_insert_rowid()", transaction: transaction);

                        //Insert Transactions
                        string QeuryTrans = @"INSERT INTO [Transaction] (IdStock,TypeTransaction,QuantiteTransaction,Date,PreviousQuantity,IdProduct) 
                                                    VALUES (@LastIdStock,1,@q,@d,'N/A',@IdProduct)";
                        await _connection.ExecuteAsync(transaction.Connection, QeuryTrans, transaction: transaction, 
                            new { LastIdStock = LastStockId, q = item.Quantite, d = item.Date , item.IdProduct });

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

       

        private void SaveImageToDisk(ImageSource image, string img)
        {


            // Convert the image source to a BitmapSource
            BitmapSource bitmapSource = image as BitmapSource;

            if (bitmapSource != null)
            {
                // Create a BitmapEncoder based on the desired image format (e.g., JPEG)
                BitmapEncoder encoder = new JpegBitmapEncoder(); // Change this to the desired format if needed

                // Create a FileStream to write the image data to a file on disk
                string filePath = $".\\Pictures\\{img}"; // Change this to your desired file path and name
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // Encode the bitmap source and write it to the file stream
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(fileStream);
                }

            }

        }

        public async Task<DbResponse<int>> GetLastProductNumber()
        {
            try
            {
                string query = "SELECT COALESCE(MAX(Id), 1) FROM PharmaceuticalProducts";
                var result = await _connection.ExecuteScalar<int?>(query); // Use int? to handle possible null result
                int lastProductNumber = result ?? 1; // If result is null, default to 1

                return new DbResponse<int>
                {
                    Response = lastProductNumber
                };
            }
            catch (Exception)
            {

                return new DbResponse<int>();
            }
           
            
        }
    }
}
