namespace Project_X.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        public Task AddAsync(T entity);
        public Task<T?> GetByIdAsync(params object[] keyValues);
        public Task<List<T>> GetAllAsync();
        public Task<List<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
