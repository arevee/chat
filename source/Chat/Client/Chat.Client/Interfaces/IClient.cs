using Chat.Protocol;

namespace Chat.Client.Interfaces
{
    /// <summary>
    /// Client Interface
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Connects to the server
        /// </summary>
        /// <param name="userName">Name of the client</param>
        void Connect(string userName);

        /// <summary>
        /// Sends message to the server
        /// </summary>
        /// <param name="command">Type of message</param>
        /// <param name="text">Text</param>
        void SendMessage(Command command, string text);

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        void Disonnect();

        /// <summary>
        /// Indicates is connection established
        /// </summary>
        bool IsConnected();
    }
}