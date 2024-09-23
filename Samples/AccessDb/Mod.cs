namespace AccessDb;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(AccessDb), new PatchClass(this));
}