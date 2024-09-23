namespace Discord;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Discord), new PatchClass(this));
}