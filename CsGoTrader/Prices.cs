using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGoTrader
{
    public class Prices
    {
        private List<double> lowestPrices;

        public Prices(List<double> lowestPrices)
        {
            this.lowestPrices = lowestPrices;
        }

        public double averagePrice(int numberToCount)
        {
            if(lowestPrices.Count == 0)
            {
                return 1000;
            }

            var iterator = lowestPrices.GetEnumerator();
            double priceSum = 0;
            for(int i=0; i<numberToCount; i++)
            {
                iterator.MoveNext();
                if (iterator.Current != null)
                {
                    priceSum += iterator.Current;                 
                }
                else{
                    numberToCount = i;
                    break;
                }

            }

            return priceSum / numberToCount;
        }
    }
}
