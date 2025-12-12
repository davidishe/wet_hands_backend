using Newtonsoft.Json;

namespace WetHands.Core.TonModels.TonRates
{


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Diff24h
    {
        [JsonProperty("USD")]
        public string USD;
    }

    public class Diff30d
    {
        [JsonProperty("USD")]
        public string USD;
    }

    public class Diff7d
    {
        [JsonProperty("USD")]
        public string USD;
    }

    public class Prices
    {
        [JsonProperty("USD")]
        public double USD;
    }

    public class Rates
    {
        [JsonProperty("TON")]
        public TON TON;
    }

    public class RatesRoot
    {
        [JsonProperty("rates")]
        public Rates rates;
    }

    public class TON
    {
        [JsonProperty("prices")]
        public Prices prices;

        [JsonProperty("diff_24h")]
        public Diff24h diff_24h;

        [JsonProperty("diff_7d")]
        public Diff7d diff_7d;

        [JsonProperty("diff_30d")]
        public Diff30d diff_30d;
    }




}