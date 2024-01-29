using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MedicamentStore 
{

    public class MedicamentRepository : IMedicamentRepository
    {
        private readonly SqliteDbConnection _connection;

        public MedicamentRepository(SqliteDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<MedicinDCI>> GetDCI(string Nom)
        {
            if (string.IsNullOrWhiteSpace(Nom))
            {
                return Enumerable.Empty<MedicinDCI>();
            }
            string sql = "SELECT DISTINCT DCI   FROM Medicament WHERE DCI LIKE @Word ; ";

            var res = await _connection.QueryAsync<MedicinDCI>(sql, new { Word = $"{Nom}%" });

            if (res.Count() > 0)
            {
                return res;
            }
            else
            {
                return Enumerable.Empty<MedicinDCI>();

            }
        }

        public async Task<IEnumerable<MedicinCommercial>> GetNomSearch(string Nom)
        {
            if(string.IsNullOrWhiteSpace(Nom))
            {
                return Enumerable.Empty<MedicinCommercial>();
            }
            string sql = "SELECT Id , Nom_Commercial   FROM Medicament WHERE Nom_Commercial LIKE @Word ; ";

            var res = await _connection.QueryAsync<MedicinCommercial>(sql, new { Word = $"{Nom}%" });

            if(res.Count() > 0)
            {
                return res;
            }
            else
            {
                return Enumerable.Empty<MedicinCommercial>();

            }

        }

        public async Task <IEnumerable<MedicamentFact>> GetSearch(string Nom)
        {
            string sql = "SELECT Id , Nom_Commercial ,Forme,Dosage,Img,Conditionnement,Tarif_de_Référence,PPA_indicatif  FROM Medicament WHERE Nom_Commercial LIKE @Word ; ";

           return await _connection.QueryAsync<MedicamentFact>(sql, new { Word =$"%{Nom}%" });

        }

        public async Task<IEnumerable<MedicamentStock>> GetSearchForStock(string Nom)
        {
            string sql = "SELECT Id , Nom_Commercial ,Forme,Dosage,Img,Conditionnement,Tarif_de_Référence,PPA_indicatif  FROM Medicament WHERE Nom_Commercial LIKE @Word ; ";

            return await _connection.QueryAsync<MedicamentStock>(sql, new { Word = $"%{Nom}%" });

        }

       
    }
}
