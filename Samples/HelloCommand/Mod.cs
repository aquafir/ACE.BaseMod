namespace HelloCommand;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(HelloCommand), new PatchClass(this));
}