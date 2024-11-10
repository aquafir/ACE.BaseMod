namespace SocietyTailoring;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(SocietyTailoring), new PatchClass(this));
}