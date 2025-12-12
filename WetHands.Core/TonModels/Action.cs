using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class Action
    {
        [JsonProperty("valid")]
        public bool? Valid { get; set; }

        [JsonProperty("success")]
        public bool? Success { get; set; }

        [JsonProperty("no_funds")]
        public bool? NoFunds { get; set; }

        [JsonProperty("result_code")]
        public int? ResultCode { get; set; }

        [JsonProperty("tot_actions")]
        public int? TotActions { get; set; }

        [JsonProperty("msgs_created")]
        public int? MsgsCreated { get; set; }

        [JsonProperty("spec_actions")]
        public int? SpecActions { get; set; }

        [JsonProperty("tot_msg_size")]
        public TotMsgSize TotMsgSize { get; set; }

        [JsonProperty("status_change")]
        public string StatusChange { get; set; }

        [JsonProperty("total_fwd_fees")]
        public string TotalFwdFees { get; set; }

        [JsonProperty("skipped_actions")]
        public int? SkippedActions { get; set; }

        [JsonProperty("action_list_hash")]
        public string ActionListHash { get; set; }

        [JsonProperty("total_action_fees")]
        public string TotalActionFees { get; set; }
    }
}