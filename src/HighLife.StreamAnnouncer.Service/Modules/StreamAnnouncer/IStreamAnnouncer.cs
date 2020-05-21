using System.Collections.Generic;
using System.Threading.Tasks;
using HighLife.StreamAnnouncer.Domain.Entities;

namespace HighLife.StreamAnnouncer.Service.Modules.StreamAnnouncer
{
    public interface IStreamAnnouncer : IModule
    {
        /// <summary>
        ///     Announce the streamers to the guild and channel specified in the settings.
        /// </summary>
        /// <param name="streamers">Streamers to announce.</param>
        /// <returns>List of live streamers.</returns>
        Task<List<Streamer>> Announce(IEnumerable<Streamer> streamers);

        /// <summary>
        ///     Update the pinned message for list of streamers.
        /// </summary>
        /// <param name="liveStreamers">Streamers that are live.</param>
        /// <param name="offlineStreamers">Streamers that are not live.</param>
        Task UpdatePinnedMessage(IEnumerable<Streamer> liveStreamers, IEnumerable<Streamer> offlineStreamers);
    }
}