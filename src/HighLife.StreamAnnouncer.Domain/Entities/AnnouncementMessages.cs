namespace HighLife.StreamAnnouncer.Domain.Entities
{
    public class AnnouncementMessages : Entity
    {
        public ulong MessageId { get; set; }

        public Streamer Streamer { get; set; }
    }
}