namespace Chat.Server.Options
{
    /// <summary>
    ///  Server configuration options
    /// </summary>
    public class ServerOptions
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
        ///  The number of messages to send to the new client
        /// </summary>
        public int NumberOfMessagesToSend { get; set; } = 10;

        /// <summary>
        ///  The buffer size
        /// </summary>
        public int BufferSize { get; set; } = 512;

        /// <summary>
        ///  The number of incoming connections that can be queued for acceptance
        /// </summary>
        public int Backlog { get; set; } = 5;
    }
}