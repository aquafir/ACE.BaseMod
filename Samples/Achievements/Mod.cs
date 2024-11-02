namespace Achievements;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Achievements), new PatchClass(this));
}