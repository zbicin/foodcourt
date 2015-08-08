using System;
using System.Linq;
using System.Threading.Tasks;
using FoodCourt.Model.Identity;

namespace FoodCourt.Service.Repository
{
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Tries to retrieve single record using it's primary key. Throws exception if fails.
        /// </summary>
        /// <param name="primaryKey">The primary key of the record</param>
        /// <returns>T</returns>
        Task<T> Single(Guid primaryKey, bool withDeleted = false, string include = "");

        /// <summary>
        /// Retrieve a single item using it's primary key, null if not found
        /// </summary>
        /// <param name="primaryKey">The primary key of the record</param>
        /// <returns>T</returns>
        Task<T> SingleOrDefault(Guid primaryKey, bool withDeleted = false, string include = "");

        /// <summary>
        /// Returns all the rows for type T
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll(bool withDeleted = false, string include = "");

        /// <summary>
        /// Does this item exist by it's primary key
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        Task<bool> Exists(Guid primaryKey, bool withDeleted = false);

        /// <summary>
        /// Inserts the data into the table
        /// </summary>
        /// <param name="entity">The entity to insert</param>
        /// <returns></returns>
        Task<int> Insert(T entity);

        /// <summary>
        /// Updates this entity in the database using it's primary key
        /// </summary>
        /// <param name="entity">The entity to update</param>
        Task<int> Update(T entity);



        /// <summary>
        /// Deletes this entry fro the database
        /// ** WARNING - Most items should be marked inactive and Updated, not deleted
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <returns></returns>
        Task<int> Delete(T entity);

        IUnitOfWork UnitOfWork { get; }
        IApplicationUser CurrentUser { get; set; }
    }
}