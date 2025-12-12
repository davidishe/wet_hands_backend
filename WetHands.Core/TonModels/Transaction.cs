using System.Collections.Generic;
using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class Transaction
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("lt")]
        public string Lt { get; set; }

        [JsonProperty("now")]
        public int? Now { get; set; }

        [JsonProperty("orig_status")]
        public string OrigStatus { get; set; }

        [JsonProperty("end_status")]
        public string EndStatus { get; set; }

        [JsonProperty("total_fees")]
        public string TotalFees { get; set; }

        [JsonProperty("prev_trans_hash")]
        public string PrevTransHash { get; set; }

        [JsonProperty("prev_trans_lt")]
        public string PrevTransLt { get; set; }

        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("block_ref")]
        public BlockRef BlockRef { get; set; }

        [JsonProperty("in_msg")]
        public InMsg InMsg { get; set; }

        [JsonProperty("out_msgs")]
        public List<OutMsg> OutMsgs { get; set; }

        [JsonProperty("account_state_before")]
        public AccountStateBefore AccountStateBefore { get; set; }

        [JsonProperty("account_state_after")]
        public AccountStateAfter AccountStateAfter { get; set; }

        [JsonProperty("mc_block_seqno")]
        public int? McBlockSeqno { get; set; }
    }
}