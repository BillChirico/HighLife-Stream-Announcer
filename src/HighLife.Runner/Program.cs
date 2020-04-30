using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Repository;
using HighLife.StreamAnnouncer.Service.Discord;
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
                    services.Configure<Settings>(GetSettingsFile("appsettings.json", "Settings"));

                    // Discord
                    services.AddSingleton<IDiscordBot, DiscordBot>();
                    services.AddSingleton<DiscordSocketClient>();

                    // Twitch Api
                    services.AddSingleton(provider =>
                        TwitchApiFactory.Create(
                            provider.GetRequiredService<IOptions<Settings>>().Value.TwitchClientId,
                            provider.GetRequiredService<IOptions<Settings>>().Value.TwitchAccessToken));

                    // Database
                    services.AddSingleton<IDataStore>(new DataStore("Database.json", keyProperty: "id",
                        reloadBeforeGetCollection: true));

                    // Repositories
                    services.AddSingleton(typeof(IDataStoreRepository<>), typeof(DataStoreRepository<>));
                });
        }

        private static IConfigurationSection GetSettingsFile(string file, string section)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();

            builder
                .AddJsonFile(file, false, true);

            IConfigurationRoot configuration = builder.Build();

            return configuration.GetSection(section);
        }
    }
}