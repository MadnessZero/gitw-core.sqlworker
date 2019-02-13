namespace System
{
    /// <summary>
    /// Exception spécifique au fonctionnement du SqlWorker
    /// </summary>
    public class SqlWorkerException : Exception
    {
        /// 
        public SqlWorkerException() { }
        /// 
        public SqlWorkerException(string message) : base (message) { }
        /// 
        public SqlWorkerException(string message, Exception innerException) : base (message, innerException) { }
    }
}
