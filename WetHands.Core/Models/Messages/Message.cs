// using System;
// using System.Collections.Generic;
// using Core.Models;

// namespace WetHands.Core.Models.Messages
// {
//     public class Message : BaseEntity
//     {
//         public int AuthorId { get; set; }
//         public int RecepientId { get; set; }
//         public int ChatId { get; set; }
//         public string GuId { get; set; }
//         public string Text { get; set; }
//         public DateTime CreatedAt { get; set; }
//         public bool IsReaded { get; set; }

//         //TODO: new fields to prod
//         public bool WithImages { get; set; }
//         public ICollection<Image>? Images { get; set; }


//     }
// }