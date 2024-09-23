namespace EasyEnlightenment;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(EasyEnlightenment), new PatchClass(this));
}