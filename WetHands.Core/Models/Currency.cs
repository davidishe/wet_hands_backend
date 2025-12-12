using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WetHands.Core.Models
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Currency

    // Российский рубль 5 380 000
    // Китай 17 000 000
    // Казахстан 367 000
    // Узбекистан 415 000
    // ОАЭ 4 532 00
    // Индия 1 500 000
    // Турция 1 953 000
    // Кыргызстан 21 700
    // Белоруссия 417 000
    // Грузия 320 000
    // Япония 1 240 000
    // Армения 180 000
    // Таджикистан 10 000

    {
        [EnumMember(Value = "Российский рубль")]
        RUB,

        [EnumMember(Value = "Китайский юань")]
        CNY,

        [EnumMember(Value = "Казахстанский тенге")]
        KZT,

        [EnumMember(Value = "Узбекский сум")]
        UZS,

        [EnumMember(Value = "Дирхам ОАЭ")]
        AED,

        [EnumMember(Value = "Индийский рупий")]
        INR,

        [EnumMember(Value = "Турецкая лира")]
        TRY,

        [EnumMember(Value = "Киргизский сом")]
        KGS,

        [EnumMember(Value = "Белорусский рубль")]
        BYN,

        [EnumMember(Value = "Грузинский лари")]
        GEL,

        [EnumMember(Value = "Японская йена")]
        JPY,

        [EnumMember(Value = "Армянский драм")]
        AMD,

        [EnumMember(Value = "Таджикский сомони")]
        TJS,



    }
}