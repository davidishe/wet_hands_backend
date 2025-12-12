using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class ComputePh
    {
        [JsonProperty("mode")]
        public int? Mode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("success")]
        public bool? Success { get; set; }

        [JsonProperty("gas_fees")]
        public string GasFees { get; set; }

        [JsonProperty("gas_used")]
        public string GasUsed { get; set; }

        [JsonProperty("vm_steps")]
        public int? VmSteps { get; set; }

        [JsonProperty("exit_code")]
        public int? ExitCode { get; set; }

        [JsonProperty("gas_limit")]
        public string GasLimit { get; set; }

        [JsonProperty("gas_credit")]
        public string GasCredit { get; set; }

        [JsonProperty("msg_state_used")]
        public bool? MsgStateUsed { get; set; }

        [JsonProperty("account_activated")]
        public bool? AccountActivated { get; set; }

        [JsonProperty("vm_init_state_hash")]
        public string VmInitStateHash { get; set; }

        [JsonProperty("vm_final_state_hash")]
        public string VmFinalStateHash { get; set; }
    }
}