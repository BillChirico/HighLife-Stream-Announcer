using TwitchLib.Api;
using TwitchLib.Api.Interfaces;

namespace HighLife.StreamAnnouncer.Service.Twitch
{
    public class TwitchApiFactory
    {
        /// <summary>
        ///     Create a new Twitch API instance
        /// </summary>
        /// <param name="clientId">Id of the Twitch client</param>
        /// <param name="secret">Client secret of the client</param>
        /// <returns>New Twitch API instance</returns>
        public static ITwitchAPI Create(string clientId, string secret)
        {
            TwitchAPI twitchApi = new TwitchAPI();

            twitchApi.Settings.ClientId = clientId;
            twitchApi.Settings.Secret = secret;

            return twitchApi;
        }
    }
}