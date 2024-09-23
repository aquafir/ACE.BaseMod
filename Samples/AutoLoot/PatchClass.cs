namespace AutoLoot;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    
}

public enum CompareType
{
    GreaterThan,
    LessThanEqual,
    LessThan,
    GreaterThanEqual,
    NotEqual,
    NotEqualNotExist,
    Equal,
    NotExist,
    Exist,
    NotHasBits,
    HasBits,
}

