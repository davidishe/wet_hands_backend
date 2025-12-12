using Newtonsoft.Json;
using TonSdk.Core;

namespace WetHands.Core.TonModels.JettonTransaction
{
    public class JettonTransaction
    {
        [JsonProperty("Operation")]
        public int? Operation { get; set; }

        [JsonProperty("QueryId")]
        public long? QueryId { get; set; }

        [JsonProperty("Amount")]
        public Coins Amount { get; set; }

        [JsonProperty("ForwardTonAmount")]
        public ForwardTonAmount ForwardTonAmount { get; set; }

        [JsonProperty("Source")]
        public Address Source { get; set; }

        [JsonProperty("Destination")]
        public object Destination { get; set; }

        [JsonProperty("Comment")]
        public object Comment { get; set; }

        [JsonProperty("Data")]
        public Data Data { get; set; }

        [JsonProperty("Transaction")]
        public Transaction Transaction { get; set; }
    }
}