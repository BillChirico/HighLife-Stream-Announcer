using System.Collections.Generic;
using System.Threading.Tasks;
using JsonFlatFileDataStore;

namespace HighLife.StreamAnnouncer.Repository
{
    public interface IDataStoreRepository<T> where T : class
    {
        IDocumentCollection<T> GetCollection();

        IEnumerable<T> Get(int id);

        Task<T> Add(T item);

        void Delete(T item);

        Task<T> Update(T item);
    }
}