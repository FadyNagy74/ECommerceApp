using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace E_CommerceApp.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        //We want to be able to use a different context for each repository

        protected DbSet<T> _dbSet;
        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
            /*
            * DbSet<T> cannot be created with 'new' because:
            *  - It has no public constructor (EF Core controls its creation).
            *  - A DbSet must be tied to a specific DbContext instance so it can:
            *      • Track entity state (Added, Modified, Deleted).
            *      • Know which database/provider to query.
            *      • Use the DbContext’s change tracker and query services.
            *
            * Therefore, we always obtain a DbSet using context.Set<T>() 
             * or by exposing it as a property on the DbContext.
             */

        }
        public void Add(T entity) {
            _dbSet.Add(entity);
        }

        public async Task<T?> GetByIdAsync(string Id) {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<string>(e,"Id")==Id);
            //FindAsync returns null if nothing is found
        }

        public async Task<List<T>> GetAllAsync() {
            return await _dbSet.AsNoTracking().ToListAsync<T>();
        }

        public void Update(T entity) {
            _dbSet.Update(entity);
        }

        public void Remove(T entity) {
            _dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
/* The repository class is responsible for implementing the methods defined in the IRepository interface
 * once so we don't have to implement them in each repository class
 * we inject a DbContext instance so we are allowed to inject any context and work with any database
 */
