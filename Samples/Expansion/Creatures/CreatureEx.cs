//using Weenie = ACE.Entity.Models.Weenie;

namespace Expansion.Creatures;

[HarmonyPatch]
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
