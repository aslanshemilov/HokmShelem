using Engine.Entities;
using System.Numerics;

namespace Engine.Repository
{
    public class BaseRepo<T> : IBaseRepo<T> where T : BaseEntity
    {
        private readonly Context _context;
        internal DbSet<T> contextSet;

        public BaseRepo(Context context)
        {
            _context = context;
            contextSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            contextSet.Add(entity);
        }
        public void Update(T source, T destination)
        {
            contextSet.Entry(source).CurrentValues.SetValues(destination);
        }
        public bool AnyByName(string name)
        {
            return contextSet.Any(x => x.Name.ToLower() == name.ToLower());
        }
        public T FindByName(string name, string includeProperties = null)
        {
            IQueryable<T> query = contextSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.Where(n => n.Name == name).FirstOrDefault();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string includeProperties = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null)
        {
            IQueryable<T> query = contextSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (orderby != null)
            {
                return orderby(query).ToList();
            }

            return query.ToList();
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = contextSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.FirstOrDefault();
        }
        public int Count()
        {
            throw new NotImplementedException();
        }
        public void Remove(T entity)
        {
            contextSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            contextSet.RemoveRange(entities);
        }
    }
}
