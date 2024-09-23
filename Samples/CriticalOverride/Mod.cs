namespace CriticalOverride;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(CriticalOverride), new PatchClass(this));
}