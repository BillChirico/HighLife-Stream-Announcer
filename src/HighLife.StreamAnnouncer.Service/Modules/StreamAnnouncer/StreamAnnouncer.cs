using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Entities;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Repository;
using HighLife.StreamAnnouncer.Service.Discord;
using HighLife.StreamAnnouncer.Service.Twitch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Helix.Models.Streams;

namespace HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer, IModule
    {
        private readonly IDataStoreRepository<AnnouncementMessages> _announcementMessageRepository;
        private readonly ConfigSettings _configSettings;
        private readonly DiscordSocketClient _discordClient;
        private readonly IDataStoreRepository<DiscordSettings> _discordSettingsRepository;
        private readonly IDataStoreRepository<PinnedMessage> _pinnedMessageRepository;
        private readonly ILogger<StreamAnnouncer> _logger;
        private readonly IDataStoreRepository<Streamer> _streamerRepository;
        private readonly ITwitchApiHelper _twitchApiHelper;

        public StreamAnnouncer(DiscordSocketClient discordClient, ITwitchApiHelper twitchApiHelper,
            ILogger<StreamAnnouncer> logger, IDataStoreRepository<Streamer> streamerRepository,
            IOptions<ConfigSettings> settings, IDataStoreRepository<AnnouncementMessages> announcementMessageRepository,
            IDataStoreRepository<DiscordSettings> discordSettingsRepository,
            IDataStoreRepository<PinnedMessage> pinnedMessageRepository)
        {
            _discordClient = discordClient;
            _twitchApiHelper = twitchApiHelper;
            _logger = logger;
            _streamerRepository = streamerRepository;
            _announcementMessageRepository = announcementMessageRepository;
            _discordSettingsRepository = discordSettingsRepository;
            _pinnedMessageRepository = pinnedMessageRepository;
            _configSettings = settings.Value;
        }

        public void Init()
        {
            _logger.LogInformation("Initializing Stream Announcer");

            Task.Run(async () =>
            {
                while (true)
                {
                    _logger.LogDebug("Starting stream announcements");

                    var streamers = _streamerRepository.GetAll();

                    var liveStreamers = await Announce(streamers);

                    var liveStreamerIds = liveStreamers.Select(ls => ls.Id);

                    var offlineStreamers = streamers.Where(s => !liveStreamerIds.Contains(s.Id));

                    await UpdatePinnedMessage(liveStreamers, offlineStreamers);

                    _logger.LogDebug("Finished stream announcements");

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            });
        }

        public async Task<List<Streamer>> Announce(IEnumerable<Streamer> streamers)
        {
            var liveStreamers = new List<Streamer>();

            foreach (var streamer in streamers)
            {
                try
                {
                    var channel = GetChannel(_discordSettingsRepository.GetAll().FirstOrDefault()?.DiscordChannelId);

                    var user = await _twitchApiHelper.GetUser(streamer.Username);

                    if (user == null)
                    {
                        continue;
                    }

                    var stream = await _twitchApiHelper.GetStream(user);

                    var announcementMessage = _announcementMessageRepository.GetAll()
                        .FirstOrDefault(am => am.Streamer.Id == streamer.Id);

                    if (announcementMessage != null)
                    {
                        if (stream == null || CheckStreamTitle(stream) || stream.GameId != TwitchConstants.GtaGameId)
                        {
                            await RemoveLiveMessage(channel, announcementMessage);
                        }

                        continue;
                    }

                    if (stream == null)
                    {
                        continue;
                    }

                    if (CheckStreamTitle(stream))
                    {
                        continue;
                    }

                    var message = await channel.SendMessageAsync(string.Empty,
                        embed: EmbedHelper.LiveMessageEmbedBuilder(streamer, user, stream));

                    await _announcementMessageRepository.Add(new AnnouncementMessages
                    {
                        MessageId = message.Id,
                        Streamer = streamer
                    });

                    _logger.LogInformation($"Successfully announced streamer [{streamer.Username}]");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception,
                        $"Error occured while announcing streamer [{streamer.Username}]: {exception}");
                }
            }

            return liveStreamers;
        }

        private SocketTextChannel GetChannel(ulong? channelId)
        {
            var guild = _discordClient.GetGuild(_configSettings.DiscordGuildId);

            if (guild == null)
            {
                _logger.LogError("Could not find guild to announce streams!");

                return null;
            }

            if (channelId == null)
            {
                _logger.LogError("There are no set channels to announce streams!");

                return null;
            }

            var channel = guild.GetTextChannel(channelId.Value);

            if (channel != null)
            {
                return channel;
            }

            _logger.LogError(
                $"Could not find channel (ID = {channelId}) to announce streams!");

            return null;
        }

        public async Task UpdatePinnedMessage(IEnumerable<Streamer> liveStreamers,
            IEnumerable<Streamer> offlineStreamers)
        {
            var pinnedMessageId = _pinnedMessageRepository.GetAll().FirstOrDefault()?.MessageId;

            if (pinnedMessageId == null)
            {
                _logger.LogError("There is no pinned message set!");

                return;
            }

            var guild = _discordClient.GetGuild(_configSettings.DiscordGuildId);

            if (guild == null)
            {
                _logger.LogError("Could not find guild to update pinned message!");

                return;
            }

            var channel = guild.GetTextChannel(channelId.Value);

            if (channel == null)
            {
                _logger.LogError(
                    $"Could not find channel (ID = {channelId}) to announce stream!");

                continue;
            }
        }

        private static bool CheckStreamTitle(Stream stream)
        {
            return !stream.Title.Contains("HighLifeRP", StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task RemoveLiveMessage(SocketTextChannel channel, AnnouncementMessages announcementMessage)
        {
            try
            {
                await _announcementMessageRepository.Delete(announcementMessage);

                await (await channel.GetMessageAsync(announcementMessage.MessageId)).DeleteAsync();

                _logger.LogInformation(
                    $"Successfully removed announcement message for [{announcementMessage.Streamer.Username}]");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    $"Could not remove announcement message for [{announcementMessage.Streamer.Username}]: {exception}");
            }
        }
    }
}