namespace Spells;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Spells), new PatchClass(this));
}