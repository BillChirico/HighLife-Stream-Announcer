using HighLife.StreamAnnouncer.Domain.Entities;

namespace HighLife.StreamAnnouncer.Domain.Settings
{
    public class DiscordSettings : Entity
    {
        public ulong DiscordChannelId { get; set; }
    }
}