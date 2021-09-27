using Chat.Client.Interfaces;
using Chat.Client.Options;
using Chat.Logging.Implementation;
using Chat.Logging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Chat.Client
{
    internal class Program
    {
        private static IClient client;

        public static IServiceProvider ServiceProvider { get; private set; }

        public static IConfiguration Configuration { get; private set; }

        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            AppDomain.CurrentDomain.ProcessExit += OnExit;

            client = ServiceProvider.GetRequiredService<IClient>();

            Console.WriteLine("Enter your user name:");

            var userName = EnterUserName();

            client.Connect(userName);

            StartChatting(client);

            Console.WriteLine("Session is ended");
        }

        private static void OnExit(object sender, EventArgs e)
        {
            client.SendMessage(Protocol.Command.Disconnect, "");
            client.Disonnect();
        }

        private static string EnterUserName()
        {
            string userName;
            const int maxUserNameLength = 8;

            while (true)
            {
                userName = Console.ReadLine();
                var isNotNullOrEmpty = !string.IsNullOrEmpty(userName);
                var isNotTooLong = userName?.Length < maxUserNameLength;
                if (!isNotNullOrEmpty)
                {
                    Console.WriteLine("User name can not be null or empty");
                }

                if (!isNotTooLong)
                {
                    Console.WriteLine($"User name max lentgth is {maxUserNameLength} symbols");
                }

                if (isNotTooLong && isNotNullOrEmpty)
                {
                    return userName;
                }
            }
        }

        private static void StartChatting(IClient client)
        {
            const int maxMessageLentgth = 40;

            while (client.IsConnected())
            {
                var message = Console.ReadLine();
                var isNotNullOrEmpty = !string.IsNullOrEmpty(message);
                var isNotTooLong = message?.Length < maxMessageLentgth;
                if (!isNotNullOrEmpty)
                {
                    Console.WriteLine("Message can not be null or empty");
                }

                if (!isNotTooLong)
                {
                    Console.WriteLine($"User name max lentgth is {maxMessageLentgth} symbols");
                }

                if (isNotTooLong && isNotNullOrEmpty)
                {
                    client.SendMessage(Protocol.Command.TextMessage, message);
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var settings = Configuration.GetSection(nameof(ConnectionOptions)).Get<ConnectionOptions>() ?? new ConnectionOptions();
            services
                .AddSingleton<ILog, ConsoleLog>()
                .AddSingleton(settings)
                .AddSingleton<IClient, Implementation.Client>();
        }
    }
}