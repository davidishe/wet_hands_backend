using System;
using Core.Models;

namespace WetHands.Core.TonModels
{
    public class TrxPayment : BaseEntity
    {
        public int IsTransactionCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DestWalletHexAddress { get; set; }
        // public string? Hash { get; set; }
        public double? PaymentAmountFiat { get; set; }
        public double PaymentAmmountProductTokens { get; set; }
        public int ProjectId { get; set; }
        public string? TextDestination { get; set; }
        public string? PayerWalletAddress { get; set; }
        public string? VerificationGuId { get; set; }
        public bool? IsVerifyed { get; set; }


        // Base64BounceableAddress
        // TODO: сдеать обязательным
        // public string? TransactionGuId { get; set; }
    }
}