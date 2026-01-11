
using Microsoft.EntityFrameworkCore;
using Project_X.Data.Context;
using Project_X.Data.Repositories;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project_X.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbConext _context;
        private readonly DbSet<T> _dbSet;
        private AppDbConext context;

        public Repository(AppDbConext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
           await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<T>> GetAllAsync()
        {
            var entities=  await _dbSet.ToListAsync();
            return entities;
        }

        public async Task<T?> GetByIdAsync(params object[] keyValues)
        {
           var entity = await _dbSet.FindAsync(keyValues);
           return entity;
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public void Update(T entity)
        {
           _dbSet.Update(entity);
        }
    }
}
