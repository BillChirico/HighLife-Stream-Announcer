using System.Threading.Tasks;

namespace HighLife.StreamAnnouncer.Service.Modules
{
    public interface IModule
    {
        /// <summary>
        ///     Initialize the Module.
        /// </summary>
        Task Init();
    }
}