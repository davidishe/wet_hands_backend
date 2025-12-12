using Core.Helpers;
using WetHands.Core.Models.Items;

namespace WetHands.Infrastructure.Specifications
{
  public class FileSpecification : BaseSpecification<File>
  {
    public FileSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {
      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);

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


    public FileSpecification()
    : base()
    {
    }



    public FileSpecification(int plotId) : base(x => x.PlotId == plotId)
    {
    }

    public FileSpecification(int fileId, bool param) : base(x => x.Id == fileId)
    {
    }








  }


}