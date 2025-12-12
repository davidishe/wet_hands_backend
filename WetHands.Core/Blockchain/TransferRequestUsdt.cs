using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Models;
using Newtonsoft.Json;

public class UsdtTransferRequest : BaseEntity
{
  [JsonProperty("tokensAmount")]
  public int TokensAmount { get; set; }

  [JsonProperty("crowdfundigMasterAddressText")]
  public string CrowdfundigMasterAddressText { get; set; }

  [JsonProperty("campaignJettonMasterAddressText")]
  public string? CampaignJettonMasterAddressText { get; set; }

  [JsonProperty("campaignJettonMasterAddressHexText")]
  public string? CampaignJettonMasterAddressHexText { get; set; }

  [JsonProperty("userCampaignJettonWalletHex")]
  public string? UserCampaignJettonWalletHex { get; set; }

  [JsonProperty("userAddressHexText")]
  public string UserAddressHexText { get; set; }

  [JsonProperty("projectId")]
  public int ProjectId { get; set; }

  [JsonProperty("price")]
  public decimal Price { get; set; }

  [JsonProperty("userId")]
  public int UserId { get; set; }

  [JsonProperty("statusId")]
  public int StatusId { get; set; }

  [JsonProperty("createdAt")]
  public DateTime CreatedAt { get; set; }

  [JsonProperty("userAddressText")]
  public string UserAddressText { get; set; }

  [JsonProperty("userUsdtWalletText")]
  public string? UserUsdtWalletText { get; set; }

  [JsonProperty("base64Payload")]
  public string? Base64Payload { get; set; }


  [NotMapped]
  [JsonProperty("forwardTonAmountText")]
  public string? ForwardTonAmountText { get; set; }
}

public class UsdtTransferRequestDto
{
  [JsonProperty("tokensAmount")]
  public int TokensAmount { get; set; }

  [JsonProperty("crowdfundigMasterAddressText")]
  public string? CrowdfundigMasterAddressText { get; set; }

  [JsonProperty("campaignJettonMasterAddressText")]
  public string? CampaignJettonMasterAddressText { get; set; }

  [JsonProperty("campaignJettonMasterAddressHexText")]
  public string? CampaignJettonMasterAddressHexText { get; set; }

  [JsonProperty("userCampaignJettonWalletHex")]
  public string? UserCampaignJettonWalletHex { get; set; }

  [JsonProperty("userAddressHexText")]
  public string UserAddressHexText { get; set; }

  [JsonProperty("projectId")]
  public int ProjectId { get; set; }

  [JsonProperty("price")]
  public decimal Price { get; set; }

  [JsonProperty("userId")]
  public int UserId { get; set; }

  [JsonProperty("statusId")]
  public int StatusId { get; set; }

  [JsonProperty("createdAt")]
  public DateTime CreatedAt { get; set; }

  [JsonProperty("userAddressText")]
  public string UserAddressText { get; set; }

  [JsonProperty("userUsdtWalletText")]
  public string? UserUsdtWalletText { get; set; }

  [JsonProperty("base64Payload")]
  public string? Base64Payload { get; set; }

}
