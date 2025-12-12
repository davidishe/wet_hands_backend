using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class AccountStateAfter
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("balance")]
        public string Balance { get; set; }

        [JsonProperty("account_status")]
        public string AccountStatus { get; set; }

        [JsonProperty("frozen_hash")]
        public object FrozenHash { get; set; }

        [JsonProperty("code_hash")]
        public string CodeHash { get; set; }

        [JsonProperty("data_hash")]
        public string DataHash { get; set; }
    }
}