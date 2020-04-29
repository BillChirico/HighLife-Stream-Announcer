using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Service.Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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