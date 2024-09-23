namespace Ironman;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Ironman), new PatchClass(this));
}