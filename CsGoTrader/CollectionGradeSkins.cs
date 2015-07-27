using System;
using System.Collections.Generic;

namespace CsGoTrader
{
    internal class CollectionGradeSkins
    {
        public List<Skin> skins;

        public CollectionGrade grade;

        public double averageLowestPricePerQuality(Quality quality, int offersNumber)
        {
            double lowestPrice = double.MaxValue;
            foreach (var skin in skins)
            {
                lowestPrice = Math.Min(lowestPrice, skin.averagePrice(quality, offersNumber));
            }

            return lowestPrice;
        }
    }
}