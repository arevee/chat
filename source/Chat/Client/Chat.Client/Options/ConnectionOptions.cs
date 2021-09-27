namespace Chat.Client.Options
{
    /// <summary>
    ///  Client connection configuration options
    /// </summary>
    public class ConnectionOptions
    {
        /// <summary>
        ///  IP address
        /// </summary>
        public string HostAddress { get; set; } = "127.0.0.1";

        /// <summary>
        ///  The port
        /// </summary>
        public int Port { get; set; } = 5555;

        /// <summary>
        ///  The size of incoming message
        /// </summary>
        public int BufferSize { get; set; } = 512;
    }
}