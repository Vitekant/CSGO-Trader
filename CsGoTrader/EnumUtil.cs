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

        internal static string getQualityShort(Quality quality)
        {
            switch (quality)
            {
                case Quality.BattleScared:
                    return "BS";
                case Quality.WellWorn:
                    return "WW";
                case Quality.FieldTested:
                    return "FT";
                case Quality.MinimalWear:
                    return "MW";
                case Quality.FactoryNew:
                    return "FN";
                default:
                    throw new Exception("Unsupported quality");
            }
        }

        internal static double getQualityBorder(Quality quality)
        {
            switch (quality)
            {
                case Quality.BattleScared:
                    return 1;
                case Quality.WellWorn:
                    return 0.45;
                case Quality.FieldTested:
                    return 0.37;
                case Quality.MinimalWear:
                    return 0.15;
                case Quality.FactoryNew:
                    return 0.07;
                default:
                    throw new Exception("Unsupported quality");
            }
        }
    }
}
