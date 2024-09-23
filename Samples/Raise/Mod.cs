namespace Raise;

public class Mod : BasicMod
{
    public Mod() : base() => base.Setup(nameof(Raise), new PatchClass(this));
}