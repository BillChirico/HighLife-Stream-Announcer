using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Entitites;
using HighLife.StreamAnnouncer.Service.Twitch;
using Microsoft.Extensions.Logging;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;

namespace HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly ILogger<StreamAnnouncer> _logger;
        private readonly ITwitchApiHelper _twitchApiHelper;

        public StreamAnnouncer(DiscordSocketClient discordClient, ITwitchApiHelper twitchApiHelper,
            ILogger<StreamAnnouncer> logger)
        {
            _discordClient = discordClient;
            _twitchApiHelper = twitchApiHelper;
            _logger = logger;
        }

        public async Task Announce(Streamer streamer, ulong guildId, ulong channelId)
        {
            try
            {
                SocketGuild guild = _discordClient.GetGuild(guildId);

                if (guild == null)
                {
                    _logger.LogError("Could not find guild to announce stream!");

                    return;
                }

                SocketTextChannel channel = guild.GetTextChannel(channelId);

                if (channel == null)
                {
                    _logger.LogError($"Could not find channel (ID = {channelId}) to announce stream!");

                    return;
                }

                User user = await _twitchApiHelper.GetUser(streamer.Username);

                if (user == null)
                {
                    return;
                }

                Stream stream = await _twitchApiHelper.GetStream(user);

                if (stream == null)
                {
                    return;
                }

                Game game = await _twitchApiHelper.GetStreamGame(stream);

                if (game == null)
                {
                    return;
                }

                if (game.Name != "Grand Theft Auto V")
                {
                    return;
                }

                await channel.SendMessageAsync($"{streamer.Username} is now live!");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error occured while announcing streams: {exception}");
            }
        }
    }
}