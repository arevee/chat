using Chat.Logging.Interfaces;
using Chat.Protocol;
using Chat.Server.Interfaces;
using Chat.Server.Models;
using Chat.Server.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Chat.Server.Implementation
{
    /// <summary>
    /// Socket Server implementation
    /// </summary>
    public class SocketServer : IServer
    {
        private const string WelcomeMessage = "welcome to the chat room";
        private const string GoodbyeMessage = "left the chat room";

        private readonly List<User> usersList = new List<User>();
        private readonly List<string> messages = new List<string>();
        private readonly Socket socketServer;
        private readonly byte[] buffer;
        private readonly ILog log;

        /// <summary>
        ///  The buffer size
        /// </summary>
        public int BuffferSize { get; }

        /// <summary>
        ///  The number of incoming connections that can be queued for acceptance
        /// </summary>
        public int Backlog { get; }

        /// <summary>
        ///  IP address
        /// </summary>
        public string HostAddress { get; }

        /// <summary>
        ///  Port
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///  The number of messages to send to the new client
        /// </summary>
        public int CountOfMessagesToSend { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer"/> class.
        /// </summary>
        /// <param name="serverOptions">Server configurations options</param>
        /// <param name="log">Log</param>
        public SocketServer(ServerOptions serverOptions, ILog log = null)
        {
            this.log = log;
            BuffferSize = serverOptions.BufferSize;
            Backlog = serverOptions.Backlog;
            HostAddress = serverOptions.HostAddress ?? throw new ArgumentNullException(paramName: nameof(serverOptions.HostAddress));
            Port = serverOptions.Port;
            CountOfMessagesToSend = serverOptions.NumberOfMessagesToSend;
            buffer = new byte[BuffferSize];
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        ///<inheritdoc/>
        public void Start()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.TryParse(HostAddress, out IPAddress address) ? address : IPAddress.Any, Port);
            socketServer.Bind(ipEndPoint);
            socketServer.Listen(Backlog);
            socketServer.BeginAccept(new AsyncCallback(OnAccept), null);
            log?.LogInfo($"Server has been started. Address: {HostAddress}:{Port}");
        }

        ///<inheritdoc/>
        public void Stop()
        {
            usersList.Clear();
            messages.Clear();
            socketServer.Close();
        }

        private void OnAccept(IAsyncResult result)
        {
            try
            {
                var clientSocket = socketServer.EndAccept(result);
                socketServer.BeginAccept(new AsyncCallback(OnAccept), null);
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
            }
            catch (Exception ex)
            {
                log?.LogError(ex.Message);
            }
        }

        private void OnReceive(IAsyncResult result)
        {
            try
            {
                var clientSocket = (Socket)result.AsyncState;

                clientSocket.EndReceive(result);
                var newMessage = new Message(buffer);
                string text = "";

                switch (newMessage.Command)
                {
                    case Command.UserRegistration:

                        var newUser = new User(newMessage.Text, clientSocket);
                        usersList.Add(newUser);
                        text = $"{newUser.Name}, {WelcomeMessage}!";
                        break;

                    case Command.Disconnect:

                        text = $"{GetUserName(clientSocket)} {GoodbyeMessage}";
                        usersList.Remove(usersList.SingleOrDefault(c => c.Socket == clientSocket));
                        clientSocket.Close();
                        break;

                    case Command.TextMessage:

                        text = GetUserName(clientSocket) + ": " + newMessage.Text;
                        break;

                    case Command.GetHistory:
                        var lastMessages = "Last messages:\n" + string.Join("\n", messages.TakeLast(CountOfMessagesToSend).ToArray());
                        var data = new Message(Command.TextMessage, lastMessages).ToBytes();
                        clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSend), clientSocket);
                        log?.LogInfo($"Send last {lastMessages.Length} to {GetUserName(clientSocket)}");
                        break;

                    default:
                        text = "Unknown command";
                        break;
                }

                if (newMessage.Command != Command.GetHistory)
                {
                    var responseMessage = new Message(Command.TextMessage, $"{text}");

                    var message = responseMessage.ToBytes();
                    foreach (var user in usersList)
                    {
                        user.Socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                            new AsyncCallback(OnSend), user.Socket);
                    }
                    log?.LogInfo(text);
                    messages.Add(text);
                }

                if (newMessage.Command != Command.Disconnect)
                {
                    clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
                }
            }
            catch (Exception e)
            {
                log?.LogError(e.Message);
            }
        }

        private string GetUserName(Socket socket)
        {
            return usersList.FirstOrDefault(u => u.Socket == socket)?.Name;
        }

        private void OnSend(IAsyncResult result)
        {
            try
            {
                Socket client = (Socket)result.AsyncState;
                client.EndSend(result);
            }
            catch (Exception e)
            {
                log?.LogError(e.Message);
            }
        }
    }
}