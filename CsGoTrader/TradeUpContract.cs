using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGoTrader
{
    public class TradeUpContract
    {
        public List<Tuple<Skin, Quality, int>> tradeUpList;

        public List<Tuple<Skin, Quality>> resultsList;

        public double potentialGain;

        public TradeUpContract()
        {
            tradeUpList = new List<Tuple<Skin, Quality, int>>();
            resultsList = new List<Tuple<Skin, Quality>>();
        }
    }
}
