using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;

namespace HighLife.StreamAnnouncer.Service.Twitch
{
    /// <summary>
    ///     Helper methods for the Twitch API
    /// </summary>
    public interface ITwitchApiHelper
    {
        /// <summary>
        ///     Get the Twitch user.
        /// </summary>
        /// <param name="username">Username of the user.</param>
        /// <returns>Twitch user.</returns>
        Task<User> GetUser(string username);

        /// <summary>
        ///     Get the current stream of the user.
        /// </summary>
        /// <param name="user">User to get the stream.</param>
        /// <returns>Stream of the current user or null if they are not streaming.</returns>
        Task<Stream> GetStream(User user);

        /// <summary>
        ///     Get the game that the user is currently streaming.
        /// </summary>
        /// <param name="stream">Stream to get the game from.</param>
        /// <returns>Game that the user is streaming or null if not found.</returns>
        Task<Game> GetStreamGame(Stream stream);
    }
}