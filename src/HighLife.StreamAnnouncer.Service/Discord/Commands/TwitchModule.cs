using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using HighLife.StreamAnnouncer.Domain.Entitites;
using HighLife.StreamAnnouncer.Repository;

namespace HighLife.StreamAnnouncer.Service.Discord.Commands
{
    [Group("twitch")]
    public class TwitchModule : ModuleBase
    {
        private readonly IDataStoreRepository<Streamer> _streamerRepository;

        public TwitchModule(IDataStoreRepository<Streamer> streamerRepository)
        {
            _streamerRepository = streamerRepository;
        }

        [Command("add")]
        [Summary("Adds a streamer to the database.")]
        public async Task Add(string twitchUsername, string tagLine)
        {
            IEnumerable<Streamer> streamers = _streamerRepository.GetCollection().AsQueryable();

            if (streamers.FirstOrDefault(s => s.Username == twitchUsername) != null)
            {
                await ReplyAsync("A streamer with that username already exists!");

                return;
            }

            await _streamerRepository.Add(new Streamer
            {
                Username = twitchUsername,
                TagLine = tagLine
            });

            await ReplyAsync($"Successfully added {twitchUsername} to the Live Check List");
        }

        [Command("remove")]
        [Summary("Remove a streamer from the database.")]
        public async Task Remove(string twitchUsername)
        {
            IEnumerable<Streamer> streamers = _streamerRepository.GetCollection().AsQueryable();

            Streamer streamer = streamers.FirstOrDefault(s => s.Username == twitchUsername);

            if (streamer == null)
            {
                await ReplyAsync("No streamer with that username is in the database!");

                return;
            }

            _streamerRepository.Delete(streamer);

            await ReplyAsync($"Successfully removed {twitchUsername} from the Live Check List");
        }
    }
}