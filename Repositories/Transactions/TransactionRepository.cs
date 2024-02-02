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
        public async Task<IEnumerable<TransactionDto>> GetAll()
        {
            var baseQuery = @"SELECT t.Id, m.Nom_Commercial, m.Dosage, m.Forme, m.Conditionnement, s.Quantite, m.Img, s.Prix, p.Nom, s.Date, s.Id AS IdStock , t.TypeTransaction ,t.QuantiteTransaction
                                FROM  [Transaction] t
                                INNER JOIN Stock s ON s.Id = t.IdStock
                                INNER JOIN PharmaceuticalProducts m ON s.IdMedicament = m.Id
                                INNER JOIN Supplies p ON p.Id = s.IdSupplie";

            var resultFinal = await _connection.QueryAsync<TransactionDto>(baseQuery);

            return resultFinal.Any() ? resultFinal : Enumerable.Empty<TransactionDto>();
        }
    }
}
