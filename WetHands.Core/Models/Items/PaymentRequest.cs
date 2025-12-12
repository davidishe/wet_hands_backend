using System;
using Core.Models;

namespace WetHands.Core.Models
{

  /// <summary>
  /// здесь храним заказы, которые добавлены в избранное или заказаны
  /// </summary>
  public class PaymentRequest : BaseEntity
  {
    public DateTime CreatedAt { get; set; }
    public PaymentRequestStatus? Status { get; set; }
    public int PaymentRequestStatusId { get; set; }
    public PaymentRequestType? PaymentRequestType { get; set; }
    public int PaymentRequestTypeId { get; set; }
    public string? VerificationGuId { get; set; }
    public int AgentId { get; set; }
    public string TokenType { get; set; }
    public string InvestorAddress { get; set; }
    public string PayerAddress { get; set; }
    public int TokensQuantity { get; set; }
    public double? TokensTonAmmount { get; set; }



  }
}

