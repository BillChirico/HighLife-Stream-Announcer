using System.Threading.Tasks;

namespace HighLife.StreamAnnouncer.Service.Discord
{
    public interface IDiscordBot
    {
        Task Connect(string token);
    }
}