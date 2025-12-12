using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Models;

namespace WetHands.Core
{
    public class NftPlot : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string TraitType { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public string? PinataHash { get; set; }
        public string? TonAddress { get; set; }
        public int PlotId { get; set; }
        public int Area { get; set; }
        public int? Index { get; set; }
        public bool IsTransfered { get; set; }
        public string? SrcJettonBouncableRecepientWalletAddress { get; set; }
        public string? OwnerWalletHexAddress { get; set; }
        [NotMapped]
        public string DesilindeUrl { get; set; } = "https://rose-selective-kingfisher-204.mypinata.cloud/ipfs/QmeCZhEFjKeNqiJeFAk8ZcPwmoJwU9XdwzERcj6HgAmoLb";

        [NotMapped]
        public string KadastrNumber { get; set; } = "414305015212";
        [NotMapped]
        public double Ltd { get; set; } = 25.59;

        [NotMapped]
        public double Lng { get; set; } = 97.11;

        public string TokenType { get; set; }


        // CreatedAt = DateTime.Now,
        // UpdatedAt = DateTime.Now,
        // SrcJettonBouncableWalletAddress = srcBouncableJettonWalletAddress.ToString(),


    }
}