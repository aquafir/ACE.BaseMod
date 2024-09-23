namespace ExtendACE;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(ExtendACE), new PatchClass(this));
}