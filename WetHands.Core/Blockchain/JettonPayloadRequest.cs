using System.Text.Json.Serialization;

public class JettonPayloadRequest
{
  [JsonPropertyName("userAddressText")]
  public string UserAddressText { get; set; }

  [JsonPropertyName("crowdfundigMasterAddressText")]
  public string CrowdfundigMasterAddressText { get; set; }

  [JsonPropertyName("jettonAmountText")]
  public string JettonAmountText { get; set; }

  [JsonPropertyName("refundPriceText")]
  public string RefundPriceText { get; set; }

  [JsonPropertyName("forwardTonAmountText")]
  public string ForwardTonAmountText { get; set; }

  [JsonPropertyName("jettonMasterContract")]
  public string JettonMasterContract { get; set; }

  [JsonPropertyName("userAddressHexText")]
  public string UserAddressHexText { get; set; }




}
