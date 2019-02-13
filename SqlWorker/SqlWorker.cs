using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GitWCore.SqlWorker
{
    /// <summary>
    /// Outil de communication avec la base de données
    /// </summary>
    public class SqlWorker
    {
        private readonly Func<SqlConnection> InitConnection;

        /// <summary>
        ///  Instancie un nouvel objet SqlWorker
        /// </summary>
        /// <param name="settings">Configuration de connexion au serveur de base de données</param>
        public SqlWorker(IHostSettings settings)
        {
            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = settings.Server,
                UserID = settings.User,
                Password = settings.Password,
                IntegratedSecurity = false,
                MultipleActiveResultSets = true
            };

            if (!string.IsNullOrEmpty(settings.Database))
                builder.InitialCatalog = settings.Database;

            InitConnection = () => { return new SqlConnection(builder.ConnectionString); };
        }

        /// <summary>
        /// Prépare et exécute une commande ExecuteReader en base de données
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public TResult ExecuteReader<TResult>(string query, Func<SqlReader, TResult> callback)
        {
            return ExecuteReader(query, callback, new Dictionary<string, object>());
        }

        /// <summary>
        /// Prépare et exécute une commande ExecuteReader en base de données
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="SqlWorkerException"></exception>
        public TResult ExecuteReader<TResult>(string query, Func<SqlReader, TResult> callback, Dictionary<string, object> parameters)
        {
            return TryExecute(query, parameters, (cmd) =>
            {
                var reader = new SqlReader(cmd.ExecuteReader());
                return (reader != null && reader.Read() ? callback(reader) : default(TResult));
            });
        }

        /// <summary>
        /// Prépare et exécute une commande ExecuteReader qui retourne une liste queryable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IQueryable<TResult> ExecuteListReader<TResult>(string query, Func<SqlReader, TResult> callback)
        {
            return ExecuteListReader(query, callback, new Dictionary<string, object>());
        }

        /// <summary>
        /// Prépare et exécute une commande ExecuteReader qui retourne une liste queryable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="SqlWorkerException"></exception>
        public IQueryable<TResult> ExecuteListReader<TResult>(string query, Func<SqlReader, TResult> callback, Dictionary<string, object> parameters)
        {
            return TryExecute(query, parameters, (cmd) =>
            {
                var reader = new SqlReader(cmd.ExecuteReader());
                var result = new List<TResult>();

                while (reader.Read())
                {
                    result.Add(callback(reader));
                }

                return result.AsQueryable();
            });
        }

        /// <summary>
        /// Prépare et exécute une requête indéfinie dans la base de données
        /// </summary>
        /// <param name="query"></param>
        public void ExecuteNonQuery(string query)
        {
            ExecuteNonQuery(query, new Dictionary<string, object>());
        }

        /// <summary>
        /// Prépare et exécute une requête indéfinie dans la base de données
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <exception cref="SqlWorkerException"></exception>
        public void ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            TryExecute(query, parameters, (cmd) => { return cmd.ExecuteNonQuery(); });
        }

        /// <summary>
        /// Prépare et exécute une requête indéfinie dans la base de données en récupérant le premier résultat 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public TResult ExecuteScalar<TResult>(string query)
        {
            return ExecuteScalar<TResult>(query, new Dictionary<string, object>());
        }

        /// <summary>
        /// Prépare et exécute une requête indéfinie dans la base de données en récupérant le premier résultat 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="SqlWorkerException"></exception>
        public TResult ExecuteScalar<TResult>(string query, Dictionary<string, object> parameters)
        {
            return TryExecute(query, parameters, (cmd) => { return (TResult)Convert.ChangeType(cmd.ExecuteScalar(), typeof(TResult)); });
        }

        #region Fonctions de préparation et d'exécution du worker
        /// <summary>
        /// Sécurise l'exécution des requêtes
        /// </summary>
        private TResult TryExecute<TResult>(string query, Dictionary<string, object> parameters, Func<SqlCommand, TResult> action)
        {
            using (var sqlConnection = InitConnection())
            {
                using (var cmd = new SqlCommand().Prepare(sqlConnection, query, parameters))
                {
                    try
                    {
                        sqlConnection.Open();
                        return action(cmd);
                    }
                    catch (Exception ex)
                    {
                        throw new SqlWorkerException($"[EXECUTION] { ex.Message }", ex);
                    }
                }
            }
        }
        #endregion
    }
}
