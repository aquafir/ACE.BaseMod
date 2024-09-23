namespace Tinkering;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Tinkering), new PatchClass(this));
}