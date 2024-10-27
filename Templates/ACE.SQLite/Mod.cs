namespace ACE.SQLite;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(ACE.SQLite), new PatchClass(this));
}