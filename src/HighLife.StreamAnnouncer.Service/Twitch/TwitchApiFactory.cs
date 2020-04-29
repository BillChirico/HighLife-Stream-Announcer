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
        /// <param name="accessToken">Access token of the client</param>
        /// <returns>New Twitch API instance</returns>
        public static ITwitchAPI Create(string clientId, string accessToken)
        {
            TwitchAPI twitchApi = new TwitchAPI();

            twitchApi.Settings.ClientId = clientId;
            twitchApi.Settings.AccessToken = accessToken;

            return twitchApi;
        }
    }
}