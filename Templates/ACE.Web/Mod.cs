namespace ACE.Web;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(ACE.Web), new PatchClass(this));
}
