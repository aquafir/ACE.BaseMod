namespace ChatFilter;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(ChatFilter), new PatchClass(this));
}