using System.Collections.Generic;
using System.Threading.Tasks;
using WetHands.Infrastructure.Specifications;
using Core.Models;

namespace WetHands.Infrastructure.Database

{
  public interface IGenericRepository<T> where T : BaseEntity
  {
    Task<T> GetByIdAsync(int id);
    // Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task Delete(T entity);


  }
}