using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Entities;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Repository;

namespace HighLife.StreamAnnouncer.Service.Discord.Commands
{
    [Group("settings")]
    public class SettingsModule : ModuleBase
    {
        private readonly IDataStoreRepository<DiscordSettings> _discordSettingsRepository;
        private readonly IDataStoreRepository<PinnedMessage> _pinnedMessageRepository;

        public SettingsModule(IDataStoreRepository<DiscordSettings> discordSettingsRepository,
            IDataStoreRepository<PinnedMessage> pinnedMessageRepository)
        {
            _discordSettingsRepository = discordSettingsRepository;
            _pinnedMessageRepository = pinnedMessageRepository;
        }

        [Command("AddChannel")]
        [Summary("Sets the channel to announce the streams to.")]
        public async Task AddChannel(SocketGuildChannel channel)
        {
            await _discordSettingsRepository.Add(new DiscordSettings
            {
                DiscordChannelId = channel.Id
            });

            await ReplyAsync($"Successfully added announcement channel to [{channel.Name}]!");
        }

        [Command("SetPinnedMessage")]
        [Summary("Sets the pinned message to show all streamers.")]
        public async Task SetPinnedMessage(ulong messageId)
        {
            await _pinnedMessageRepository.Add(new PinnedMessage
            {
                MessageId = messageId
            });

            await ReplyAsync($"Successfully set pinned message to [{messageId}]!");
        }
    }
}