using System;
using Core.Models;

namespace WetHands.Core
{
    public class NftSellRequest : BaseEntity
    {
        public string NewHolderAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsFinished { get; set; }
        public int? TokensAmmount { get; set; }
        public int PlotId { get; set; }
        public int NftPlotId { get; set; }
        public string OwnerWalletHexAddress { get; set; }
        // public string? VerificationGuId { get; set; }
        // public bool? IsVerifyed { get; set; }

    }
}