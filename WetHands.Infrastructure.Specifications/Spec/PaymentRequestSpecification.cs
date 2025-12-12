using Core.Helpers;
using WetHands.Core.Models;

namespace WetHands.Infrastructure.Specifications
{
  public class PaymentRequestSpecification : BaseSpecification<PaymentRequest>
  {
    public PaymentRequestSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {
      AddInclude(x => x.Status);
      AddInclude(x => x.PaymentRequestType);
      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      AddOrderByDescending(x => x.CreatedAt);

      if (!string.IsNullOrEmpty(userParams.sort))
      {
        switch (userParams.sort)
        {
          case "name":
            // AddOrderByAscending(s => s.CreatedAt);
            break;
          default:
            // AddOrderByAscending(x => x.CreatedAt);
            break;
        }
      }
    }





    public PaymentRequestSpecification() : base()
    {
      AddInclude(x => x.Status);
      AddInclude(x => x.PaymentRequestType);
      AddOrderByDescending(x => x.CreatedAt);
    }

    public PaymentRequestSpecification(int requestId) : base(x => x.Id == requestId)
    {
      AddInclude(x => x.Status);
      AddInclude(x => x.PaymentRequestType);
      AddOrderByDescending(x => x.CreatedAt);
    }



  }


}