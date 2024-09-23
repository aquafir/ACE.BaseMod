namespace CustomSpells;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(CustomSpells), new PatchClass(this));
}