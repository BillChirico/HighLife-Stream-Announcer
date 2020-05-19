using System;
using Discord.Commands;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Repository;
using HighLife.StreamAnnouncer.Service.Discord;
using HighLife.StreamAnnouncer.Service.Discord.Commands;
using HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer;
using HighLife.StreamAnnouncer.Service.Twitch;
using JsonFlatFileDataStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HighLife.Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    // Settings
                    services.Configure<ConfigSettings>(GetSettingsFile("appsettings.json", "Settings"));

                    // Discord
                    services.AddSingleton<IDiscordBot, DiscordBot>();
                    services.AddSingleton<DiscordSocketClient>();
                    services.AddSingleton<CommandService>();
                    services.AddSingleton<CommandHandler>();

                    // Twitch Api
                    services.AddSingleton(provider =>
                        TwitchApiFactory.Create(
                            provider.GetRequiredService<IOptions<ConfigSettings>>().Value.TwitchClientId,
                            provider.GetRequiredService<IOptions<ConfigSettings>>().Value.TwitchClientSecret));

                    services.AddSingleton<ITwitchApiHelper, TwitchApiHelper>();

                    // Database
                    services.AddSingleton<IDataStore>(new DataStore("Database.json", keyProperty: "id",
                        reloadBeforeGetCollection: true));

                    // Repositories
                    services.AddSingleton(typeof(IDataStoreRepository<>), typeof(DataStoreRepository<>));

                    // Modules
                    services
                        .AddSingleton<IStreamAnnouncer, StreamAnnouncer.Service.Modules.StreamAnnouncer.StreamAnnouncer
                        >();
                });
        }

        private static IConfigurationSection GetSettingsFile(string file, string section)
        {
            var builder = new ConfigurationBuilder();

            builder
                .AddJsonFile(
                    Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Production"
                        ? "appsettings.json"
                        : "appsettings.Development.json", false,
                    true);

            var configuration = builder.Build();

            return configuration.GetSection(section);
        }
    }
}