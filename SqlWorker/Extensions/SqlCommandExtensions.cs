using System.Collections.Generic;
using System.Linq;

namespace System.Data.SqlClient
{
    /// <summary>
    /// 
    /// </summary>
    public static class SqlCommandExtensions
    {
        /// <summary>
        /// Définit les paramètres d'une commande.
        /// </summary>
        public static SqlCommand Prepare(this SqlCommand cmd, SqlConnection connection, string query, Dictionary<string, object> parameters)
        {
            // Conversion du dictionnaire 
            var sqlParameters = new SqlParameter[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                var p = parameters.Skip(i).FirstOrDefault();
                sqlParameters[i] = new SqlParameter($"@{ p.Key }", p.Value ?? DBNull.Value);
            }

            cmd.Connection = connection;
            cmd.CommandText = query;
            cmd.Parameters.AddRange(sqlParameters);

            return cmd;
        }
    }
}
