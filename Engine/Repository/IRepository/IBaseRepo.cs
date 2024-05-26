namespace Engine.Repository.IRepository
{
    public interface IBaseRepo<T> where T : BaseEntity
    {
        void Add(T entity);
        void Update(T source, T destination);
        T FindByName(string name, string includeProperties = null);
        bool AnyByName(string name);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
           string includeProperties = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
            string includeProperties = null);
        int Count(Expression<Func<T, bool>> filter = null);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
