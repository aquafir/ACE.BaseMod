namespace Tower;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Tower), new PatchClass(this));
}