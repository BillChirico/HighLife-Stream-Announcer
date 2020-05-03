using System.Threading.Tasks;
using HighLife.StreamAnnouncer.Domain.Entitites;

namespace HighLife.StreamAnnouncer.Service.StreamAnnouncer
{
    public interface IStreamAnnouncer
    {
        Task Announce(Streamer streamer, ulong guildId, ulong channelId);
    }
}