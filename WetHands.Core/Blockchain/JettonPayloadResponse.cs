using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class JettonPayloadResponse
{
  [JsonPropertyName("id")]
  public int? Id { get; set; }

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

  [JsonPropertyName("userJettonWallet")]
  public string UserJettonWallet { get; set; }

  [JsonPropertyName("jettonMasterContract")]
  public string JettonMasterContract { get; set; }

  [JsonPropertyName("userAddressHexText")]
  public string UserAddressHexText { get; set; }

  [JsonPropertyName("bouncableFriendly")]
  public string BouncableFriendly { get; set; }

  [JsonPropertyName("base64Payload")]
  public string Base64Payload { get; set; }

  [NotMapped]
  [JsonPropertyName("userCampaignJettonWalletHex")]
  public string? UserCampaignJettonWalletHex { get; set; }


}
