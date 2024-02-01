﻿using System;
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
         
        public async Task<IEnumerable<InvoiceProduct>> GetAllInvoiceProduct()
        {
            var Qeury = @"SELECT p.Nom_Commercial,p.Forme,p.Dosage,p.Conditionnement,p.Id AS ProductId ,p.Img,s.Quantite AS QuantiteRest,
                            s.Prix , u.Name AS Unite , t.type AS Type
                            FROM PharmaceuticalProducts p INNER JOIN Stock s ON s.IdMedicament = p.Id
                            INNER JOIN Units u ON u.Id = s.Unit
                            INNER JOIN Type t ON t.Id = s.Type";
            var ResultFinal = await _connection.QueryAsync<InvoiceProduct>(Qeury);

            if (ResultFinal.Count() > 0)
            {
                return ResultFinal;
            }
            else
            {
                return Enumerable.Empty<InvoiceProduct>();

            }
        }

        public async Task<IEnumerable<InvoiceProduct>> GetAllInvoiceProduct(int type)
        { 
           
            var Qeury = @"SELECT p.Nom_Commercial,p.Forme,p.Dosage,p.Conditionnement,p.Id AS ProductId ,p.Img,s.Quantite AS QuantiteRest,
                            s.Prix , u.Name AS Unite , t.type AS Type ,s.Id AS IdS
                            FROM PharmaceuticalProducts p INNER JOIN Stock s ON s.IdMedicament = p.Id
                            INNER JOIN Units u ON u.Id = s.Unit
                            INNER JOIN Type t ON t.Id = s.Type 
                            WHERE s.Type = @type";
            var ResultFinal = await _connection.QueryAsync<InvoiceProduct>(Qeury, new { type });

            if (ResultFinal.Count() > 0)
            {
                return ResultFinal;
            }
            else
            {
                return Enumerable.Empty<InvoiceProduct>();

            }
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoices()
        {
            var Qeury = "SELECT * FROM Invoice";

            var ResultFinal = await _connection.QueryAsync<Invoice>(Qeury);

            if (ResultFinal.Count() > 0)
            {
                return ResultFinal;
            }
            else
            {
                return Enumerable.Empty<Invoice>();

            }
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoices(int pageNumber, int pageSize)
        {
            int offset = (pageNumber - 1) * pageSize;
            var Qeury = "SELECT * FROM Invoice   ORDER BY Id DESC LIMIT @PageSize OFFSET @Offset;";

            var ResultFinal = await _connection.QueryAsync<Invoice>(Qeury, new { PageSize = pageSize, Offset = offset });

            if (ResultFinal.Count() > 0)
            {
                return ResultFinal;
            }
            else
            {
                return Enumerable.Empty<Invoice>();

            }
        }

        public async Task<int> GetLastInvoiceNumber()
        {
                // Query to get the maximum invoice number, or default to 1 if the table is empty
                string query = "SELECT COALESCE(MAX(Id), 1) FROM Invoice";
                return await _connection.ExecuteScalar<int>(query); 
        }

        public async Task<IEnumerable<InvoiceProduct>> GetSearchInvoiceProduct(string word)
        {
            var Qeury = @"SELECT p.Nom_Commercial,p.Forme,p.Dosage,p.Conditionnement,p.Id AS ProductId ,p.Img,s.Quantite AS QuantiteRest,
                            s.Prix , u.Name AS Unite , t.type AS Type , ,s.Id AS IdS
                            FROM PharmaceuticalProducts p INNER JOIN Stock s ON s.IdMedicament = p.Id
                            INNER JOIN Units u ON u.Id = s.Unit
                            INNER JOIN Type t ON t.Id = s.Type 
                            WHERE p.Nom_Commercial LIKE @mot OR p.DCI LIKE @mot";
            var ResultFinal = await _connection.QueryAsync<InvoiceProduct>(Qeury,new {mot = $"%{word}%"});

            if (ResultFinal.Count() > 0)
            {
                return ResultFinal;
            }
            else
            {
                return Enumerable.Empty<InvoiceProduct>();

            }
        }

        public async Task<DbResponse<Invoice>> InsertInvoice(Invoice invoice)
        {
            string sql = @"INSERT INTO Invoice (Date,Number,MontantTotal,ProduitTotal)
                            VALUES (@Date,@Number,@MontantTotal,@ProduitTotal)";
            int s = await _connection.InsertDataAsync(sql, invoice);
            if (s > 0)
            {
                return new DbResponse<Invoice>
                {
                    Response = new Invoice
                    { 
                        Id = s,
                        Date = invoice.Date,
                        Number = invoice.Number,
                        MontantTotal = invoice.MontantTotal,
                        ProduitTotal = invoice.ProduitTotal
                    }
                };
            }
            else
            {
                return new DbResponse<Invoice>
                {
                    ErrorMessage = "Erreur de connexion à la base de données",
                };
            }
        }

        public async Task<DbResponse> InsertInvoice(Invoice invoice, ObservableCollection<InvoiceProduct> invoiceDetails)
        {
            using (var transaction = _connection.Connection().BeginTransaction()) 
            {
                try
                {
                    string sql = @"INSERT INTO Invoice (Date,Number,MontantTotal,ProduitTotal)
                            VALUES (@Date,@Number,@MontantTotal,@ProduitTotal)";
                    await _connection.ExecuteAsync(transaction.Connection,sql,transaction, invoice);
                    foreach (var item in invoiceDetails)
                    {
                        var ProductStock = (await _connection.QueryAsync<Stock>("SELECT * FROM Stock WHERE Id=@idS", new { idS = item.IdS })).SingleOrDefault() ;

                        if(ProductStock != null)
                        {
                            ProductStock.Quantite -= item.Quantite;

                            await _connection.ExecuteAsync(transaction.Connection, "UPDATE Stock SET Quantite=@Quantite WHERE Id=@Id", transaction,ProductStock);

                            await _connection.ExecuteAsync(transaction.Connection, @"INSERT INTO InvoiceDetail (InvoiceNumber,ProductId,Quantite,QuantiteRest,PrixTotal)
                                                                            VALUES (@InvoiceNumber,@ProductId,@Quantite,@QuantiteRest,@PrixTotal);
                                                                                   ",
                                            transaction: transaction, item);

                            string QeuryTrans = @"INSERT INTO Transaction (IdStock,TypeTransaction) VALUES (@LastIdStock,2)";
                            await _connection.ExecuteAsync(transaction.Connection, QeuryTrans, transaction: transaction, new { LastIdStock = ProductStock.Id });

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

        public async Task<DbResponse<InvoiceDetail>> InsertInvoiceDetail(ObservableCollection<InvoiceProduct> invoiceDetails)
        {
            using (var transaction = _connection.Connection().BeginTransaction()) 
            {
                try
                {
                    foreach (var item in invoiceDetails)
                    {
                        await _connection.ExecuteAsync(transaction.Connection, @"INSERT INTO InvoiceDetail (InvoiceNumber,ProductId,Quantite,QuantiteRest,PrixTotal)
                                                                            VALUES (@InvoiceNumber,@ProductId,@Quantite,@QuantiteRest,@PrixTotal);
                                                                                     UPDATE Stock SET Quantite = Quantite - @Quantite WHERE ",
                                             transaction: transaction, item);
                    }

                    transaction.Commit();
                    return new DbResponse<InvoiceDetail>();
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    return new DbResponse<InvoiceDetail>
                    {
                        ErrorMessage = ex.Message,
                    };
                }
            }
        }
    }
}
