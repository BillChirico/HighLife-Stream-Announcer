using System.Collections.Generic;
using System.Threading.Tasks;
using JsonFlatFileDataStore;

namespace HighLife.StreamAnnouncer.Repository
{
    public interface IDataStoreRepository<T> where T : class
    {
        /// <summary>
        ///     Gets the collection in the database.
        /// </summary>
        IDocumentCollection<T> GetCollection();

        /// <summary>
        ///     Gets an entity by id from the database.
        /// </summary>
        /// <param name="id">Id of the entity to retrieve.</param>
        /// <returns>Entity with the specified id.</returns>
        IEnumerable<T> GetById(int id);

        /// <summary>
        ///     Adds a new entity to the database.
        /// </summary>
        /// <param name="item">Entity to add.</param>
        /// <returns>Created entity.</returns>
        Task<T> Add(T item);

        /// <summary>
        ///     Delete an entity from the database.
        /// </summary>
        /// <param name="item">Entity to delete.</param>
        void Delete(T item);

        /// <summary>
        ///     Updates an entity in the database.
        /// </summary>
        /// <param name="item">Entity to update.</param>
        /// <returns>Updated entity.</returns>
        Task<T> Update(T item);
    }
}