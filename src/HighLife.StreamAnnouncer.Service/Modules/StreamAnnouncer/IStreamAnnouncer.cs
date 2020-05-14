using System.Threading.Tasks;
using HighLife.StreamAnnouncer.Domain.Entitites;

namespace HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer
{
    public interface IStreamAnnouncer
    {
        Task Announce(Streamer streamer, ulong guildId, ulong channelId);
    }
}