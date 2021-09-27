using Chat.Client.Interfaces;
using Chat.Client.Options;
using Chat.Logging.Interfaces;
using Chat.Protocol;
using System;
using System.Net;
using System.Net.Sockets;

namespace Chat.Client.Implementation
{
    /// <summary>
    /// Socket Client implementation
    /// </summary>
    public class Client : IClient
    {
        private readonly Socket clientSocket;
        private readonly ILog log;

        private byte[] buffer;

        /// <summary>
        ///  IP address
        /// </summary>
        public string HostAddress { get; }

        /// <summary>
        ///  The port
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///  The buffer size
        /// </summary>
        public int BuffferSize { get; }

        /// <summary>
        ///  User name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="connectionOptions">Options to connect to the server</param>
        /// <param name="log">Log</param>
        public Client(ConnectionOptions connectionOptions, ILog log = null)
        {
            this.log = log;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            BuffferSize = connectionOptions.BufferSize;
            HostAddress = connectionOptions.HostAddress ?? throw new ArgumentNullException(paramName: nameof(connectionOptions.HostAddress));
            Port = connectionOptions.Port;
            buffer = new byte[BuffferSize];
        }

        ///<inheritdoc/>
        public void Connect(string userName)
        {
            try
            {
                UserName = userName ?? throw new NullReferenceException($"Username can not be null");
                var ipEndPoint = new IPEndPoint(IPAddress.TryParse(HostAddress, out IPAddress address) ? address : IPAddress.Any, Port);
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }
            catch (Exception e)
            {
                log?.LogError(e.Message);
            }
        }

        ///<inheritdoc/>
        public void SendMessage(Command command, string text)
        {
            try
            {
                var message = new Message(command, text ?? throw new NullReferenceException($"Sending text can not be null"));
                var data = message.ToBytes();
                clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception e)
            {
                log?.LogError(e.Message);
            }
        }

        ///<inheritdoc/>s
        public void Disonnect()
        {
            clientSocket.Close();
        }

        ///<inheritdoc/>
        public bool IsConnected()
        {
            return clientSocket.Connected;
        }

        private void OnConnect(IAsyncResult result)
        {
            try
            {
                clientSocket.EndConnect(result);
                SendMessage(Command.UserRegistration, UserName);
                SendMessage(Command.GetHistory, string.Empty);
            }
            catch (Exception e)
            {
                log?.LogError(e.Message);
            }
        }

        private void OnSend(IAsyncResult result)
        {
            try
            {
                clientSocket.EndSend(result);
            }
            catch (Exception e)
            {
                log?.LogError(e.Message);
            }
        }

        private void OnReceive(IAsyncResult result)
        {
            try
            {
                clientSocket.EndReceive(result);

                var message = new Message(buffer);

                log?.LogInfo(message.Text);

                buffer = new byte[BuffferSize];

                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }
            catch (Exception e)
            {
                log?.LogInfo(e.Message);
            }
        }
    }
}