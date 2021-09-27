namespace Chat.Server.Interfaces
{
    /// <summary>
    /// Server Interface
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Starts Server
        /// </summary>
        void Start();

        /// <summary>
        /// Stops Server
        /// </summary>
        void Stop();
    }
}