namespace QualityOfLife;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(QualityOfLife), new PatchClass(this));
}