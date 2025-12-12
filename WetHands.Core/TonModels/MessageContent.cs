using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class MessageContent
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("decoded")]
        public Decoded Decoded { get; set; }
    }

    public class Decoded
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("comment")]
        public string? Comment { get; set; }
    }
}