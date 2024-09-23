namespace Balance;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Balance), new PatchClass(this));
}