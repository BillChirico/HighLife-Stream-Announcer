using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Entitites;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Repository;
using HighLife.StreamAnnouncer.Service.Twitch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;

namespace HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer, IModule
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly ILogger<StreamAnnouncer> _logger;
        private readonly Settings _settings;
        private readonly IDataStoreRepository<Streamer> _streamerRepository;
        private readonly ITwitchApiHelper _twitchApiHelper;

        public StreamAnnouncer(DiscordSocketClient discordClient, ITwitchApiHelper twitchApiHelper,
            ILogger<StreamAnnouncer> logger, IDataStoreRepository<Streamer> streamerRepository,
            IOptions<Settings> settings)
        {
            _discordClient = discordClient;
            _twitchApiHelper = twitchApiHelper;
            _logger = logger;
            _streamerRepository = streamerRepository;
            _settings = settings.Value;
        }

        public async Task Init()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    IEnumerable<Streamer> collection = _streamerRepository.GetCollection().AsQueryable();

                    List<Streamer> streamers = collection.ToList();

                    foreach (Streamer streamer in streamers)
                    {
                        await Announce(streamer);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            });
        }

        public async Task Announce(Streamer streamer)
        {
            try
            {
                SocketGuild guild = _discordClient.GetGuild(_settings.DiscordGuildId);

                if (guild == null)
                {
                    _logger.LogError("Could not find guild to announce stream!");

                    return;
                }

                SocketTextChannel channel = guild.GetTextChannel(_settings.DiscordChannelId);

                if (channel == null)
                {
                    _logger.LogError($"Could not find channel (ID = {_settings.DiscordChannelId}) to announce stream!");

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