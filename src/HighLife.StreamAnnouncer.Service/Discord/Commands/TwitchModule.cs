using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using HighLife.StreamAnnouncer.Domain.Entities;
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
        public async Task Add(string twitchUsername, uint hexColor, [Remainder] string tagLine)
        {
            var streamers = _streamerRepository.GetCollection().AsQueryable();

            if (streamers.FirstOrDefault(s => s.Username == twitchUsername) != null)
            {
                await ReplyAsync("A streamer with that username already exists!");

                return;
            }

            await _streamerRepository.Add(new Streamer
            {
                Username = twitchUsername,
                TagLine = tagLine,
                HexColor = hexColor
            });

            await ReplyAsync($"Successfully added {twitchUsername} to the Live Check List!");
        }

        [Command("remove")]
        [Summary("Remove a streamer from the database.")]
        public async Task Remove(string twitchUsername)
        {
            var streamers = _streamerRepository.GetCollection().AsQueryable();

            var streamer = streamers.FirstOrDefault(s => s.Username == twitchUsername);

            if (streamer == null)
            {
                await ReplyAsync("No streamer with that username is in the database!");

                return;
            }

            await _streamerRepository.Delete(streamer);

            await ReplyAsync($"Successfully removed {twitchUsername} from the Live Check List!");
        }

        [Command("update")]
        [Summary("Updates a streamer in the database.")]
        public async Task Update(string twitchUsername, uint hexColor, [Remainder] string tagLine)
        {
            var streamers = _streamerRepository.GetCollection().AsQueryable();

            var streamer = streamers.FirstOrDefault(s => s.Username == twitchUsername);

            if (streamer == null)
            {
                await ReplyAsync("No streamer with that username is in the database!");

                return;
            }

            streamer.HexColor = hexColor;
            streamer.TagLine = tagLine;

            await _streamerRepository.Update(streamer);

            await ReplyAsync($"Successfully updated {twitchUsername}!");
        }
    }
}