namespace HighLife.StreamAnnouncer.Domain.Entitites
{
    public class AnnouncementMessages : Entity
    {
        public ulong MessageId { get; set; }

        public Streamer Streamer { get; set; }
    }
}