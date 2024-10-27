namespace ACE.MySql;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(ACE.MySql), new PatchClass(this));
}