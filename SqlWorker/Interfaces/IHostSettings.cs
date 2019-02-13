namespace GitWCore.SqlWorker
{
    /// <summary>
    /// Interface décrivant les besoins besoins pour la connexion au serveur de base de données
    /// </summary>
    public interface IHostSettings
    {
        /// <summary>
        /// Retourne l'adresse du serveur de base de données
        /// </summary>
        string Server { get; }
        /// <summary>
        /// Retourne le nom de la base de données
        /// </summary>
        string Database { get; }
        /// <summary>
        /// Retourne l'utilisateur que l'on va utiliser pour se connecter à la base de données
        /// </summary>
        string User { get; }
        /// <summary>
        /// Retourne le mot de passe de l'utilisateur
        /// </summary>
        string Password { get; }
    }
}
