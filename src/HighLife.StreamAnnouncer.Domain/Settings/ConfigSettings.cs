namespace HighLife.StreamAnnouncer.Domain.Settings
{
    public class ConfigSettings
    {
        public string DiscordBotToken { get; set; }

        public ulong DiscordGuildId { get; set; }

        public string TwitchClientId { get; set; }

        public string TwitchClientSecret { get; set; }
    }
}