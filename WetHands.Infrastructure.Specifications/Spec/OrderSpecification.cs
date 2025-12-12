using Core.Helpers;
using WetHands.Core.Models;

namespace WetHands.Infrastructure.Specifications
{
  public class OrderSpecification : BaseSpecification<Order>
  {
    public OrderSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {
      AddInclude(x => x.OrderStatus);
      AddInclude(x => x.Company);
      AddOrderByDescending(x => x.UpdatedAt);

      if (!string.IsNullOrEmpty(userParams.sort))
      {
        switch (userParams.sort)
        {
          // case true:
          //   // AddOrderByAscending(s => s.CreatedAt);
          //   break;
          default:
            // AddOrderByAscending(x => x.CreatedAt);
            break;
        }

      }
    }


    public OrderSpecification() : base()
    {
      AddInclude(x => x.OrderStatus);
      AddInclude(x => x.Company);
      AddInclude(x => x.Items);
    }



    public OrderSpecification(int plotId) : base(x => x.Id == plotId)
    {
      AddInclude(x => x.OrderStatus);
      AddInclude(x => x.Company);
      AddInclude(x => x.Items);
      AddOrderByDescending(x => x.UpdatedAt);
    }

    public OrderSpecification(int authorId, bool param) : base(x => x.AuthorId == authorId)
    {
      AddInclude(x => x.OrderStatus);
      AddInclude(x => x.Company);
      AddInclude(x => x.Items);
      AddOrderByDescending(x => x.UpdatedAt);
    }








  }


}
