using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MedicamentStore
{
    public class SqliteDbConnection
    {
        private readonly string? _connectionString;

        public SqliteDbConnection(string? connectionString)
        {
            _connectionString = connectionString;
        }
         
        public IDbConnection Connection() 
        {
           var conn = new SQLiteConnection(_connectionString);
           conn.Open();
            return conn;
        } 

        public async Task<int> ExecuteAsync(string query, object? parameters = null)
        {
            using (var connection =  Connection())
            {

              return  await connection.ExecuteAsync(query, parameters);
              
            }
        }

        public async Task<int> ExecuteAsync(IDbConnection conn, string query, IDbTransaction transaction, object? parameters = null )
        {
            
                 
             
                return await conn.ExecuteAsync(query, parameters , transaction);

            
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
        {
            using (var connection =  Connection())
            {
                return await connection.QueryAsync<T>(query, parameters);
            }
        }

        public  IEnumerable<T> Query<T>(string query, object? parameters = null)
        {
            using (var connection = Connection())
            {
                return  connection.Query<T>(query, parameters);
            }
        }

        public async Task<T> ExecuteScalar<T>(string query, object? parameters = null) 
            {
            using (var connection = Connection()) 
            {
                return await connection.ExecuteScalarAsync<T>(query, parameters);
            }

        }
        public async Task<T> ExecuteScalarTransaction<T>(IDbConnection conn, string query, IDbTransaction transaction, object? parameters = null)
        {
            
                 return await conn.ExecuteScalarAsync<T>(query, parameters, transaction);
            

        }

        public async Task<int> InsertDataAsync(string query, object? parameters = null)
        {
            using (var connection = Connection()) // Open a new connection
            { 
                 

                using (var transaction = connection.BeginTransaction()) // Start a transaction
                {
                    try
                    {
                        // Perform insertion
                        await connection.ExecuteAsync(query,parameters);

                        // Commit the transaction
                        transaction.Commit();

                        // Retrieve the last inserted row ID
                        int lastInsertedId = await connection.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");

                        return lastInsertedId;
                    }
                    catch 
                    {
                        // Handle exceptions if the insertion fails
                        transaction.Rollback(); // Rollback the transaction
                        throw;
                    }
                }
            }
        }

    }
}
