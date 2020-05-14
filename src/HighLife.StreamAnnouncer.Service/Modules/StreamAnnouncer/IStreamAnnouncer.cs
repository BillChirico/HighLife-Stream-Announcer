using System.Threading.Tasks;
using HighLife.StreamAnnouncer.Domain.Entitites;

namespace HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer
{
    public interface IStreamAnnouncer
    {
        /// <summary>
        ///     Announce the streamer to the guild and channel specified in the settings.
        /// </summary>
        /// <param name="streamer">Streamer to announce.</param>
        /// <returns></returns>
        Task Announce(Streamer streamer);
    }
}