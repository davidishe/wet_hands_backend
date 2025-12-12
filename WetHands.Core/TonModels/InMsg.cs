using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{

    public class InMsg
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("source")]
        public object Source { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("fwd_fee")]
        public object FwdFee { get; set; }

        [JsonProperty("ihr_fee")]
        public object IhrFee { get; set; }

        [JsonProperty("created_lt")]
        public object CreatedLt { get; set; }

        [JsonProperty("created_at")]
        public object CreatedAt { get; set; }

        [JsonProperty("opcode")]
        public string Opcode { get; set; }

        [JsonProperty("ihr_disabled")]
        public object IhrDisabled { get; set; }

        [JsonProperty("bounce")]
        public object Bounce { get; set; }

        [JsonProperty("bounced")]
        public object Bounced { get; set; }

        [JsonProperty("import_fee")]
        public string ImportFee { get; set; }

        [JsonProperty("message_content")]
        public MessageContent MessageContent { get; set; }

        [JsonProperty("init_state")]
        public object InitState { get; set; }
    }

}