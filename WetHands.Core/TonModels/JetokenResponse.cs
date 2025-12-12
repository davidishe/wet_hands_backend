using System.Collections.Generic;
using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    // JetokenResponse myDeserializedClass = JsonConvert.DeserializeObject<JetokenResponse>(myJsonResponse);


    public class JettonWallet
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance")]
        public string Balance { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("jetton")]
        public string Jetton { get; set; }

        [JsonProperty("last_transaction_lt")]
        public string LastTransactionLt { get; set; }

        [JsonProperty("code_hash")]
        public string CodeHash { get; set; }

        [JsonProperty("data_hash")]
        public string DataHash { get; set; }
        // [JsonIgnore]
        public string? Nickname { get; set; }


    }

    public class JetokenResponse
    {
        [JsonProperty("jetton_wallets")]
        public List<JettonWallet> JettonWallets { get; set; }

        [JsonProperty("address_book")]
        public object AddressBook { get; set; }

    }


}