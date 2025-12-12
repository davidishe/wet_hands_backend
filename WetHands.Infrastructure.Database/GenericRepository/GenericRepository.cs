using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using WetHands.Infrastructure.Specifications;
using Microsoft.Extensions.Logging;

namespace WetHands.Infrastructure.Database
{
  public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
  {

    private readonly AppDbContext _context;
    private readonly ILogger<GenericRepository<T>> _logger;



    public GenericRepository(
      AppDbContext context,
      ILogger<GenericRepository<T>> logger
    )
    {
      _context = context;
      _logger = logger;
    }


    // generic
    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
      return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
      // return await _context.Set<T>().FindAsync(id);
      var entity = await Task.Run(() => _context.Set<T>().Where(x => x.Id == id).FirstOrDefault());
      await _context.SaveChangesAsync();
      return entity;
    }

    public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
    {
      return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
      return await ApplySpecification(spec).ToListAsync();
    }



    public async Task<T> AddAsync(T entity)
    {
      await _context.Set<T>().AddAsync(entity);
      await _context.SaveChangesAsync();
      return entity;
    }

    public async Task UpdateAsync(T entity)
    {
      await Task.Run(() => _context.Set<T>().Update(entity));
      await _context.SaveChangesAsync();
    }

    async Task IGenericRepository<T>.Delete(T entity)
    {
      _context.Set<T>().Remove(entity);
      await _context.SaveChangesAsync();
    }


    public async Task<int> CountAsync(ISpecification<T> spec)
    {
      return await ApplySpecification(spec).CountAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
      return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
    }


  }


}