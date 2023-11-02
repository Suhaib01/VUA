using Microsoft.AspNetCore.Identity;

namespace VUA.Core.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void AddAsync(T entity);
        Task Update(T entity);
        bool UpdateWithIdentityResult(T entity);
        void Delete(T entity);
        T Find(int id);
        List<T> GetValues(string col);



    }
}
