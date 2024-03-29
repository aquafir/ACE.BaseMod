namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureEx))]
public class CreatureEx : Creature
{
    public CreatureEx(Biota biota) : base(biota)
    {
        Initialize();
    }
    public CreatureEx(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
    {
        Initialize();
    }
    /// <summary>
    /// Called upon construction to set up creature
    /// </summary>
    protected virtual void Initialize() { }
}
