using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace WetHands.Infrastructure.Database
{
  public interface IDbRepository<TEntity> where TEntity : BaseEntity
  {
    IQueryable<TEntity> GetAll();
    IQueryable<TEntity> GetAllAsync();
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task SaveAsync();
    Task<TEntity> GetByIdAsync(int id);

  }
}
