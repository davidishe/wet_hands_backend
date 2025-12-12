using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class BlockRef
    {
        [JsonProperty("workchain")]
        public int? Workchain { get; set; }

        [JsonProperty("shard")]
        public string Shard { get; set; }

        [JsonProperty("seqno")]
        public int? Seqno { get; set; }
    }
}