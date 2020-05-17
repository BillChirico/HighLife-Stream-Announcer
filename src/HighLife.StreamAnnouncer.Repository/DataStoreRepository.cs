using System.Collections.Generic;
using System.Threading.Tasks;
using HighLife.StreamAnnouncer.Domain.Entities;
using JsonFlatFileDataStore;

namespace HighLife.StreamAnnouncer.Repository
{
    public class DataStoreRepository<T> : IDataStoreRepository<T> where T : Entity
    {
        private readonly IDataStore _db;

        public DataStoreRepository(IDataStore db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public IDocumentCollection<T> GetCollection()
        {
            return _db.GetCollection<T>();
        }

        /// <inheritdoc />
        public IEnumerable<T> GetById(int id)
        {
            var collection = _db.GetCollection<T>();

            return collection.Find(item => item.Id == id);
        }

        /// <inheritdoc />
        public async Task<T> Add(T item)
        {
            var collection = _db.GetCollection<T>();

            await collection.InsertOneAsync(item);

            return item;
        }

        /// <inheritdoc />
        public async void Delete(T item)
        {
            var collection = _db.GetCollection<T>();

            await collection.DeleteOneAsync(item);
        }

        /// <inheritdoc />
        public async Task<T> Update(T item)
        {
            var collection = _db.GetCollection<T>();

            await collection.UpdateOneAsync(item.Id, item);

            return item;
        }
    }
}