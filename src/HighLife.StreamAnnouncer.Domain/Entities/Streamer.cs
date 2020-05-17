namespace HighLife.StreamAnnouncer.Domain.Entities
{
    public class Streamer : Entity
    {
        public string Username { get; set; }

        public string TagLine { get; set; }

        public uint HexColor { get; set; }
    }
}