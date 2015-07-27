using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGoTrader
{
    public static class EnumUtil
    {
        public static string qualityToString(Quality quality)
        {
            switch (quality)
            {
                case Quality.BattleScared:
                    return "Battle-Scarred";
                case Quality.WellWorn:
                    return "Well-Worn";
                case Quality.FieldTested:
                    return "Field-Tested";
                case Quality.MinimalWear:
                    return "Minimal Wear";
                case Quality.FactoryNew:
                    return "Factory New";
                default:
                    throw new Exception("Unsupported quality");
            }
        }
    }
}
