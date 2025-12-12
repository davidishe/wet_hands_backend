using System;

namespace WetHands.Core.Models
{

  /// <summary>
  /// здесь храним заказы, которые добавлены в избранное или заказаны
  /// </summary>
  public class PaymentRequestDto
  {
    public int? Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Status { get; set; }
    public int PaymentRequestStatusId { get; set; }
    public string? PaymentRequestType { get; set; }
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

