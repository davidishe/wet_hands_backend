using Core.Helpers;
using WetHands.Core.Models.Messages;

namespace WetHands.Infrastructure.Specifications
{
  public class CorrespondentSpecification : BaseSpecification<Correspondent>
  {
    public CorrespondentSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {
      // AddInclude(x => x.Status);

      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      // AddOrderByDescending(x => x.CreatedAt);

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


    public CorrespondentSpecification()
    : base()
    {
      // AddInclude(x => x.Status);
    }

    // public CorrespondentSpecification(int id, bool some) : base(x => x.Id == id)
    // {
    //   // AddInclude(x => x.Status);
    // }


    public CorrespondentSpecification(int userId) : base(x => x.CoresspondentId == userId)
    {
    }

    public CorrespondentSpecification(int userId, int recepientId) : base(x => (x.CoresspondentId == userId && x.AnotherCoresspondentId == recepientId) || (x.CoresspondentId == recepientId && x.AnotherCoresspondentId == userId))
    {
    }




  }


}