using Core.Helpers;
using WetHands.Core.Models.Items;

namespace WetHands.Infrastructure.Specifications
{
  public class ImageSpecification : BaseSpecification<Picture>
  {
    public ImageSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {
      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      // AddOrderByDescending(x => x.UpdatedAt);

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


    public ImageSpecification()
    : base()
    {
      // AddInclude(x => x.Messages);
      // AddInclude(x => x.Status);
    }




    public ImageSpecification(int plotId) : base(x => x.PlotId == plotId)
    {
      // AddOrderByDescending(x => x.UpdatedAt);
    }




  }


}