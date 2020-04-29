using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace HighLife.StreamAnnouncer.Service.Discord
{
    public class DiscordBot : IDiscordBot
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger _logger;

        public DiscordBot(DiscordSocketClient client, ILogger<DiscordBot> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task Connect(string token)
        {
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();
        }

        private Task Log(LogMessage logMessage)
        {
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(logMessage.ToString());
                    break;
                case LogSeverity.Error:
                    _logger.LogError(logMessage.ToString());
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(logMessage.ToString());
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(logMessage.ToString());
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.LogDebug(logMessage.ToString());
                    break;
                default:
                    _logger.LogCritical(logMessage.ToString());
                    break;
            }

            return Task.CompletedTask;
        }
    }
}