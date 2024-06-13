using ACE.Server.Realms;

namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureEx))]
public class CreatureEx : Creature
{
    public CreatureEx(Biota biota) : base(biota)
    {
        Initialize();
    }
#if REALM
    public CreatureEx(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public CreatureEx(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    {
        Initialize();
    }
    /// <summary>
    /// Called upon construction to set up creature
    /// </summary>
    protected virtual void Initialize() { }
}
