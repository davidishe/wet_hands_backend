using System.Collections.Generic;
using Newtonsoft.Json;

namespace WetHands.Core.TonModels.JettonTransaction
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Amount
    {

    }

    public class Bits
    {
        [JsonProperty("Length")]
        public int? Length { get; set; }

        [JsonProperty("Data")]
        public List<bool?> Data { get; set; }
    }

    public class BitsWithDescriptors
    {
        [JsonProperty("Length")]
        public int? Length { get; set; }

        [JsonProperty("Data")]
        public List<bool?> Data { get; set; }
    }

    public class Body
    {
        [JsonProperty("Bits")]
        public Bits Bits { get; set; }

        [JsonProperty("Refs")]
        public List<Ref> Refs { get; set; }

        [JsonProperty("Type")]
        public int? Type { get; set; }

        [JsonProperty("IsExotic")]
        public bool? IsExotic { get; set; }

        [JsonProperty("RefsCount")]
        public int? RefsCount { get; set; }

        [JsonProperty("BitsCount")]
        public int? BitsCount { get; set; }

        [JsonProperty("FullData")]
        public int? FullData { get; set; }

        [JsonProperty("Depth")]
        public int? Depth { get; set; }

        [JsonProperty("BitsWithDescriptors")]
        public BitsWithDescriptors BitsWithDescriptors { get; set; }

        [JsonProperty("Hash")]
        public Hash Hash { get; set; }
    }

    public class Data
    {
        [JsonProperty("Bits")]
        public Bits Bits { get; set; }

        [JsonProperty("Refs")]
        public List<object> Refs { get; set; }

        [JsonProperty("Type")]
        public int? Type { get; set; }

        [JsonProperty("IsExotic")]
        public bool? IsExotic { get; set; }

        [JsonProperty("RefsCount")]
        public int? RefsCount { get; set; }

        [JsonProperty("BitsCount")]
        public int? BitsCount { get; set; }

        [JsonProperty("FullData")]
        public int? FullData { get; set; }

        [JsonProperty("Depth")]
        public int? Depth { get; set; }

        [JsonProperty("BitsWithDescriptors")]
        public BitsWithDescriptors BitsWithDescriptors { get; set; }

        [JsonProperty("Hash")]
        public Hash Hash { get; set; }
    }

    public class Destination
    {
    }

    public class Fee
    {
    }

    public class ForwardTonAmount
    {
    }

    public class FwdFee
    {
    }

    public class Hash
    {
        [JsonProperty("Length")]
        public int? Length { get; set; }

        [JsonProperty("Data")]
        public List<bool?> Data { get; set; }
    }

    public class IhrFee
    {
    }

    public class InMsg
    {
        [JsonProperty("Source")]
        public Source Source { get; set; }

        [JsonProperty("Destination")]
        public Destination Destination { get; set; }

        [JsonProperty("Value")]
        public Value Value { get; set; }

        [JsonProperty("FwdFee")]
        public FwdFee FwdFee { get; set; }

        [JsonProperty("IhrFee")]
        public IhrFee IhrFee { get; set; }

        [JsonProperty("CreatedLt")]
        public long? CreatedLt { get; set; }

        [JsonProperty("BodyHash")]
        public string BodyHash { get; set; }

        [JsonProperty("MsgData")]
        public MsgData MsgData { get; set; }

        [JsonProperty("Message")]
        public object Message { get; set; }
    }

    public class MsgData
    {
        [JsonProperty("Text")]
        public object Text { get; set; }

        [JsonProperty("Body")]
        public Body Body { get; set; }

        [JsonProperty("InitState")]
        public object InitState { get; set; }
    }

    public class OutMsg
    {
        [JsonProperty("Source")]
        public Source Source { get; set; }

        [JsonProperty("Destination")]
        public Destination Destination { get; set; }

        [JsonProperty("Value")]
        public Value Value { get; set; }

        [JsonProperty("FwdFee")]
        public FwdFee FwdFee { get; set; }

        [JsonProperty("IhrFee")]
        public IhrFee IhrFee { get; set; }

        [JsonProperty("CreatedLt")]
        public object CreatedLt { get; set; }

        [JsonProperty("BodyHash")]
        public string BodyHash { get; set; }

        [JsonProperty("MsgData")]
        public MsgData MsgData { get; set; }

        [JsonProperty("Message")]
        public object Message { get; set; }
    }

    public class Ref
    {
        [JsonProperty("Bits")]
        public Bits Bits { get; set; }

        [JsonProperty("Refs")]
        public List<object> Refs { get; set; }

        [JsonProperty("Type")]
        public int? Type { get; set; }

        [JsonProperty("IsExotic")]
        public bool? IsExotic { get; set; }

        [JsonProperty("RefsCount")]
        public int? RefsCount { get; set; }

        [JsonProperty("BitsCount")]
        public int? BitsCount { get; set; }

        [JsonProperty("FullData")]
        public int? FullData { get; set; }

        [JsonProperty("Depth")]
        public int? Depth { get; set; }

        [JsonProperty("BitsWithDescriptors")]
        public BitsWithDescriptors BitsWithDescriptors { get; set; }

        [JsonProperty("Hash")]
        public Hash Hash { get; set; }
    }



    public class Source
    {
    }

    public class Transaction
    {
        [JsonProperty("Utime")]
        public int? Utime { get; set; }

        [JsonProperty("Data")]
        public object Data { get; set; }

        [JsonProperty("TransactionId")]
        public TransactionId TransactionId { get; set; }

        [JsonProperty("Fee")]
        public Fee Fee { get; set; }

        [JsonProperty("StorageFee")]
        public object StorageFee { get; set; }

        [JsonProperty("OtherFee")]
        public object OtherFee { get; set; }

        [JsonProperty("InMsg")]
        public InMsg InMsg { get; set; }

        [JsonProperty("OutMsgs")]
        public List<OutMsg> OutMsgs { get; set; }
    }

    public class TransactionId
    {
        [JsonProperty("lt")]
        public long? Lt { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }
    }

    public class Value
    {
    }


}