namespace ConnectionLimit;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(ConnectionLimit), new PatchClass(this));
}