using System;
using System.Net.Sockets;

namespace Chat.Server.Models
{
    /// <summary>
    /// User model
    /// </summary>
    public class User
    {
        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Client socket
        /// </summary>
        public Socket Socket { get; }

        /// <summary>
        /// User ID
        /// </summary>
        public Guid Id { get; }

        public User(string name, Socket socket)
        {
            Id = Guid.NewGuid();
            Name = name + "#" + Id.ToString().Substring(0, 4) ?? throw new ArgumentNullException(paramName: nameof(name));
            Socket = socket ?? throw new ArgumentNullException(paramName: nameof(socket));
        }
    }
}