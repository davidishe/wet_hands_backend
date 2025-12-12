using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class StoragePh
    {
        [JsonProperty("status_change")]
        public string StatusChange { get; set; }

        [JsonProperty("storage_fees_collected")]
        public string StorageFeesCollected { get; set; }
    }
}