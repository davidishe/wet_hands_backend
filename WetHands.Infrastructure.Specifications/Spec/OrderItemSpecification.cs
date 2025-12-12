using Core.Helpers;

namespace WetHands.Infrastructure.Specifications
{
  public class OrderItemSpecification : BaseSpecification<OrderItem>
  {
    public OrderItemSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {

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


    public OrderItemSpecification() : base()
    {

    }



    public OrderItemSpecification(int plotId) : base(x => x.Id == plotId)
    {

    }

    public OrderItemSpecification(int id, bool param) : base(x => x.OrderId == id)
    {

    }








  }


}