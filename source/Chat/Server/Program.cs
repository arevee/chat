using Chat.Logging.Implementation;
using Chat.Logging.Interfaces;
using Chat.Server.Implementation;
using Chat.Server.Interfaces;
using Chat.Server.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Chat.Server
{
    internal class Program
    {
        private static IServer server;
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

            server = ServiceProvider.GetRequiredService<IServer>();

            server.Start();

            Console.WriteLine("Enter anything to stop the server");
            Console.ReadLine();
        }

        private static void OnExit(object sender, EventArgs e)
        {
            server.Stop();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var settings = Configuration.GetSection(nameof(ServerOptions)).Get<ServerOptions>() ?? new ServerOptions();
            services
                .AddSingleton<ILog, ConsoleLog>()
                .AddSingleton(settings)
                .AddSingleton<IServer, SocketServer>();
        }
    }
}