namespace Tinkering;

public class Mod : BasicMod
{
    public Mod() : base() => base.Setup(nameof(global::Tinkering), new PatchClass(this));
}