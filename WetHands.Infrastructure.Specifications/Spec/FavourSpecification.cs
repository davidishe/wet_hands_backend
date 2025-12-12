using Core.Helpers;
using WetHands.Core.Models;

namespace WetHands.Infrastructure.Specifications
{
  public class FavourSpecification : BaseSpecification<Favour>
  {
    public FavourSpecification(UserParams userParams)
    : base(x =>
          (string.IsNullOrEmpty(userParams.Search)) &&
          (!userParams.typeId.HasValue)
        )
    {
      AddInclude(x => x.Order);
      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      AddOrderByDescending(x => x.CreatedAt);

      if (!string.IsNullOrEmpty(userParams.sort))
      {
        switch (userParams.sort)
        {
          case "ammount":
            AddOrderByAscending(s => s.CreatedAt);
            break;
          default:
            AddOrderByAscending(x => x.CreatedAt);
            break;
        }
      }
    }


    public FavourSpecification()
    : base()
    {
      AddInclude(x => x.Order);
    }




  }


}


// public FavourSpecification(int strapiProposalId) : base(x => x.StrapiProposalId == strapiProposalId)
// {
//   AddInclude(x => x.Requests);
//   AddInclude(z => z.Offers);
// }