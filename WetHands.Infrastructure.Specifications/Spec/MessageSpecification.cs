// using Core.Helpers;
// using WetHands.Core.Models.Messages;

// namespace WetHands.Infrastructure.Specifications
// {
//   public class MessageSpecification : BaseSpecification<Message>
//   {
//     public MessageSpecification(UserParams userParams)
//     : base(x =>
//           string.IsNullOrEmpty(userParams.Search)
//         )
//     {

//       ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
//       // AddOrderByDescending(x => x.CreatedAt);

//       if (!string.IsNullOrEmpty(userParams.sort))
//       {
//         switch (userParams.sort)
//         {
//           case "name":
//             // AddOrderByAscending(s => s.CreatedAt);
//             break;
//           default:
//             // AddOrderByAscending(x => x.CreatedAt);
//             break;
//         }
//       }
//     }


//     public MessageSpecification()
//     : base()
//     {
//       AddInclude(x => x.Images);
//     }

//     public MessageSpecification(int id) : base(x => x.Id == id)
//     {
//       AddInclude(x => x.Images);
//     }

//     /// непрочитанные сообщения
//     public MessageSpecification(int userId, double digit) : base(x => x.RecepientId == userId && x.IsReaded == false)
//     {
//     }

//     public MessageSpecification(int userId, bool param) : base(x => x.AuthorId == userId || x.RecepientId == userId)
//     {
//     }


//     public MessageSpecification(int userId, int recepientId) : base(x => (x.AuthorId == userId && x.RecepientId == recepientId) || (x.AuthorId == recepientId && x.RecepientId == userId))
//     {
//       AddInclude(x => x.Images);
//     }




//   }


// }