namespace Project_X.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        public Task AddAsync(T entity);
        public Task<T?> GetByIdAsync(params object[] keyValues);
        public Task<List<T>> GetAllAsync();
        public void Update(T entity);
        public void Delete(T entity);
    }
}
