namespace CleaveTranspiler;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(CleaveTranspiler), new PatchClass(this));
}