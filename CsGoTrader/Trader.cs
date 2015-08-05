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
            selenium.buySkins(new List<Tuple<Skin, Quality, double, int>>()
            {
                //new Tuple<Skin, Quality, double, int>(new Skin("CZ75-Auto | Pole Position", 0, 1, CollectionGrade.Classified), Quality.FactoryNew, 0.75, 10),
                new Tuple<Skin, Quality, double, int>(new Skin("SG 553 | Cyrex", 0, 1, CollectionGrade.Classified), Quality.FactoryNew, 7.5, 5),
                new Tuple<Skin, Quality, double, int>(new Skin("CZ75-Auto | Yellow Jacket", 0, 1, CollectionGrade.Classified), Quality.FactoryNew, 7.5, 5),
                new Tuple<Skin, Quality, double, int>(new Skin("MP7 | Nemesis", 0, 1, CollectionGrade.Classified), Quality.FactoryNew, 7.5, 5),
                new Tuple<Skin, Quality, double, int>(new Skin("MP9 | Ruby Poison Dart", 0, 1, CollectionGrade.Classified), Quality.FactoryNew, 0.75, 10)
            });
            //selenium.buySkin(new Skin("CZ75-Auto | Pole Position", 0, 1, CollectionGrade.Classified), Quality.FactoryNew, 0.75, 10);
            selenium.deinitializeDriver();
        }
    }
}
