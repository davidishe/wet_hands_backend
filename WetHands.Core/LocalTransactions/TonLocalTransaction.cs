using System;
using Core.Models;

namespace WetHands.Core.TonModels
{
    public class TonLocalTransaction : BaseEntity
    {
        public int IsTransactionCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SourceAccount { get; set; }
        public string Hash { get; set; }
        public double AmmountNanoTons { get; set; }
        public int ParsedTokensQuantity { get; set; }
        public int ProjectId { get; set; }
        public string Base64BounceableAddress { get; set; }
        public string? Text { get; set; }

    }
}