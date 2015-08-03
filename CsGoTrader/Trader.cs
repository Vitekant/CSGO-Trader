using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGoTrader
{
    public static class Trader
    {
        static void Main(string[] args)
        {
            var selenium = new SeleniumEngine();
            selenium.initializeWebDriver();
            selenium.buySkin(new Skin("UMP-45 | Scorched", 0, 1, CollectionGrade.Classified), Quality.WellWorn, 0.4, 4);
            selenium.deinitializeDriver();
        }
    }
}
