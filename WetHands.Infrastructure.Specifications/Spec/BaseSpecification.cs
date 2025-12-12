using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WetHands.Infrastructure.Specifications
{
  public class BaseSpecification<T> : ISpecification<T>
  {
    public BaseSpecification()
    {
    }

    public Expression<Func<T, bool>> Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
    public Expression<Func<T, object>> OrderByAscending { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }


    public BaseSpecification(Expression<Func<T, bool>> criteria)
    {
      Criteria = criteria;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
      Includes.Add(includeExpression);
    }

    protected void AddOrderByAscending(Expression<Func<T, object>> orderByAscExpression)
    {
      OrderByAscending = orderByAscExpression;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
      OrderByDescending = orderByDescExpression;
    }

    protected void ApplyPaging(int skip, int take)
    {
      Skip = skip;
      Take = take;
      IsPagingEnabled = true;
    }

  }
}