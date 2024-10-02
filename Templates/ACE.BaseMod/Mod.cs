namespace ACE.BaseMod;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(ACE.BaseMod), new PatchClass(this));
}