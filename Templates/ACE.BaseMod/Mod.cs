namespace ACE.BaseMod;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(BaseMod), new PatchClass(this));
}