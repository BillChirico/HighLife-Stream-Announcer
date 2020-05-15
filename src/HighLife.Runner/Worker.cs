using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Service.Discord;
using HighLife.StreamAnnouncer.Service.Discord.Commands;
using HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HighLife.Runner
{
    public class Worker : BackgroundService
    {
        private readonly CommandHandler _commandHandler;
        private readonly IDiscordBot _discordBot;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger<Worker> _logger;
        private readonly Settings _settings;
        private readonly IStreamAnnouncer _streamAnnouncer;

        public Worker(ILogger<Worker> logger, IOptions<Settings> settings, IDiscordBot discordBot,
            DiscordSocketClient discordSocketClient,
            IStreamAnnouncer streamAnnouncer, CommandHandler commandHandler)
        {
            _logger = logger;
            _discordBot = discordBot;
            _discordSocketClient = discordSocketClient;
            _streamAnnouncer = streamAnnouncer;
            _commandHandler = commandHandler;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Reset event
            var mre = new ManualResetEvent(false);

            await _discordBot.Connect(_settings.DiscordBotToken);

            _discordSocketClient.Ready += async () =>
            {
                _logger.LogInformation("Discord client is ready");

                await _commandHandler.InstallCommandsAsync();

                // Initialize modules
                await _streamAnnouncer.Init();
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