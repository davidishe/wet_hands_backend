using System;
using System.Text.Json.Serialization;
using Core.Models;

public class TonTransferRequest : BaseEntity
{

  [JsonPropertyName("tokensAmount")]
  public string TokensAmount { get; set; }

  [JsonPropertyName("crowdfundigMasterAddressText")]
  public string CrowdfundigMasterAddressText { get; set; }

  [JsonPropertyName("userAddressHexText")]
  public string UserAddressHexText { get; set; }

  [JsonPropertyName("projectId")]
  public int ProjectId { get; set; }

  [JsonPropertyName("jettonMasterContract")]
  public string JettonMasterContract { get; set; }

  [JsonPropertyName("price")]
  public double Price { get; set; }

  [JsonPropertyName("userId")]
  public int? UserId { get; set; }

  [JsonPropertyName("statusId")]
  public int? StatusId { get; set; }

  [JsonPropertyName("createdAt")]
  public DateTime? CreatedAt { get; set; }

}
