using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using HighLife.StreamAnnouncer.Domain.Settings;
using HighLife.StreamAnnouncer.Repository;

namespace HighLife.StreamAnnouncer.Service.Discord.Commands
{
    [Group("settings")]
    public class SettingsModule : ModuleBase
    {
        private readonly IDataStoreRepository<DiscordSettings> _discordSettingsRepository;

        public SettingsModule(IDataStoreRepository<DiscordSettings> discordSettingsRepository)
        {
            _discordSettingsRepository = discordSettingsRepository;
        }

        [Command("AddChannel")]
        [Summary("Sets the channel to announce the streams to.")]
        public async Task AddChannel(SocketGuildChannel channel)
        {
            await _discordSettingsRepository.Add(new DiscordSettings
            {
                DiscordChannelId = channel.Id
            });

            await ReplyAsync($"Successfully added announcement channel to {channel.Name}!");
        }
    }
}