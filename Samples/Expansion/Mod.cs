namespace Expansion;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Expansion), new PatchClass(this));
}