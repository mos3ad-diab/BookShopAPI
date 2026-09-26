using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace BookShopAPI.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<EntityEntry<T>> InsertAsync(T entity);


        void Update(T entity);

        void Delete(T entity);



        IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            );


        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true,
            int order = 0,
            Expression<Func<T, object>>? ordering = null,
            int top = 0
            );



        Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,

            bool isTracked = true,
            int order = 0,
            Expression<Func<T, object>>? ordering = null
            );


        Task<int> CommitAsync();
        
    }
}

