using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class TotMsgSize
    {
        [JsonProperty("bits")]
        public string Bits { get; set; }

        [JsonProperty("cells")]
        public string Cells { get; set; }
    }
}