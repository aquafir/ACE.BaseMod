namespace AutoLoot;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(AutoLoot), new PatchClass(this));
}