using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WetHands.Core.TonModels
{
    public class TransactionReposnse
    {
        [JsonProperty("transactions")]
        public List<Transaction> Transactions { get; set; }

        [JsonProperty("address_book")]
        public Object AddressBook { get; set; }
    }
}