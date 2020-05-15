using System.Threading.Tasks;
using HighLife.StreamAnnouncer.Domain.Entities;

namespace HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer
{
    public interface IStreamAnnouncer : IModule
    {
        /// <summary>
        ///     Announce the streamer to the guild and channel specified in the settings.
        /// </summary>
        /// <param name="streamer">Streamer to announce.</param>
        /// <returns></returns>
        Task Announce(Streamer streamer);
    }
}