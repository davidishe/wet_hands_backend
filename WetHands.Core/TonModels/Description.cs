using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{

    public class Description
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("action")]
        public Action Action { get; set; }

        [JsonProperty("aborted")]
        public bool? Aborted { get; set; }

        [JsonProperty("credit_ph")]
        public CreditPh CreditPh { get; set; }

        [JsonProperty("destroyed")]
        public bool? Destroyed { get; set; }

        [JsonProperty("compute_ph")]
        public ComputePh ComputePh { get; set; }

        [JsonProperty("storage_ph")]
        public StoragePh StoragePh { get; set; }

        [JsonProperty("credit_first")]
        public bool? CreditFirst { get; set; }
    }

}