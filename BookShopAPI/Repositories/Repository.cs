using BookShopAPI.Data;
using BookShopAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
enum order
{
    up,
    down
}

namespace BookShopAPI.Repositories
{
    public class Repository<T>: IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<EntityEntry<T>> InsertAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
             _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }


        public IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = _dbSet.AsQueryable();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
            if (filter != null)
            {
                entities = entities.Where(filter);
            }
            
            if (!isTracked)
            {
                entities = entities.AsNoTracking();
            }
            
            return  entities;
        }

       

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T,bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true,
            int order = 0,
            Expression<Func<T, object>>? ordering = null,
            int top = 0
            )
        {
            var entities = Query(filter, includes,isTracked);
            if(ordering != null)
            {
                if(order == 0 )
                {
                    entities = entities.OrderBy(ordering);
                }
                if(order == 1)
                {
                    entities = entities.OrderByDescending(ordering);
                }
            }
            if(top > 0)
            {
                entities = entities.Skip(0).Take(top);
            }
            return await entities.ToListAsync();
        }


        public async Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            
            bool isTracked = true,
            int order = 0,
            Expression<Func<T, object>>? ordering = null
            )
        {
            var entities = Query(filter, includes, isTracked);
            if (ordering != null)
            {
                if (order == 0)
                {
                    entities = entities.OrderBy(ordering);
                }
                if (order == 1)
                {
                    entities = entities.OrderByDescending(ordering);
                }
            }
            return await entities.FirstOrDefaultAsync();
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
