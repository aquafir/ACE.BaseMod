using System.Runtime.CompilerServices;

namespace Balance;

public class AngouriPatchSettings
{
    //Type done elsewhere
    public bool Enabled { get; set; }
    public string Formula { get; set; }

    public static AngouriMathPatch Create(PatchType type)
    {

        var patch = new AngouriMathPatch()
        {
            Formula = 
        };
        //LevelCost();
        //return (PatchType)(new LevelCost());
    }
}

public enum PatchType
{
    GrantExperience,
    LevelCost,
    MeleeAttributeDamage,
    MissileAttributeDamage,
    NetherRating,
}