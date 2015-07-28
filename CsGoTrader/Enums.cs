using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

public enum Quality
{
    BattleScared = 1,
    WellWorn = 2,
    FieldTested = 3,
    MinimalWear = 4,
    FactoryNew = 5,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum CollectionGrade
{
    ConsumerGrade = 1,
    IndustrialGgrade = 2,
    MilSpec = 3,
    Restricted = 4,
    Classified = 5,
    Covert = 6,
}