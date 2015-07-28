using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace CsGoTrader
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Skin
    {
        public Dictionary<Quality, Prices> skinPrices;

        [JsonProperty]
        public string name;
        [JsonProperty]
        public double minFloatValue;
        [JsonProperty]
        public double maxFloatValue;
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public CollectionGrade collectionGrade;

        [JsonConstructor]
        public Skin(string name, double minFloatValue, double maxFloatValue, CollectionGrade collectionGrade)
        {
            this.name = name;
            this.minFloatValue = minFloatValue;
            this.maxFloatValue = maxFloatValue;
            this.collectionGrade = collectionGrade;
        }

        public Skin(string steamName)
        {
            skinPrices = new Dictionary<Quality, Prices>();

            // this will fetch prices from market
            foreach(Quality quality in Enum.GetValues(typeof(Quality))){
                skinPrices.Add(quality, new Prices(SteamMarket.getPrices(steamName, quality)));
            }
        }

        internal double averagePrice(Quality quality, int offersNumber)
        {
            return skinPrices[quality].averagePrice(offersNumber);
        }
    }
}