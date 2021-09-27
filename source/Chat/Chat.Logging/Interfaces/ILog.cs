namespace Chat.Logging.Interfaces
{
    /// <summary>
    /// Log Interface
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Log Info message
        /// </summary>
        /// <param name="message">Message to log</param>
        void LogInfo(string message);

        /// <summary>
        /// Log Info message
        /// </summary>
        /// <param name="message">Message to log</param>
        void LogError(string message);
    }
}