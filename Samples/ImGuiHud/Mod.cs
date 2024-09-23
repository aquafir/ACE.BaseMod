namespace ImGuiHud;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(ImGuiHud), new PatchClass(this));
}