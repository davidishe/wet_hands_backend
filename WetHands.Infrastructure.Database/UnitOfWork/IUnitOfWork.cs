using System;
using System.Threading.Tasks;
using Core.Models;

namespace WetHands.Infrastructure.Database.UnitOfWork
{
  public interface IUnitOfWork : IDisposable
  {
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> Complete();
  }
}