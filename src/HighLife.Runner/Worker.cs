using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Entitites;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Repository;
using HighLife.StreamAnnouncer.Service.Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HighLife.Runner
{
    public class Worker : BackgroundService
    {
        private readonly IDiscordBot _discordBot;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger<Worker> _logger;
        private readonly Settings _settings;
        private readonly IDataStoreRepository<Streamer> _streamerRepository;

        public Worker(ILogger<Worker> logger, IOptions<Settings> settings, IDiscordBot discordBot,
            DiscordSocketClient discordSocketClient, IDataStoreRepository<Streamer> streamerRepository)
        {
            _logger = logger;
            _discordBot = discordBot;
            _discordSocketClient = discordSocketClient;
            _streamerRepository = streamerRepository;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Reset event
            ManualResetEvent mre = new ManualResetEvent(false);

            await _discordBot.Connect(_settings.DiscordBotToken);

            _discordSocketClient.Ready += () =>
            {
                _logger.LogInformation("Discord client is ready");

                return Task.CompletedTask;
            };

            _discordSocketClient.GuildAvailable += guild =>
            {
                _logger.LogInformation("Discord guild is ready");

                mre.Set();

                return Task.CompletedTask;
            };

            // Wait for all connectable services to be ready
            mre.WaitOne();

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}