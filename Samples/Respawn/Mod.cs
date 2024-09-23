namespace Respawn;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Respawn), new PatchClass(this));
}