namespace SelectiveStartup;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(SelectiveStartup), new PatchClass(this));
}