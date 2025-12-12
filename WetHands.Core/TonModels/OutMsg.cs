using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{

    public class OutMsg
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("fwd_fee")]
        public string FwdFee { get; set; }

        [JsonProperty("ihr_fee")]
        public string IhrFee { get; set; }

        [JsonProperty("created_lt")]
        public string CreatedLt { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("opcode")]
        public string Opcode { get; set; }

        [JsonProperty("ihr_disabled")]
        public bool? IhrDisabled { get; set; }

        [JsonProperty("bounce")]
        public bool? Bounce { get; set; }

        [JsonProperty("bounced")]
        public bool? Bounced { get; set; }

        [JsonProperty("import_fee")]
        public object ImportFee { get; set; }

        [JsonProperty("message_content")]
        public MessageContent MessageContent { get; set; }

        [JsonProperty("init_state")]
        public object InitState { get; set; }
    }
}