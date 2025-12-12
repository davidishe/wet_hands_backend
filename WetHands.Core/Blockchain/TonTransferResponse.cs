using System.Text.Json.Serialization;
using Core.Models;

public class TonTransferResponse : BaseEntity
{

  [JsonPropertyName("jettonAmountText")]
  public string JettonAmountText { get; set; }

  [JsonPropertyName("crowdfundigMasterAddressText")]
  public string CrowdfundigMasterAddressText { get; set; }

  [JsonPropertyName("userAddressHexText")]
  public string UserAddressHexText { get; set; }

  [JsonPropertyName("projectId")]
  public int ProjectId { get; set; }
}
