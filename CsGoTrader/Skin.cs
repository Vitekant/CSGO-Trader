using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace CsGoTrader
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Skin
    {
        private Dictionary<Quality, Prices> skinPrices;

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
            skinPrices = new Dictionary<Quality, Prices>();


            //initalizePrices();
        }

        public void initalizePrices()
        {
            skinPrices = new Dictionary<Quality, Prices>();

            // this will fetch prices from market
            foreach (Quality quality in Enum.GetValues(typeof(Quality)))
            {
                skinPrices.Add(quality, new Prices(SteamMarket.getPrices(name, quality)));
            }
        }

        internal double averagePrice(Quality quality, int offersNumber)
        {
            if(!skinPrices.ContainsKey(quality) || skinPrices[quality] == null)
            {
                refreshPrices(quality, offersNumber);
            }

            return skinPrices[quality].averagePrice(offersNumber);
        }

        private void refreshPrices(Quality quality, int offersNumber)
        {
            skinPrices.Add(quality, new Prices(SteamMarket.getPrices(name, quality)));
        }

        public double getAverageFloatValue(Quality quality)
        {
            double min;
            double max;

            switch (quality)
            {
                case Quality.BattleScared:
                    min = 0.45;
                    max = 1;
                    break;
                case Quality.WellWorn:
                    min = 0.37;
                    max = 0.45;
                    break;
                case Quality.FieldTested:
                    min = 0.15;
                    max = 0.37;
                    break;
                case Quality.MinimalWear:
                    min = 0.07;
                    max = 0.15;
                    break;
                case Quality.FactoryNew:
                    min = 0;
                    max = 0.07;
                    break;
                default:
                    throw new Exception("Unsupported quality");
            }

            if(minFloatValue > max)
            {
                return 1000;
            }

            if(maxFloatValue < min)
            {
                return 1000;
            }

            return (Math.Max(min, minFloatValue) + Math.Min(max, maxFloatValue)) / 2;
        }

        public double getFloatValueRange()
        {
            return maxFloatValue - minFloatValue;
        }
    }
}