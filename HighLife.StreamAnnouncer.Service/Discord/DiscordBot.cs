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

        public DiscordBot(DiscordSocketClient client, ILogger logger)
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
                    _logger.Fatal(logMessage.ToString());
                    break;
                case LogSeverity.Error:
                    _logger.Error(logMessage.ToString());
                    break;
                case LogSeverity.Warning:
                    _logger.Warning(logMessage.ToString());
                    break;
                case LogSeverity.Info:
                    _logger.Information(logMessage.ToString());
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.Debug(logMessage.ToString());
                    break;
                default:
                    _logger.Fatal(logMessage.ToString());
                    break;
            }

            return Task.CompletedTask;
        }
    }
}