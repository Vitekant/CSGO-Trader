using System;
using System.Collections.Generic;

namespace CsGoTrader
{
    public class Skin
    {
        public Dictionary<Quality, Prices> skinPrices;

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