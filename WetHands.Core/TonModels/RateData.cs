using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Diff24h
    {
        [JsonProperty("USD")]
        public string USD { get; set; }
    }

    public class Diff30d
    {
        [JsonProperty("USD")]
        public string USD { get; set; }
    }

    public class Diff7d
    {
        [JsonProperty("USD")]
        public string USD { get; set; }
    }

    public class Prices
    {
        [JsonProperty("USD")]
        public double USD { get; set; }
    }

    public class Rates
    {
        [JsonProperty("TON")]
        public TON TON { get; set; }
    }

    public class RateData
    {
        [JsonProperty("rates")]
        public Rates Rates { get; set; }
    }

    public class TON
    {
        [JsonProperty("prices")]
        public Prices Prices { get; set; }

        [JsonProperty("diff_24h")]
        public Diff24h Diff24h { get; set; }

        [JsonProperty("diff_7d")]
        public Diff7d Diff7d { get; set; }

        [JsonProperty("diff_30d")]
        public Diff30d Diff30d { get; set; }
    }


}