using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Entitites;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Repository;
using HighLife.StreamAnnouncer.Service.Twitch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer, IModule
    {
        private readonly IDataStoreRepository<AnnouncementMessages> _announcementMessageRepository;
        private readonly DiscordSocketClient _discordClient;
        private readonly ILogger<StreamAnnouncer> _logger;
        private readonly Settings _settings;
        private readonly IDataStoreRepository<Streamer> _streamerRepository;
        private readonly ITwitchApiHelper _twitchApiHelper;

        public StreamAnnouncer(DiscordSocketClient discordClient, ITwitchApiHelper twitchApiHelper,
            ILogger<StreamAnnouncer> logger, IDataStoreRepository<Streamer> streamerRepository,
            IOptions<Settings> settings, IDataStoreRepository<AnnouncementMessages> announcementMessageRepository)
        {
            _discordClient = discordClient;
            _twitchApiHelper = twitchApiHelper;
            _logger = logger;
            _streamerRepository = streamerRepository;
            _announcementMessageRepository = announcementMessageRepository;
            _settings = settings.Value;
        }

        public async Task Init()
        {
            _logger.LogInformation("Initializing Stream Announcer");

            Task.Run(async () =>
            {
                while (true)
                {
                    var collection = _streamerRepository.GetCollection().AsQueryable();

                    var streamers = collection.ToList();

                    foreach (var streamer in streamers)
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
                var guild = _discordClient.GetGuild(_settings.DiscordGuildId);

                if (guild == null)
                {
                    _logger.LogError("Could not find guild to announce stream!");

                    return;
                }

                var channel = guild.GetTextChannel(_settings.DiscordChannelId);

                if (channel == null)
                {
                    _logger.LogError($"Could not find channel (ID = {_settings.DiscordChannelId}) to announce stream!");

                    return;
                }

                var user = await _twitchApiHelper.GetUser(streamer.Username);

                if (user == null)
                {
                    return;
                }

                var stream = await _twitchApiHelper.GetStream(user);

                var announcementMessage = _announcementMessageRepository.GetCollection().AsQueryable()
                    .FirstOrDefault(am => am.Streamer.Id == streamer.Id);

                if (announcementMessage != null)
                {
                    if (stream == null)
                    {
                        await (await channel.GetMessageAsync(announcementMessage.MessageId)).DeleteAsync();
                    }

                    return;
                }

                if (stream == null)
                {
                    return;
                }

                var game = await _twitchApiHelper.GetStreamGame(stream);

                if (game == null)
                {
                    return;
                }

                if (game.Name != "Grand Theft Auto V")
                {
                    return;
                }

                var embed = new EmbedBuilder()
                    .WithTitle($"{user.DisplayName} is now live!")
                    .WithDescription($"[Twitch](https://twitch.tv/{user.Login})")
                    .WithColor(new Color(0x4A90E2))
                    .WithThumbnailUrl(user.ProfileImageUrl)
                    .AddField("Title", stream.Title, true).Build();

                var message = await channel.SendMessageAsync(string.Empty, embed: embed);

                _logger.LogInformation($"Successfully announced streamer [{streamer.Username}]");

                await _announcementMessageRepository.Add(new AnnouncementMessages
                {
                    MessageId = message.Id,
                    Streamer = streamer
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error occured while announcing streams: {exception}");
            }
        }
    }
}